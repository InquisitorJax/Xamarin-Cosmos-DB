using CosmosResourceTokenProvider;
using Wibci.LogicCommand;

namespace XamarinCosmosDB
{
	public interface IFetchCosmosResourceTokenCommand : IAsyncLogicCommand<CosmosResourceTokenRequest, CosmosResourceTokenResult>
	{
	}

	public class FetchCosmosResourceTokenCommand
	{
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
