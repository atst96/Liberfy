using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Liberfy
{
    [Serializable]
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static bool IsEquals<T>(ref T oldValue, ref T newValue)
        {
            return EqualityComparer<T>.Default.Equals(oldValue, newValue);
        }

        protected bool SetProperty<T>(ref T storage, T value, PropertyChangedEventArgs e)
        {
            if (!NotificationObject.IsEquals(ref storage, ref value))
            {
                storage = value;
                this.RaisePropertyChanged(e);

                return true;
            }

            return false;
        }

        protected bool SetProperty<T>(ref T storage, ref T value, PropertyChangedEventArgs e)
        {
            if (!NotificationObject.IsEquals(ref storage, ref value))
            {
                storage = value;
                this.RaisePropertyChanged(e);

                return true;
            }

            return false;
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (!NotificationObject.IsEquals(ref storage, ref value))
            {
                storage = value;
                this.RaisePropertyChanged(propertyName);

                return true;
            }

            return false;
        }

        protected bool SetProperty<T>(ref T storage, ref T value, [CallerMemberName] string propertyName = "")
        {
            if (!NotificationObject.IsEquals(ref storage, ref value))
            {
                storage = value;
                this.RaisePropertyChanged(propertyName);

                return true;
            }

            return false;
        }

        protected void SetPropertyForce<T>(ref T storage, T value, PropertyChangedEventArgs e)
        {
            storage = value;
            this.RaisePropertyChanged(e);
        }

        protected void SetPropertyForce<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            storage = value;
            this.RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}
