using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;

namespace XamarinCosmosDB
{
	public class ListItemsViewModel : ViewModelBase
	{

		public ListItemsViewModel()
		{
			Models = new ObservableCollection<object>();

			RefreshRecordsCommand = new DelegateCommand(async () => await FetchRecordsAsync());
			DeleteModelCommand = new DelegateCommand<ModelBase>(DeleteModel);
			EditModelCommand = new DelegateCommand<TestModel2>(EditModel);
			CreateDBRecordCommand = new DelegateCommand(CreateDBRecord);
		}

		public ICommand CreateDBRecordCommand { get; }

		public ICommand EditModelCommand { get; }


		public ICommand DeleteModelCommand { get; }

		public ICommand RefreshRecordsCommand { get; }

		private ObservableCollection<object> _models;

		public ObservableCollection<object> Models
		{
			get { return _models; }
			set { SetProperty(ref _models, value); }
		}

		public async Task FetchRecordsAsync()
		{
			IsBusy = true;

			try
			{
				var models = new List<object>();

				var modelCollectionResult = await Repo.FetchModelCollectionAsync<TestModel>();
				if (modelCollectionResult.IsValid())
				{
					models.AddRange(modelCollectionResult.ModelCollection);
				}

				var modelCollection2Result = await Repo.FetchModelCollectionAsync<TestModel2>();
				if (modelCollection2Result.IsValid())
				{
					models.AddRange(modelCollection2Result.ModelCollection);
				}

				Models = new ObservableCollection<object>(models);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void EditModel(TestModel2 model)
		{
			var page = new TestModel2Page(model);
			App.Current.MainPage.Navigation.PushAsync(page);
		}

		private async void CreateDBRecord()
		{
			IsBusy = true;

			try
			{
				var model = new TestModel { Name = "New Record! " };
				await Repo.SaveModelAsync(model);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async void DeleteModel(ModelBase model)
		{
			IsBusy = true;

			try
			{
				Notification deleteResult = Notification.Success();

				if (model.GetType() == typeof(TestModel2))
				{
					deleteResult = await Repo.DeleteModelAsync<TestModel2>(model.Id);
				}
				else
				{
					deleteResult = await Repo.DeleteModelAsync<TestModel>(model.Id);
				}

				if (deleteResult.IsValid())
				{
					//normally we'd just remove the record for the Models collection, but want to make sure DB operation was successful 
					await FetchRecordsAsync();
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

	}
}
