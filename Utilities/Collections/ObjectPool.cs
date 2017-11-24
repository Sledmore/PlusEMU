using Plus.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Utilities.Collections
{
    public sealed class ObjectPool<T>
    {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) { Logging.LogCriticalException("ObjectGenerator was null"); }
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public T GetObject()
        {
            T Item;
            if (_objects.TryTake(out Item)) { return Item; }
            return _objectGenerator();
        }

        public void PutObject(T Item)
        {
            _objects.Add(Item);
        }
    }
}
