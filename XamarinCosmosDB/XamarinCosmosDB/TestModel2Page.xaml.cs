
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinCosmosDB
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestModel2Page : ContentPage
	{
		public TestModel2Page(TestModel2 model)
		{
			InitializeComponent();

			var viewModel = DependencyService.Get<TestModel2ViewModel>();
			viewModel.Model = model;

			BindingContext = viewModel;
		}
	}
}