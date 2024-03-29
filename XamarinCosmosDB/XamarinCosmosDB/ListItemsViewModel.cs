﻿using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;
using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public class ListItemsViewModel : ViewModelBase
	{

		private readonly IDeleteAllCosmosDataCommand _deleteAllDataLogic;
		public ListItemsViewModel()
		{
			_deleteAllDataLogic = DependencyService.Get<IDeleteAllCosmosDataCommand>();

			Models = new ObservableCollection<object>();

			FilterDate = DateTime.Now;

			RefreshRecordsCommand = new DelegateCommand(async () => await FetchRecordsAsync());
			DeleteModelCommand = new DelegateCommand<ModelBase>(DeleteModel);
			EditModelCommand = new DelegateCommand<TestModel2>(EditModel);
			CreateDBRecordCommand = new DelegateCommand(CreateDBRecord);
			DeleteAllDataCommand = new DelegateCommand(async () => await DeleteAllDataAsync());
		}

		public ICommand CreateDBRecordCommand { get; }

		public ICommand EditModelCommand { get; }

		public ICommand DeleteModelCommand { get; }

		public ICommand RefreshRecordsCommand { get; }

		public ICommand DeleteAllDataCommand { get; }

		private ObservableCollection<object> _models;

		public ObservableCollection<object> Models
		{
			get { return _models; }
			set { SetProperty(ref _models, value); }
		}

		private DateTime _filterDate;

		public DateTime FilterDate
		{
			get { return _filterDate; }
			set { SetProperty(ref _filterDate, value); }
		}

		private bool _filterByDate;

		public bool FilterByDate
		{
			get { return _filterByDate; }
			set { SetProperty(ref _filterByDate, value); }
		}

		public async Task FetchRecordsAsync()
		{
			IsBusy = true;

			try
			{
				var models = new List<object>();

				if (FilterByDate)
				{
					var modelCollection2Result = await Repo.FetchTestModel2ByDateAsync(FilterDate);
					if (modelCollection2Result.IsValid())
					{
						models.AddRange(modelCollection2Result.ModelCollection);
					}
				}
				else
				{
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

		private async Task DeleteAllDataAsync()
		{
			IsBusy = true;

			try
			{
				var request = new CosmosResourceRequest(App.CurrentUserId);
				var deleteResult = await _deleteAllDataLogic.ExecuteAsync(request);
				if (deleteResult.IsValid())
				{
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
