using System.Collections.Generic;
using Wibci.LogicCommand;

namespace XamarinCosmosDB
{
	public class FetchModelCollectionResult<T> : CommandResult where T : ModelBase
	{
		public FetchModelCollectionResult()
		{
			ModelCollection = new List<T>();
		}

		public List<T> ModelCollection { get; set; }
	}

	public static class CommandResultExtensions
	{
		public static void Fail(this CommandResult result, string message)
		{
			if (result != null)
			{
				result.Notification.Fail(message);
			}
		}
	}
}
