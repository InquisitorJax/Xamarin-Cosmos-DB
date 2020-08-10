using Prism.Mvvm;
using Xamarin.Forms;

namespace XamarinCosmosDB
{
	public class ViewModelBase : BindableBase
	{
		public ViewModelBase()
		{
			Repo = DependencyService.Get<ICosmosRepository>();
		}

		protected ICosmosRepository Repo { get; private set; }

		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

	}
}
