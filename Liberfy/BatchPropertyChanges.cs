using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class BatchPropertyChanges
    {
        private HashSet<string> _changedPropertyNames;

        public int Threshold { get; } = 1;

        public BatchPropertyChanges()
        {
            this._changedPropertyNames = new HashSet<string>();
        }

        public BatchPropertyChanges(int threshold)
        {
            this.Threshold = threshold;
        }

        public bool Set<T>(ref T destValue, T newValue, string propertyName)
        {
            var comparer = EqualityComparer<T>.Default;
            if (!comparer.Equals(destValue, newValue))
            {
                destValue = newValue;
                this.Raise(propertyName);

                return true;
            }

            return false;
        }

        public void Raise(string propertyName)
        {
            if (!this._changedPropertyNames.Contains(propertyName))
            {
                this._changedPropertyNames.Add(propertyName);
            }
        }

        public void Clear()
        {
            this._changedPropertyNames.Clear();
        }

        public void Execute(Action<string> raisePropertyChangedAction)
        {
            int changedPropertiesCount = this._changedPropertyNames.Count;
            if (changedPropertiesCount <= 0 || raisePropertyChangedAction == null)
            {
                return;
            }
            else if (changedPropertiesCount <= this.Threshold)
            {
                foreach (var propertyName in this._changedPropertyNames.AsParallel())
                {
                    raisePropertyChangedAction.Invoke(propertyName);
                }
            }
            else
            {
                raisePropertyChangedAction.Invoke(string.Empty);
            }
        }

        ~BatchPropertyChanges()
        {
            this.Clear();
        }
    }
}
