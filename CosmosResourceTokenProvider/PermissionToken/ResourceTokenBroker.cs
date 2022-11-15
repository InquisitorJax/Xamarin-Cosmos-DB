using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

namespace CosmosResourceTokenProvider.PermissionToken
{
    //doc: https://github.com/1iveowl/CosmosResourceTokenBroker (extensive example using scopes for different permissions)

    //links:  https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=csharp#http-trigger-get-multiple-docs-using-documentclient-c
    //        https://docs.microsoft.com/en-us/sandbox/functions-recipes/cosmos-db?tabs=csharp
    //	      https://codemilltech.com/adding-azure-ad-b2c-authentication-to-azure-functions/

    // in-process vs isolated worker process functions (https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-in-process-differences)
    // - currently this is an in-process function

    // upgrading function apps in azure (using slots)
    // https://learn.microsoft.com/en-us/azure/azure-functions/migrate-version-3-version-4?tabs=net6-in-proc%2Cazure-cli%2Cwindows&pivots=programming-language-csharp#upgrade-using-slots

    /// <summary>
    /// Get a resource token based on userID (UserID is the partition key for storing records)
    /// </summary>
    public static class ResourceTokenBroker
    {

        static CosmosClient _cosmosClient;
        static Database _database;
        static Container _container;

        [FunctionName("CosmosResourceTokenBroker")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
         
            // for this example, we're just sending userId as a query parameter
            // sample code for getting auth header from request can be found in AuthHelper class
            string userId = request.Query["userId"];

            log.LogInformation($"UserId: {userId}");

            //TODO: Check for scopes, and return partition permissions based on scope

            try
            {
				var clientResult = CosmosClientFactory.Create();
				var cosmosClient = clientResult.Client;
				_database = clientResult.Database;
				_container = clientResult.Container;

                var permission = await GetPartitionPermissionAsync(userId, log);

                return new OkObjectResult(permission);
            }
            catch (Exception ex)
            {
                return LogErrorAndReturnBadObjectResult("Unable to get permission token", log, ex);
            }
        }

        private static BadRequestObjectResult LogErrorAndReturnBadObjectResult(string error, ILogger log, Exception ex = default)
        {
            log.Log(LogLevel.Error, ex is null ? $"{error}" : $"{error}. Unhandled exception: {ex}");
            return new BadRequestObjectResult(ex is null ? $"{error}" : $"{error}. Unhandled exception: {ex}");
        }

        static async Task<CustomPermissionResponse> GetPartitionPermissionAsync(string userId, ILogger log)
        {
            var permissionId = userId; //$"{userId}-{Constants.UserReadWriteScope}";
            CustomPermissionResponse retPermission = new CustomPermissionResponse(); ;

            var user = await GetOrCreatePermissionUserAsync(userId);

            // NOTE: make this lifespan something like TimeSpan.FromMinutes(10); to test token recovery in the mobile app
            var tokenLifetimeSpan = TimeSpan.FromMinutes(10); //default value is 1 hour
            int tokenLifetime = Convert.ToInt32(tokenLifetimeSpan.TotalSeconds); //default is 1 hour - should this be longer?
            retPermission.TokenExpiry = DateTime.UtcNow.Add(tokenLifetimeSpan);

            try
            {
                // read the permission that has been set up for an existing user
                var permission = user.GetPermission(permissionId);
                var permissionResponse = await permission.ReadAsync(tokenLifetime);
                retPermission.Id = permissionResponse.Resource.Id;
                retPermission.Token = permissionResponse.Resource.Token;
                retPermission.PartitionKey = userId;

                log.LogInformation($"Successful existing permission token generated for user {userId}");

                return retPermission;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return await UpsertPermissionAsync(user, permissionId, tokenLifetime, log);
                }

                throw new ResourceTokenBrokerException($"Unable to read or create user permissions. Unhandled exception: {ex}");
            }
        }

        static async Task<CustomPermissionResponse> UpsertPermissionAsync(User user, string permissionId, int expiryInSeconds, ILogger log)
        {
            try
            {
                var partitionKey = new PartitionKey(permissionId);

                var permissionProperties = new PermissionProperties(
                    permissionId,
                    PermissionMode.All, //default to all. This should be derived from scope in future
                    _container,
                    partitionKey);

                var permissionResponse = await user.UpsertPermissionAsync(permissionProperties, expiryInSeconds);

                if (permissionResponse?.StatusCode == HttpStatusCode.OK || permissionResponse?.StatusCode == HttpStatusCode.Created)
                {
                    if (!string.IsNullOrEmpty(permissionResponse?.Resource?.Token) && !string.IsNullOrEmpty(permissionResponse?.Resource?.Id))
                    {
                        log.LogInformation($"Successful permission token generated for user {permissionId}");

                        return new CustomPermissionResponse()
                        {
                            Id = permissionResponse.Resource.Id,
                            Token = permissionResponse.Resource.Token,
                            TokenExpiry = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(expiryInSeconds)),
                            PartitionKey = permissionId
                        };
                    }
                }

                throw new ResourceTokenBrokerException($"Unable to upsert permission for user. Token or Id is missing or invalid. Status code: {permissionResponse?.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new ResourceTokenBrokerException($"Unable to upsert new permission for user. Unhandled exception: {ex}");
            }
        }

        static async Task<User> GetOrCreatePermissionUserAsync(string userId)
        {
            var permissionUserId = userId; //When diff scopes, append scope to the permission. for now, userid on it's own assumes read-write scope

            try
            {
                var user = _database.GetUser(permissionUserId);

                var userResponse = await user.ReadAsync();

                // If the user does not exist, then create it.
                // This if statement is probably not necessary, as an CosmosException is throw if the user.ReadAsync fails:
                // however the documentation is not 100 % clear on this: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.database.readasync?view=azure-dotnet
                if (userResponse?.StatusCode != HttpStatusCode.OK || userResponse?.User is null)
                {
                    userResponse = await _database.CreateUserAsync(permissionUserId);
                }

                return userResponse.User;
            }
            catch (CosmosException ex)
            {
                // If the user does not exist, then create it.
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    var user = await _database.CreateUserAsync(permissionUserId);
                    return user;
                }

                throw new ResourceTokenBrokerException($"Unable to get or create user with user id: {permissionUserId}. Unhandled exception: {ex}");
            }
        }
    }
}
