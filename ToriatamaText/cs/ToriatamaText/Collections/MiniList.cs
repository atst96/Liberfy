using System;
using System.Runtime.CompilerServices;

namespace ToriatamaText.Collections
{
    public struct MiniList<T>
    {
        public T[] InnerArray { get; set; }

        public int Count { get; set; }

        public void Clear()
        {
            this.Count = 0;
        }

        public void SetCapacity(int capacity)
        {
            if (this.InnerArray == null || this.InnerArray.Length < capacity)
            {
                var newArray = new T[capacity];

                if (this.Count > 0)
                    Array.Copy(this.InnerArray, newArray, this.Count);

                this.InnerArray = newArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int additionalCapacity)
        {
            var minCapacity = this.Count + additionalCapacity;
            if (this.InnerArray == null)
            {
                this.InnerArray = new T[Math.Max(4, minCapacity)];
            }
            else if (this.InnerArray.Length < minCapacity)
            {
                var newArray = new T[Math.Max(this.Count * 2, minCapacity)];
                Array.Copy(this.InnerArray, newArray, this.Count);
                this.InnerArray = newArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T value)
        {
            this.EnsureCapacity(1);
            this.InnerArray[this.Count++] = value;
        }

        public T this[int index]
        {
            get
            {
                return this.InnerArray[index];
            }
            set
            {
                this.InnerArray[index] = value;
            }
        }

        public T Last => this.InnerArray[this.Count - 1];
    }
}
