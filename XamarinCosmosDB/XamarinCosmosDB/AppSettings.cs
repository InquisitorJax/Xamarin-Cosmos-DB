using Newtonsoft.Json.Linq;

namespace XamarinCosmosDB
{
	public class AppSettings
	{
		public const string COSMOS_BROKER_BASE_URL = "CosmosBrokerBaseUrl";
		public const string COSMOS_BROKER_BASE_URL_LOCAL = "CosmosBrokerBaseUrl_Local";
		public const string COSMOS_USER_BROKER_URL = "CosmosUserBrokerUrl";
		public const string COSMOS_USER_BROKER_URL_LOCAL = "CosmosUserBrokerUrl_Local";
		public const string COSMOS_DB_URL = "CosmosDBUrl";
		public const string COSMOS_DB_URL_LOCAL = "CosmosDBUrl_Local";
		public const string COSMOS_DB_CONNECTION_STRING_LOCAL = "CosmosDBUrl_Local_ConnectionString";
		public const string COSMOS_DB_NAME = "CosmosDBName";
		public const string COSMOS_COLLECTION_NAME = "CosmosCollectionName";

		private const string NAMESPACE = "XamarinCosmosDB"; //root namespace of files to read

#if DEBUG
		private const string FILE_NAME = "appsettings.local.json";
#else
        private const string FILE_NAME = "appsettings.json";
#endif

		private readonly JObject _secrets;

		public AppSettings()
		{
			var fileString = FILE_NAME.GetStringFromFile(NAMESPACE, typeof(AppSettings).Assembly.FullName);
			_secrets = JObject.Parse(fileString);
		}

		public string this[string name]
		{
			get
			{
				try
				{
					var path = name.Split(':');

					JToken node = _secrets[path[0]];
					for (int index = 1; index < path.Length; index++)
					{
						node = node[path[index]];
					}

					return node.ToString();
				}
				catch
				{
					return string.Empty;
				}
			}
		}
	}
}
