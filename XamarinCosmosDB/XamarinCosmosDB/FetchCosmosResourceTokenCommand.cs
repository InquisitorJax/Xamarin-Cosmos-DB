using CosmosResourceTokenProvider.PermissionToken;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public interface IFetchCosmosResourceTokenCommand : IAsyncLogicCommand<CosmosResourceRequest, CosmosResourceTokenResult>
	{
	}

	public class FetchCosmosResourceTokenCommand : AsyncLogicCommand<CosmosResourceRequest, CosmosResourceTokenResult>, IFetchCosmosResourceTokenCommand
	{
		private readonly HttpClient _httpClient;

		public FetchCosmosResourceTokenCommand()
		{
			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			_httpClient = new HttpClient(handler);
		}

		public override async Task<CosmosResourceTokenResult> ExecuteAsync(CosmosResourceRequest request)
		{
			var result = new CosmosResourceTokenResult();

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
			string userBrokerUrl = App.Settings[AppSettings.COSMOS_USER_BROKER_URL];
#if DEBUG
			if (App.UseLocalResourceTokenBroker)
			{
				userBrokerUrl = App.Settings[AppSettings.COSMOS_USER_BROKER_URL_LOCAL]; //for local function execution
			}
#endif

			Uri baseUri = new Uri(baseUrl);
			//temp add userId query parameter to bypass adding full auth header
			userBrokerUrl += $"?userId={request.AuthAccessToken}";
			Uri callUri = new Uri(baseUri, userBrokerUrl);

			_httpClient.Init(callUri);
			var response = await _httpClient.GetAsync(callUri).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				if (response.Content is null)
				{
					throw new Exception("Error acquiring resource token. Content missing is response from resource token broker.");
				}

				string resourceTokenResult = await response.Content.ReadAsStringAsync();
				result.TokenResponse = JsonConvert.DeserializeObject<CustomPermissionResponse>(resourceTokenResult);
			}
			else
			{
				result.Notification.Fail("Could not get access token from CosmosDB Server Function! " + response.ReasonPhrase);
			}


			return result;
		}
	}

	public class CosmosResourceTokenResult : CommandResult
	{
		private CustomPermissionResponse tokenResponse;

		public CustomPermissionResponse TokenResponse
		{
			get => tokenResponse ?? new CustomPermissionResponse();
			set => tokenResponse = value;
		}
	}

	public class CosmosResourceRequest
	{
		public CosmosResourceRequest(string authAccessToken)
		{
			AuthAccessToken = authAccessToken;
		}
		public string AuthAccessToken { get; }
	}

}
