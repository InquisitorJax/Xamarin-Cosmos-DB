using Microsoft.Azure.Cosmos;
using System;
using System.Diagnostics;
using System.Net.Http;
using Xamarin.Forms;

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

				if (!App.UseLocalCosmosDB && string.IsNullOrEmpty(_cosmosAccessToken))
				{
					Debug.WriteLine("Could not create Cosmos Client - there is no Cosmos Resource Token!");
					return null;
				}

				var options = new CosmosClientOptions { AllowBulkExecution = allowBulkExecution };
				string connectionString = App.Settings[AppSettings.COSMOS_DB_URL];

#if DEBUG
				if (App.UseLocalCosmosDB)
				{
					//using resource token:
					connectionString = App.Settings[AppSettings.COSMOS_DB_LOCAL];
					if (Device.RuntimePlatform == Device.UWP)
					{
						connectionString = "https://localhost:8081/";
					}
					else
					{
						//bypass SSL for iOS / Android emulators
						options.HttpClientFactory = () =>
							{
								HttpMessageHandler httpMessageHandler = new HttpClientHandler()
								{
									ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
								};
								return new HttpClient(httpMessageHandler);
							};
						options.ConnectionMode = ConnectionMode.Gateway;							
					}

					//var localKey = string.Format(App.Settings[AppSettings.COSMOS_DB_LOCAL_KEY], connectionString);
				}
#endif
				_client = new CosmosClient(connectionString, _cosmosAccessToken, options); //403 Forbidden error if include "AllowBulkExecution" = true

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
