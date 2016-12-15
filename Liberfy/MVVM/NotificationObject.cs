using System.ComponentModel;

namespace Liberfy
{
	class NotificationObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetProperty<T>(ref T refVal, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
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

		protected void SetPropertyForce<T>(ref T refValue, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			refValue = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
