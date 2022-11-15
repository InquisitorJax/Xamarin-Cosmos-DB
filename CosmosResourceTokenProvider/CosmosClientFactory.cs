using Microsoft.Azure.Cosmos;
using System;

namespace CosmosResourceTokenProvider
{
	internal class CosmosClientFactory
	{
		static readonly string DbName = Environment.GetEnvironmentVariable("CosmosDBName");
		static readonly string UserDataCollectionName = Environment.GetEnvironmentVariable("CosmosContainerName");
		static readonly string CosmosConnection = Environment.GetEnvironmentVariable("CosmosConnectionString_Local"); //"_Local";

		public static CosmosDBClientFactoryResult Create()
		{
			var cosmosClient = new CosmosClient(CosmosConnection);
			var database = cosmosClient.GetDatabase(DbName);
			var container = database.GetContainer(UserDataCollectionName);

			return new CosmosDBClientFactoryResult(cosmosClient, database, container);
		}

		public record CosmosDBClientFactoryResult(CosmosClient Client, Database Database, Container Container);
	}
}
