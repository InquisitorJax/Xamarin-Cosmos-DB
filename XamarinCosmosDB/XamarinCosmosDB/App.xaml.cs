﻿using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			DependencyService.Register<IFetchCosmosResourceTokenCommand, FetchCosmosResourceTokenCommand>();
			DependencyService.Register<IDeleteAllCosmosDataCommand, DeleteAllCosmosDataCommand>();

			DependencyService.RegisterSingleton<ICosmosRepository>(new CosmosRepository());
			DependencyService.Register<ICosmosClientFactory, CosmosClientFactory>();

			DependencyService.Register<MainViewModel>();
			DependencyService.Register<ListItemsViewModel>();
			DependencyService.Register<TestModel2ViewModel>();

			Settings = new AppSettings();

			MainPage = new NavigationPage(new MainPage());
		}

		public static AppSettings Settings { get; private set; }

		public static string CurrentUserId { get; set; }

		public static bool UseLocalResourceTokenBroker { get; set; }

		public static bool UseLocalCosmosDB { get; set; }

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
