using Microsoft.Xrm.Sdk;

namespace Tests.Common.Utils
{
    public class GenericParameterCollection<TKey, TValue>
    {
        public ParameterCollection Collection { get; } = new ParameterCollection();

        public void Add(TKey key, TValue value) => Collection.Add(key.ToString(), value);

        public TValue this[TKey key]
        {
            get => (TValue) Collection[key.ToString()];
            set => Collection[key.ToString()] = value;
        }

        public bool Remove(TKey key) => Collection.Remove(key.ToString());

        public void Clear() => Collection.Clear();
    }
}
