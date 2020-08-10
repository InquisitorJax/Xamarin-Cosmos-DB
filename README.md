# Xamarin-Cosmos-DB
sample project demonstrating using CosmosDB from Xamarin Apps

### Known Issues
- pointing to local CosmosDB Emulator: calls hang in what seems like a deadlock https://github.com/InquisitorJax/Xamarin-Cosmos-DB/issues/1
- When adding "AllowBulkExecution" = true, CosmosDB returns 403 Forbidden response on any calls.

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
