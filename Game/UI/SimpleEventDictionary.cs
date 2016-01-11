using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.UI
{
    class SimpleEventDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        Dictionary<TKey, TValue> values = new Dictionary<TKey, TValue>();
        public Action OnChange { get; set; }

        public TValue this[TKey k] {
            get {
                return values[k];
            }
        }

        public void addMarkedSquare(TKey k, TValue v)
        {
            values[k] = v;
            OnChange();
        }

        public void replace(List<TKey> kList, TValue v)
        {
            values.Clear();
            foreach (TKey k in kList)
            {
                values[k] = v;
            }
            OnChange();
        }

        public void removeMarkedSquare(TKey k)
        {
            try
            {
                values.Remove(k);
                OnChange();
            }
            catch (Exception)
            {
                // Not really an issue if someone attempted to remove something wasn't in the dictionary
            }
        }

        public void clearMarkedSquares()
        {
            values.Clear();
            OnChange();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
