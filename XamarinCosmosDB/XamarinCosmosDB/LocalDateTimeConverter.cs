using System;
using System.Globalization;

namespace XamarinCosmosDB
{
	public class LocalDateTimeConverter : Xamarin.Forms.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((DateTime)value).ToLocalTime();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
