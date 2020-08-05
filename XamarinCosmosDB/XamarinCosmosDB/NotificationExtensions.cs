using Wibci.LogicCommand;

namespace XamarinCosmosDB
{
	public static class NotificationExtensions
	{
		public static void Fail(this Notification notification, string message)
		{
			if (notification != null)
			{
				notification.Add(new NotificationItem(message));
			}
		}
	}
}
