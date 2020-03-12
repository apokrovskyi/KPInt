using System;

namespace KPInt_Shared
{
    public class LockedValue<T>
    {
        private T _value;

        public readonly object _locker = new object();

        public V Retrieve<V>(Func<T, V> func)
        {
            if (func == null) throw new ArgumentNullException();

            lock (_locker)
            {
                return func.Invoke(_value);
            }
        }

        public void Act(Action<T> action)
        {
            Retrieve(x =>
            {
                action.Invoke(x);
                return "";
            });
        }

        public void SetValue(T newValue)
        {
            lock (_locker)
            {
                _value = newValue;
            }
        }

        public LockedValue(T value)
        {
            _value = value;
        }
    }
}
