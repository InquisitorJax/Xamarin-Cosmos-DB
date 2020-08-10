using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public class TestModelTemplateSelector : DataTemplateSelector
	{
		public DataTemplate TestModelTemplate { get; set; }

		public DataTemplate TestModel2Template { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item is TestModel2)
			{
				// test for derived type first
				return TestModel2Template;
			}

			if (item is TestModel)
			{
				return TestModelTemplate;
			}

			return null;
		}
	}
}
