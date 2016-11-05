using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

		protected bool SetProperty<T>(ref T refVal, T value, [CallerMemberName] string propertyName = "")
		{
			bool eq = Equals(refVal, value);

			if (eq)
			{
				refVal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}

			return eq;
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public virtual bool CanClose() => true;

		public virtual void Dispose() { }
	}
}
