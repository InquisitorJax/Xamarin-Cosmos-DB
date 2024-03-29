# Xamarin-Cosmos-DB
Sample project demonstrating using CosmosDB from Xamarin Apps
Uses the latest v4 SDK for CosmosDB + .NET 6
- Example uses single collection container to store different domain types
- Records are partitioned by UserId
- Delete all records by partition key is still in preview. See more [here](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-delete-by-partition-key?tabs=dotnet-example)

### Known Issues
- When adding "AllowBulkExecution" = true, CosmosDB returns 403 Forbidden response on any calls.
  https://github.com/Azure/azure-cosmos-dotnet-v3/issues/1783
- UWP fetch records throws internal 500 error when there are no records for a type

### Local CosmosDB Emulator Testing
-  Make sure that you setup allow network access: https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator#running-on-a-local-network
Your commandline to start Cosmos should look something like: 
"C:\Program Files\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe" /NoFirewall /AllowNetworkAccess /Key=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==
- Also key is the bypassing of SSL code you can find in the CosmosClientFactory class
- You may also first have to delete the data directory %LOCALAPPDATA%\CosmosDBEmulator. [link](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#enable-access-to-emulator-on-a-local-network)

### Switching Environments

##### Azure Function
- Copy "sample.settings.json" as "local.settings.json"
- Fill in the values in the square brackets
- Use the appropriate App Setting in ResourceTokenBroker.CosmosConnection to either get a resource token from the azure Cosmos instance, or the local emulator instance
- Make sure that DbName & UserDataCollectionName have been set up in the settings

##### Mobile App (Android)
- Copy appSettings.json to appsettings.local.json
- For the android emulator, user the IPv4 address of your local machine to access local function & Cosmos Emulator.
- For Windows UWP, localhost has been set in code
- Set MainViewModel.UseLocalResourceTokenBroker to fetch the token from local function, or azure function
- Set MainViewModel.UseLocalCosmosDB to operate against the local CosmosDB Emulator, or azure CosmosDB

### Authentication
Authentication is not covered in this example. We're just sending a hard-coded userID via a query parameter to the function.
There is, however, some sample code on how to determine the user id from a JWT access token in the azure function.
More detail on auth with AAD B2C here: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/authentication/azure-ad-b2c

This code wouldn't have been possible without the very helpful repo found here:
https://github.com/1iveowl/CosmosResourceTokenBroker

