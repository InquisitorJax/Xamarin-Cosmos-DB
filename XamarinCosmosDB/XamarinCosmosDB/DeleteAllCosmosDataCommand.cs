using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public interface IDeleteAllCosmosDataCommand : IAsyncLogicCommand<CosmosResourceRequest, CommandResult>
	{ }


	internal class DeleteAllCosmosDataCommand : AsyncLogicCommand<CosmosResourceRequest, CommandResult>, IDeleteAllCosmosDataCommand
	{
		private readonly HttpClient _httpClient;

		public DeleteAllCosmosDataCommand()
		{
			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			_httpClient = new HttpClient(handler);
		}

		public override async Task<CommandResult> ExecuteAsync(CosmosResourceRequest request)
		{
			var result = new CommandResult();

			string baseUrl = App.Settings[AppSettings.COSMOS_BROKER_BASE_URL];
#if DEBUG
			if (App.UseLocalResourceTokenBroker)
			{
				baseUrl = App.Settings[AppSettings.COSMOS_BROKER_BASE_URL_LOCAL]; //for local function execution
				if (Device.RuntimePlatform == Device.UWP)
				{
					baseUrl = "http://localhost:7071/";
				}
			}
#endif
			string deleteAccountUrl = App.Settings[AppSettings.COSMOS_DELETE_ACCOUNT_URL];
#if DEBUG
			if (App.UseLocalResourceTokenBroker)
			{
				deleteAccountUrl = App.Settings[AppSettings.COSMOS_DELETE_ACCOUNT_URL_LOCAL]; //for local function execution
			}
#endif

			Uri baseUri = new Uri(baseUrl);
			//temp add userId query parameter to bypass adding full auth header
			deleteAccountUrl += $"?userId={request.AuthAccessToken}";
			Uri callUri = new Uri(baseUri, deleteAccountUrl);

			_httpClient.Init(callUri);
			var response = await _httpClient.GetAsync(callUri).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				result.Notification.Fail("Failed to delete all the partition data! " + response.ReasonPhrase);
			}

			return result;
		}
	}
}
