using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosResourceTokenProvider
{
	public class CustomPermissionResponse
	{
		public string Id { get; set; }

		public DateTimeOffset TokenExpiry { get; set; }

		public string Token { get; set; }

		public string PartitionKey { get; set; }
	}
}
