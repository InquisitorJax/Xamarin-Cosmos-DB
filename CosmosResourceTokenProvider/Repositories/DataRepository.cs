using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CosmosResourceTokenProvider.Repositories
{
	public interface IDataRepository
	{
		Task DeleteAllAsync(string userId);
	}


	internal class DataRepository : IDataRepository
	{
		private readonly ILogger _log;
		public DataRepository(ILogger log)
		{
			_log = log;
		}

		public Task DeleteAllAsync(string userId)
		{
			// doc: https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-delete-by-partition-key?tabs=dotnet-example
			// still in preview: https://github.com/AzureCosmosDB/PartitionKeyDeleteFeedbackGroup

			var cosmosClientResult = CosmosClientFactory.Create();
			var container = cosmosClientResult.Container;

			var response = container.DeleteAllItemsByPartitionKeyStreamAsync(new Microsoft.Azure.Cosmos.PartitionKey(userId));
			if (!response.IsCompletedSuccessfully)
			{
				_log.LogError($"Whoops! Something went wrong trying to delete the partition data for {userId}");
			}
			else
			{
				_log.LogInformation($"Delete all documents with partition key operation has successfully started for {userId}");
			}

			return Task.CompletedTask;
		}
	}
}
