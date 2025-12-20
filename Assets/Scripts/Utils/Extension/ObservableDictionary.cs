using System.Linq;
using ObservableCollections;

namespace Utils.Extension
{
    public static class ObservableDictionary
    {
        public static void NotifyChanged<TKey, TValue>(this ObservableDictionary<TKey, TValue> dictionary)
        {
            var snapshot = dictionary.ToArray();
            
            foreach (var (key, value) in snapshot)
            {
                dictionary[key] = value;
            }
        }

    }
}