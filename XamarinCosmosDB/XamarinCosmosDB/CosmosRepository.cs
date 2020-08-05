using Microsoft.Azure.Cosmos;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace XamarinCosmosDB
{

	public interface ICosmosRepository
	{
		void UpdateToken(string token);

		Task<Notification> SaveModelAsync<T>(T model) where T : ModelBase, new();
	}

	public class CosmosRepository : ICosmosRepository
	{
		private readonly ICosmosClientFactory _clientFactory;

		private Container _container;
		private PartitionKey _partitionKey;
		private string _token;

		public CosmosRepository()
		{
			_clientFactory = DependencyService.Get<ICosmosClientFactory>();
		}

		public void UpdateToken(string token)
		{
			_token = token;
		}

		public async Task<Notification> SaveModelAsync<T>(T model) where T : ModelBase, new()
		{
			var result = CheckServiceCanOperate();

			if (!result.IsValid())
			{
				return result;
			}

			try
			{
				var docToSave = new CosmosDocument<T>(model);

				await _container.UpsertItemAsync(docToSave, _partitionKey); //.ConfigureAwait(false);
			}
			catch (CosmosException cex)
			{
				result.Fail($"failed to save document {typeof(T).Name} Reason: {cex.StatusCode}");
				//TODO: Handle Cosmos exceptions
				//doc: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.container.upsertitemasync?view=azure-dotnet
			}
			catch (ConfigurationException)
			{
				// There's an issue with Android and the System.Configuration.ConfigurationManager: https://github.com/xamarin/Xamarin.Forms/issues/5935
				// For now we are ignoring ConfigurationExceptions as it seems that the operation works despite the exception.
				Debug.WriteLine("Configuration Exception calling cosmos - current Android Bug issue https://github.com/xamarin/Xamarin.Forms/issues/5935");
			}
			catch
			{
				result.Fail($"failed to save document {typeof(T).Name}");
			}

			return result;
		}

		public bool IsInitialized { get; private set; }

		private void Initialize()
		{
			if (!IsInitialized)
			{
				CreateClient();
			}
		}

		private Notification CheckServiceCanOperate()
		{
			var result = Notification.Success();

			if (string.IsNullOrEmpty(_token))
			{
				result.Fail("We need a resource token please :(");
			}

			if (!IsInitialized)
			{
				Initialize();
			}

			if (_container == null)
			{
				result.Fail("Document Client did not initialize :(");
			}

			return result;
		}

		private void CreateClient()
		{
			_container = _clientFactory.Create(_token, allowBulkExecution: true); //allow bulk execution for multi-record updates
																			//todo: database endpoint url, and container id should come from the token?
			if (_container != null)
			{
				_partitionKey = new PartitionKey(App.CurrentUserId);
				IsInitialized = true;
			}
		
		}



	}
}
