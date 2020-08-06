using Microsoft.Azure.Cosmos;
using System;
using System.Diagnostics;

namespace XamarinCosmosDB
{
	public interface ICosmosClientFactory
	{
		Container Create(string token, bool allowBulkExecution = false);
	}

	public class CosmosClientFactory : ICosmosClientFactory
	{
		private CosmosClient _client;
		private Container _userDataContainer;

		private string _cosmosAccessToken;

		public Container Create(string token, bool allowBulkExecution = false)
		{
			try
			{
				// NOTE: If the access token was updated, then we need to get a new instance of the CosmosClient
				if ( _cosmosAccessToken == token && _client != null && _userDataContainer != null)
				{
					return _userDataContainer;
				}
				
				_cosmosAccessToken = token;

				if (string.IsNullOrEmpty(_cosmosAccessToken))
				{
					Debug.WriteLine("Could not create Cosmos Client - there is no Cosmos Resource Token!");
					return null;
				}

				var options = new CosmosClientOptions { AllowBulkExecution = allowBulkExecution };
				string connectionString = App.Settings[AppSettings.COSMOS_DB_URL];

#if DEBUG
				if (App.UseLocalCosmosDB)
				{
					//using static connection (not the resource token)
					//connectionString = App.Settings[AppSettings.COSMOS_DB_CONNECTION_STRING_LOCAL];
					//_client = new CosmosClient(connectionString, options); 

					//using resource token:
					connectionString = App.Settings[AppSettings.COSMOS_DB_URL_LOCAL];
				}
#endif
				_client = new CosmosClient(connectionString, _cosmosAccessToken, options);

				string databaseName = App.Settings[AppSettings.COSMOS_DB_NAME];
				string collectionName = App.Settings[AppSettings.COSMOS_COLLECTION_NAME];


				//NOTE: Assumes the database and collection have already been created in the portal
				_userDataContainer = _client.GetDatabase(databaseName).GetContainer(collectionName);

				return _userDataContainer;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex, "Could not create CosmosDB Client :(");
				return null;
			}
		}
	}
}
