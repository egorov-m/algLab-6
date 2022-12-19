using System.Collections;

namespace algLab_6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var hashMap = new HashMap();

            // Добавляем данные в хеш таблицу в виде пар Ключ-Значение.
            //hashMap.Add("Little Prince", "I never wished you any sort of harm; but you wanted me to tame you...");
            //hashMap.Add("Fox", "And now here is my secret, a very simple secret: It is only with the heart that one can see rightly; what is essential is invisible to the eye.");
            //hashMap.Add("Rose", "Well, I must endure the presence of two or three caterpillars if I wish to become acquainted with the butterflies.");
            //hashMap.Add("King", "He did not know how the world is simplified for kings. To them, all men are subjects.");

            hashMap.Add("1", "Kate");
            hashMap.Add("1", "Karolina");
            hashMap.Add("2", "Peter");
            hashMap.Add("3", "Misha");



            // Выводим хранимые значения на экран.
            ShowHashTable(hashMap, "Created hashtable.");
            Console.ReadLine();

            // Удаляем элемент из хеш таблицы по ключу
            // и выводим измененную таблицу на экран.
            //hashMap.Delete("King");
            //ShowHashTable(hashMap, "Delete item from hashtable.");
            Console.ReadLine();

            // Получаем хранимое значение из таблицы по ключу.
            //Console.WriteLine("Little Prince say:");
            //var text = hashMap.Search("Little Prince");
            //Console.WriteLine(text);
            Console.ReadLine();
        }


        private static void ShowHashTable(HashMap hashMap, string title)
        {
            // Проверяем входные аргументы.
            if (hashMap == null)
            {
                throw new ArgumentNullException(nameof(hashMap));
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            // Выводим все имеющие пары хеш-значение
            Console.WriteLine(title);
            foreach (var item in hashMap.Items)
            {
                // Выводим хеш
                Console.WriteLine(item.Key);

                // Выводим все значения хранимые под этим хешем.
                foreach (var value in item.Value)
                {
                    Console.WriteLine($"\t{value.Key} - {value.Value}");
                }
            }
            Console.WriteLine();
        }
    }
}