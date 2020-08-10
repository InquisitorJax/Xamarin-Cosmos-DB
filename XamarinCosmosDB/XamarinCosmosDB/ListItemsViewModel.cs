using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public class ListItemsViewModel : BindableBase
	{
		private readonly ICosmosRepository _cosmosRepository;

		public ListItemsViewModel()
		{
			_cosmosRepository = DependencyService.Get<ICosmosRepository>();
			Models = new ObservableCollection<object>();

			RefreshRecordsCommand = new DelegateCommand(async () => await FetchRecordsAsync());
			DeleteModelCommand = new DelegateCommand<ModelBase>(DeleteModel);
			CreateDBRecordCommand = new DelegateCommand(CreateDBRecord);
		}

		public ICommand CreateDBRecordCommand { get; }

		private async void CreateDBRecord()
		{
			IsBusy = true;

			try
			{
				var model = new TestModel { Name = "New Record! " };
				await _cosmosRepository.SaveModelAsync(model);
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
					deleteResult = await _cosmosRepository.DeleteModelAsync<TestModel2>(model.Id);
				}
				else
				{
					deleteResult = await _cosmosRepository.DeleteModelAsync<TestModel>(model.Id);
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

		public ICommand DeleteModelCommand { get; }

		public ICommand RefreshRecordsCommand { get; }

		private ObservableCollection<object> _models;

		public ObservableCollection<object> Models
		{
			get { return _models; }
			set { SetProperty(ref _models, value); }
		}

		private bool _isBusy;

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public async Task FetchRecordsAsync()
		{
			IsBusy = true;

			try
			{
				var models = new List<object>();

				var modelCollectionResult = await _cosmosRepository.FetchModelCollectionAsync<TestModel>();
				if (modelCollectionResult.IsValid())
				{
					models.AddRange(modelCollectionResult.ModelCollection);
				}

				var modelCollection2Result = await _cosmosRepository.FetchModelCollectionAsync<TestModel2>();
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
	}
}
