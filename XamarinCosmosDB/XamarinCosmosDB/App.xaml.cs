using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			DependencyService.Register<IFetchCosmosResourceTokenCommand, FetchCosmosResourceTokenCommand>();
			DependencyService.Register<ICosmosRepository, CosmosRepository>();
			DependencyService.Register<ICosmosClientFactory, CosmosClientFactory>();

			DependencyService.Register<MainViewModel>();

			Settings = new AppSettings();

			MainPage = new MainPage();
		}

		public static AppSettings Settings { get; private set; }

		public static string CurrentUserId { get; set; }

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
