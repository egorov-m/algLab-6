using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algLab_6
{
    class HashMap
    {
        //Сделайте простое значение для использования в хеш-функции и в качестве размера массива.
        private const int PRIMEVALUE = 97;

        //Определите несколько массивов.Один для ключей и один для значений
        int[] theKeys;
        string[] theValues;

        public HashMap()
        {
            theKeys = new int[PRIMEVALUE];
            theValues = new string[PRIMEVALUE];
        }

        public override string ToString()
        {
            string returnString = "Key            => Value\n";
            returnString += "----------------------\n";
            for (int i = 0; i < theKeys.Length; i++)
            {
                if (theKeys[i] != 0)
                    returnString += theKeys[i] + "  => " + theValues[i] + "\n";
            }
            return returnString;
        }

        public string ShowArrays()
        {
            string returnString = "Idx - Key   => Value\n";
            returnString += "------------------------\n";
            for (int i = 0; i < theKeys.Length; i++)
            {
                returnString += "[" + i + "] - " + theKeys[i] + " => " + theValues[i] + "\n";
            }
            return returnString;
        }

        public string Get(int Key)
        {
            //Получить индекс для использования для поиска значения
            int indexForValues = hashFunction(Key);
            //вернуть информацию в местоположении массива indexForValues
            return theValues[indexForValues];
        }

        public void Put(int Key, string Value)
        {
            //Получить индекс для хранения информации из хеш-функции
            int indexForValue = hashFunction(Key);
            //используйте индекс, чтобы поместить ключ и значение в связанный массив.
            theKeys[indexForValue] = Key;
            theValues[indexForValue] = Value;
        }

        //Хеш-функция для получения ключа и сопоставления его с меньшим числовым пространством, находящимся между 0 и ПРИМЗНАЧЕНИЕМ.Это достигается функцией MOD.
        private int hashFunction(int key)
        {
            return key % PRIMEVALUE;
        }
    }
}
