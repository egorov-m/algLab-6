using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algLab_6
{
    class GenericHashMap<TKey, TValue>
    {
        private const int PRIMEVALUE = 97;
        TValue[] theValues;

        public GenericHashMap()
        {
            theValues = new TValue[PRIMEVALUE];
        }

        public TValue Get(TKey key)
        {
            int indexForValues = hashFunction(key);
            return theValues[indexForValues];
        }

        public void Put(TKey key, TValue value)
        {
            int indexForVlaues = hashFunction(key);
            theValues[indexForVlaues] = value;
        }

        private int hashFunction(TKey key)
        {
            return key.GetHashCode() % PRIMEVALUE;
        }
    }
}
