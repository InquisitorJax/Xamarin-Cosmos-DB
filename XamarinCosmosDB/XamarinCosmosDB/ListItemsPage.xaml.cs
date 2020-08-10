
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinCosmosDB
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListItemsPage : ContentPage
	{
		private ListItemsViewModel _viewModel;

		public ListItemsPage()
		{
			InitializeComponent();
			_viewModel = DependencyService.Get<ListItemsViewModel>();
			BindingContext = _viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.FetchRecordsAsync();
		}

	}
}