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

	public class MainViewModel : BindableBase
	{

		private readonly IFetchCosmosResourceTokenCommand _fetchTokenLogic;
		private readonly ICosmosRepository _cosmosRepository;

		public MainViewModel()
		{
			_fetchTokenLogic = DependencyService.Get<IFetchCosmosResourceTokenCommand>();
			_cosmosRepository = DependencyService.Get<ICosmosRepository>();

			FetchResourceTokenCommand = new DelegateCommand(async () => await FetchResourceTokenAsync());
			CreateDBRecordCommand = new DelegateCommand(CreateDBRecord);

			App.CurrentUserId = "test-user-id";
		}

		private bool _isBusy;

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public ICommand FetchResourceTokenCommand { get; }

		public ICommand CreateDBRecordCommand { get; }

		private string _token;

		public string Token
		{
			get { return _token; }
			set { SetProperty(ref _token, value); }
		}

		private DateTimeOffset _tokenExpiry;

		public DateTimeOffset TokenExpiry
		{
			get { return _tokenExpiry; }
			set { SetProperty(ref _tokenExpiry, value); }
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
				var request = new CosmosResourceTokenRequest(App.CurrentUserId);
				var logicResponse = await _fetchTokenLogic.ExecuteAsync(request);
				response.AddRange(logicResponse.Notification);

				if (response.IsValid())
				{
					Token = logicResponse.TokenResponse.Token;
					TokenExpiry = logicResponse.TokenResponse.TokenExpiry;
					_cosmosRepository.UpdateToken(Token);
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

		private async void CreateDBRecord()
		{
			IsBusy = true;

			try
			{
				//make sure we have a valid token to call cosmos with
				await CheckForValidResourceTokenAsync();

				var testRecord = new TestModel { Name = ModelName };

				var saveResult = await _cosmosRepository.SaveModelAsync(testRecord);

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
