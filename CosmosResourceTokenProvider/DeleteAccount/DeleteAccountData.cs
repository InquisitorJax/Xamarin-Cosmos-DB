using CosmosResourceTokenProvider.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace CosmosResourceTokenProvider.DeleteAccount
{
	internal class DeleteAccountData : FunctionBase
	{
		private readonly IDataRepository _dataRepository;

		public DeleteAccountData(IDataRepository dataRepository)
		{
			_dataRepository = dataRepository;
		}

		[FunctionName("DeleteAccount")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest request,
			ILogger log)
		{
			log.LogInformation($"Remotime Permission HTTP trigger function processing a request to delete account.");

			string userId;
			try
			{
				//var userIdResponse = AuthHelper.GetUserId(request);

				//if (!userIdResponse.IsValid() || userIdResponse.UserId == null)
				//{
				//	return LogErrorAndReturnBadObjectResult(userIdResponse.Notification.ToString(), log);
				//}
				//userId = userIdResponse.UserId;

				// for this example, we're just sending userId as a query parameter
				// sample code for getting auth header from request can be found in AuthHelper class
				userId = request.Query["userId"];

				log.LogInformation($"UserId: {userId}");

			}
			catch (Exception ex)
			{
				return LogErrorAndReturnBadObjectResult("Unable to read access token from auth header", log, ex);
			}

			try
			{
				await _dataRepository.DeleteAllAsync(userId);
			}
			catch (Exception ex)
			{
				return LogInternalServerError(log, ex);
			}

			return new OkResult();
		}
	}
}
