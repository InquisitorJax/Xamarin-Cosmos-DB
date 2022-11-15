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
	public interface IFetchCosmosResourceTokenCommand : IAsyncLogicCommand<CosmosResourceTokenRequest, CosmosResourceTokenResult>
	{
	}

	public class FetchCosmosResourceTokenCommand : AsyncLogicCommand<CosmosResourceTokenRequest, CosmosResourceTokenResult>, IFetchCosmosResourceTokenCommand
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

		public override async Task<CosmosResourceTokenResult> ExecuteAsync(CosmosResourceTokenRequest request)
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

			InitializeHttpClient(callUri);
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

		private void InitializeHttpClient(Uri baseUri)
		{
			_httpClient.DefaultRequestHeaders.Clear();

			//_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}"); Access token obtained from Auth Component - we're just using fake userId for this example
			_httpClient.DefaultRequestHeaders.Add("User-Agent", $"ResourceTokenBroker/0.9.0");
			_httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
			_httpClient.DefaultRequestHeaders.Add("Host", $"{baseUri.Host}");
			_httpClient.DefaultRequestHeaders.Add("Accept-Encoding", $"gzip, deflate, br");
			_httpClient.DefaultRequestHeaders.Add("Cache-Control", $"no-cache");
			_httpClient.DefaultRequestHeaders.Add("Connection", $"keep-alive");
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

	public class CosmosResourceTokenRequest
	{
		public CosmosResourceTokenRequest(string authAccessToken)
		{
			AuthAccessToken = authAccessToken;
		}
		public string AuthAccessToken { get; }
	}

}
