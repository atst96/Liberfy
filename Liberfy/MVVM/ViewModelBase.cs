using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Liberfy
{
	class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		public ViewModelBase() : base()
		{
			DialogService = new DialogService(this);
		}

		public DialogService DialogService { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetProperty<T>(ref T refVal, T value, [CallerMemberName] string propertyName = "")
		{
			if (!Equals(refVal, value))
			{
				refVal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

				return true;
			}
			else
				return false;
		}

		protected bool SetProperty<T>(ref T refVal, T value, Command command, [CallerMemberName] string propertyName = "")
		{
			if (!Equals(refVal, value))
			{
				refVal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

				command?.RaiseCanExecute();

				return true;
			}
			else
				return false;
		}

		protected void SetPropertyForce<T>(ref T refValue, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			refValue = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		internal virtual void OnInitialized() { }

		public virtual bool CanClose() => true;

		public virtual void Dispose() { }
	}
}
