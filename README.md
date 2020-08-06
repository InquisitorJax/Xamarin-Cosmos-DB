# Xamarin-Cosmos-DB
sample project demonstrating using CosmosDB from Xamarin Apps (in progress)

### Switching Environments

##### Azure Function
- Copy "sample.settings.json" as "local.settings.json"
- Fill in the values in the square brackets
- Use the appropriate App Setting in ResourceTokenBroker.CosmosConnection to either get a resource token from the azure Cosmos instance, or the local emulator instance
- Make sure that DbName & UserDataCollectionName have been set up in the settings

##### Mobile App (Android)
- Set MainViewModel.UseLocalResourceTokenBroker to fetch the token from local function, or azure function
- Set MainViewModel.UseLocalCosmosDB to operate against the local CosmosDB Emulator, or azure CosmosDB

### Current Results
- Resource token works
- Local CosmosDB create attempt hangs on the api call
- Azure CosmosDB create attempt returns a 403 Forbidden result (Firewall "allow public access" has been enabled)
