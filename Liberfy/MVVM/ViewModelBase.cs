using System;
using System.ComponentModel;

namespace Liberfy
{
	class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		public ViewModelBase() : base()
		{
			DialogService = new Liberfy.DialogService(this);
		}

		public DialogService DialogService { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetProperty<T>(ref T refVal, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			bool eq = Equals(refVal, value);

			if (eq)
			{
				refVal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}

			return eq;
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
