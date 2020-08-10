using Prism.Commands;
using System.Windows.Input;

namespace XamarinCosmosDB
{
	public class TestModel2ViewModel : ViewModelBase
	{

		public TestModel2ViewModel()
		{
			SaveCommand = new DelegateCommand(SaveModel);
		}

		public ICommand SaveCommand { get;  }

		private TestModel2 _model;

		public TestModel2 Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}

		private async void SaveModel()
		{
			IsBusy = true;

			try
			{
				await Repo.SaveModelAsync(Model);

				await App.Current.MainPage.Navigation.PopAsync();
			}
			finally
			{
				IsBusy = false;
			}
		}
	}

}
