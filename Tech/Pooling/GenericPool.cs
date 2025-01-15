using System.Collections.Generic;

namespace Tech.Pooling
{
    public class GenericPool<T> : IPool<T> where T : class, new()
    {
        private readonly Stack<T> _pools = new();
        
        public T Get() => _pools.Count > 0 ? _pools.Pop() : new T();

        public void Return(T item)
        {
            if(_pools.Contains(item) || item == null) return;
            
            _pools.Push(item);
        }
        
        public void Clear() => _pools.Clear();

        public void PrePool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _pools.Push(new T());
            }
        }
    }
}