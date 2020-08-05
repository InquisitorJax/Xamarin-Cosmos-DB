using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinCosmosDB
{

	public class MainViewModel : BindableBase
	{

		private readonly IFetchCosmosResourceTokenCommand _fetchTokenLogic;

		public MainViewModel()
		{
			_fetchTokenLogic = DependencyService.Get<IFetchCosmosResourceTokenCommand>();
			FetchResourceTokenCommand = new DelegateCommand(FetchResourceToken);

			UserId = "test-user-id";
		}

		private async void FetchResourceToken()
		{
			IsBusy = true;
			ErrorMessage = null;
			Token = null;

			try
			{
				var request = new CosmosResourceTokenRequest(UserId);
				var response = await _fetchTokenLogic.ExecuteAsync(request);
				if (response.IsValid())
				{
					Token = response.TokenResponse.Token;
				}
				else
				{
					ErrorMessage = response.Notification.ToString();
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		private bool _isBusy;

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public ICommand FetchResourceTokenCommand { get; }

		private string _userId;

		public string UserId
		{
			get { return _userId; }
			set { SetProperty(ref _userId, value); }
		}

		private string _token;

		public string Token
		{
			get { return _token; }
			set { SetProperty(ref _token, value); }
		}

		private string _errorMessage;

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { SetProperty(ref _errorMessage, value); }
		}

	}
}
