using Microsoft.Azure.Cosmos;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace XamarinCosmosDB
{

	public class MainViewModel : ViewModelBase
	{

		private readonly IFetchCosmosResourceTokenCommand _fetchTokenLogic;

		public MainViewModel()
		{
			_fetchTokenLogic = DependencyService.Get<IFetchCosmosResourceTokenCommand>();

			FetchResourceTokenCommand = new DelegateCommand(async () => await FetchResourceTokenAsync());
			CreateDBRecordCommand = new DelegateCommand(CreateDBRecord);
			CreateDBRecord2Command = new DelegateCommand(CreateDBRecord2);

			App.CurrentUserId = "test-user-id";
			UseLocalResourceTokenBroker = true;
			UseLocalCosmosDB = true;
		}

		public ICommand FetchResourceTokenCommand { get; }

		public ICommand CreateDBRecordCommand { get; }

		public ICommand CreateDBRecord2Command { get; }

		private string _token;

		public string Token
		{
			get { return _token; }
			set { SetProperty(ref _token, value); }
		}

		private int _tokenExpiryMinutes;

		public int TokenExpiryMinutes
		{
			get { return _tokenExpiryMinutes; }
			set { SetProperty(ref _tokenExpiryMinutes, value); }
		}

		private DateTimeOffset _tokenExpiry;

		public DateTimeOffset TokenExpiry
		{
			get { return _tokenExpiry; }
			set
			{
				SetProperty(ref _tokenExpiry, value);
				var dateDiff = _tokenExpiry.Subtract( DateTime.Now.ToUniversalTime());
				TokenExpiryMinutes = (int)dateDiff.TotalMinutes;

			}
		}

		private bool _userLocalCosmosDB;

		public bool UseLocalCosmosDB
		{
			get { return _userLocalCosmosDB; }
			set 
			{ 
				SetProperty(ref _userLocalCosmosDB, value);
				App.UseLocalCosmosDB = value;
			}
		}

		private bool _useLocalResourceTokenBroker;

		public bool UseLocalResourceTokenBroker
		{
			get { return _useLocalResourceTokenBroker; }
			set 
			{
				SetProperty(ref _useLocalResourceTokenBroker, value);
				App.UseLocalResourceTokenBroker = value;
			}
		}

		private string _message;

		public string Message
		{
			get { return _message; }
			set { SetProperty(ref _message, value); }
		}

		private string _modelName;

		public string ModelName
		{
			get { return _modelName; }
			set { SetProperty(ref _modelName, value); }
		}

		private async Task<Notification> FetchResourceTokenAsync()
		{
			IsBusy = true;
			Message = null;
			Token = null;

			var response = Notification.Success();

			try
			{
				var request = new CosmosResourceRequest(App.CurrentUserId);
				var logicResponse = await _fetchTokenLogic.ExecuteAsync(request);
				response.AddRange(logicResponse.Notification);

				if (response.IsValid())
				{
					Token = logicResponse.TokenResponse.Token;
					TokenExpiry = logicResponse.TokenResponse.TokenExpiry;
					Repo.UpdateToken(Token);
				}
				else
				{
					Message = response.ToString();
				}
			}
			finally
			{
				IsBusy = false;
			}
			return response;
		}

		private async Task QuickStartLocalDB()
		{
			var cosmosClient = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
				new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
			var container = cosmosClient.GetDatabase("remotimedb").GetContainer("UserData");
			var testRecord = new TestModel { Name = ModelName };
			var item = new CosmosDocument<TestModel>(testRecord);
			ItemResponse<CosmosDocument<TestModel>> response = await container.CreateItemAsync<CosmosDocument<TestModel>>(item, new PartitionKey(item.UserId));
		}

		private async void CreateDBRecord()
		{
			if (string.IsNullOrEmpty(ModelName))
			{
				Message = "Enter a name for the model!";
				return;
			}

			//if (App.UseLocalCosmosDB)
			//{
			//	await QuickStartLocalDB();
			//}

			var testRecord = new TestModel { Name = ModelName };			
			await SaveRecordAsync(testRecord).ConfigureAwait(false);
		}

		private int _record2NumberCounter;

		private async void CreateDBRecord2()
		{
			if (string.IsNullOrEmpty(ModelName))
			{
				Message = "Enter a name for the model!";
				return;
			}

			var newDate = DateTime.Now.AddDays(_record2NumberCounter);
			var testRecord = new TestModel2 { Name = ModelName, Number = _record2NumberCounter, Date = newDate };
			_record2NumberCounter++;

			await SaveRecordAsync(testRecord).ConfigureAwait(false);
		}

		private async Task SaveRecordAsync<T>(T model) where T : ModelBase, new()
		{
			IsBusy = true;

			try
			{
				//make sure we have a valid token to call cosmos with
				await CheckForValidResourceTokenAsync();

				var saveResult = await Repo.SaveModelAsync(model).ConfigureAwait(false);

				if (saveResult.IsValid())
				{
					Message = "SUCCESS!";
				}
				else
				{
					Message = saveResult.ToString();
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task<Notification> CheckForValidResourceTokenAsync()
		{
			var result = Notification.Success();
			if (TokenExpiry.ToUniversalTime() < DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)))
			{
				Debug.WriteLine("The Cosmos Resource Token is about to expire. Let's get a new one....");
				await FetchResourceTokenAsync();
			}

			return result;
		}

	}
}
