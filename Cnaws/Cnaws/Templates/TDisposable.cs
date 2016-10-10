using System;

namespace Cnaws.Templates
{
    public sealed class TDisposable<T> : IDisposable where T : IDisposable
    {
        private T _instance;

        public TDisposable(T instance)
        {
            _instance = instance;
        }
        ~TDisposable()
        {
            Dispose();
        }

        public T Instance
        {
            get { return _instance; }
        }

        public void Dispose()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _instance = default(T);
            }
        }
    }
}
