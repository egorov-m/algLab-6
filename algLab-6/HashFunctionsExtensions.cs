namespace algLab_6
{
    public static class HashFunctionsExtensions
    {
        public static double GoldenRatioConst { get; } = (Math.Sqrt(5) - 1) / 2;

        /// <summary> Получить хеш-код методом деления </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="sizeHashTable"> Размер Хеш-таблицы </param>
        public static int GetHashCodeDivMethod(this object key, int sizeHashTable) => Math.Abs(key.GetHashCode() % sizeHashTable);

        /// <summary> Получить хеш-код методом умножения </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="sizeHashTable"> Размер Хеш-таблицы </param>
        public static int GetHashCodeMultiMethod(this object key, int sizeHashTable)
        {
            return (int) (sizeHashTable * (key.GetHashCode() * GoldenRatioConst % 1));
        }
    }
}
