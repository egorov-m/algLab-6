using System.Text;

namespace algLab_6
{
    class DictionaryTester
    {
        /// <summary> Генерировать тестовые ключи и значения для хеш-таблицы </summary>
        public static (List<string>, List<string>) GeneratingValuesAndKeys(int sizeHash = 100000)
        {
            var start = Enumerable.Range(0, sizeHash).ToArray();
            var randomKeys = new Random();
            var randomValues = new Random();

            var keys = new List<string>();
            var values = new List<string>();

            //Генерация ключей
            for (var i = 0; i < sizeHash; i++)
                keys.Add($"{i}{start[i]}{randomKeys.Next()}");

            //Генерация значений
            for (var j = 0; j < sizeHash; j++)
            {
                var s = "";
                for (var i = 0; i < 5; i++)
                {
                    var a = (char)randomValues.Next(0, 255);
                    s += a;

                }
                values.Add(s);
            }

            return (keys, values);
        }

        public static void Testing(HashProbingType hashProbingType, params HashMethodType[] hashMethodType)
        {
            //var dict = new Hashtable<string, string>(100000, hashMethodType);
            var ht = new Hashtable<string, string>(10000, hashProbingType, hashMethodType[0], hashMethodType[1]);

            Console.WriteLine("Генерация данных начата.");
            var (keys, values) = GeneratingValuesAndKeys(2500);

            Console.WriteLine("Начинаем добавлять данные в хеш-таблицу.");
            for (var i = 0; i < keys.Count; i++)
            {
                ht.Add(keys[i], values[i]);
            }

            //Console.WriteLine($"Коэффициент заполнения равен: {dict.FillFactor}.");
            //Console.WriteLine($"Максимальная длинна цепочки: {dict.MaxLengthChain}.");
            //Console.WriteLine($"Минимальная длинна цепочки: {dict.MinLengthChain}.");
            //var lengths = dict.LengthsChains;

            Console.WriteLine(ht.MaxClusterLength);

            //using var writer = new StreamWriter($"{hashMethodType:G}.csv", append: false, Encoding.UTF8);
            //writer.AutoFlush = true;

            //var index = 0;
            //foreach (var length in lengths)
            //{
            //    writer.WriteLine($"{index};{length}");
            //    index++;
            //}
        }
    }
}
