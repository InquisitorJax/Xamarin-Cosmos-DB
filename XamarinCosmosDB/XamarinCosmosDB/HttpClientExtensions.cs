using System;
using System.Net.Http;

namespace XamarinCosmosDB
{
	internal static class HttpClientExtensions
	{
		public static void Init(this HttpClient httpClient, Uri baseUri)
		{
			httpClient.DefaultRequestHeaders.Clear();

			//_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}"); Access token obtained from Auth Component - we're just using fake userId for this example
			httpClient.DefaultRequestHeaders.Add("User-Agent", $"ResourceTokenBroker/0.9.0");
			httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
			httpClient.DefaultRequestHeaders.Add("Host", $"{baseUri.Host}");
			httpClient.DefaultRequestHeaders.Add("Accept-Encoding", $"gzip, deflate, br");
			httpClient.DefaultRequestHeaders.Add("Cache-Control", $"no-cache");
			httpClient.DefaultRequestHeaders.Add("Connection", $"keep-alive");
		}
	}
}
