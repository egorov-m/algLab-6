using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algLab_6
{
    class GenerateDictionary
    {
        public void GeneratingValuesandKeys()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            const int sizeHash = 10000;
            int[] start = Enumerable.Range(0, sizeHash).ToArray();
            Random randomKeys = new Random();
            Random randomValues = new Random();

            List<int> keys = new List<int>();
            List<string> values = new List<string>();

            //Генерация ключей
            for (int i = 0; i < sizeHash; i++)
                keys.Add(start[i] + randomKeys.Next() + i);



            //Генерация значений
            for (int j = 0; j < sizeHash; j++)
            {
                string s = "";
                for (int i = 0; i < 5; i++)
                {
                    char a = (char)randomValues.Next(0, 255);
                    s += a;

                }
                values.Add(s);
            }

            dict.Add(keys.ToArray(), values.ToArray());

        }
    }
}
