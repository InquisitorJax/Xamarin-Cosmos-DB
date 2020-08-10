using System.ComponentModel;
using Xamarin.Forms;

namespace XamarinCosmosDB
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			var viewModel = DependencyService.Get<MainViewModel>();
			BindingContext = viewModel;
		}

		private void Button_Clicked(object sender, System.EventArgs e)
		{
			this.Navigation.PushAsync(new ListItemsPage());
		}
	}
}
