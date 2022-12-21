namespace algLab_6
{
    /// <summary> Хеш-таблица, разрешение коллизий методом открытой адресации </summary>
    /// <typeparam name="TKey"> Тип ключа </typeparam>
    /// <typeparam name="TValue"> Тип значения </typeparam>
    public class Hashtable<TKey, TValue>
    {
        /// <summary> Размер хеш-таблицы </summary>
        private readonly int _size;

        /// <summary> Элементы хеш-таблицы </summary>
        private readonly KeyValuePair<TKey, TValue?>[] _items;

        /// <summary> Класс хеш-функции </summary>
        private readonly HashProbingType _hashProbingType;

        /// <summary> Метод хеширования </summary>
        private readonly HashMethodType[] _hashMethodType;

        /// <summary> Хеш-коды удалённых элементов</summary>
        private readonly bool[] _removed;

        /// <summary> Функция линейного хеширования </summary>
        private static readonly Func<Func<object, int, int>, object, int, int, int> LinearHashing =
            (f, key, sizeHashTable, index) => (f(key, sizeHashTable) + index) % sizeHashTable;

        /// <summary> Функция квадратичного хеширования </summary>
        private static readonly Func<Func<object, int, int>, object, int, int, int> QuadraticHashing =
            (f, key, sizeHashTable, index) => (f(key, sizeHashTable) + (int)Math.Pow(index, 2)) % sizeHashTable;

        /// <summary> Функция двойного хеширования</summary>
        private static readonly Func<Func<object, int, int>, Func<object, int, int>, object, int, int, int> DoubleHashing =
            (f1, f2, key, sizeHashTable, index) => (f1(key, sizeHashTable) + index * f2(key, sizeHashTable)) % sizeHashTable;

        /// <summary> Количество элементов в хеш-таблице </summary>
        public int Count { get; private set; }

        /// <summary> Получить длину самого длинного кластера в таблице </summary>
        public int MaxClusterLength
        {
            get
            {
                var max = 0;
                var current = 0;
                foreach (var item in _items)
                {
                    if (!item.Equals(default(KeyValuePair<TKey, TValue>)))
                    {
                        current++;
                    }
                    else
                    {
                        max = Math.Max(max, current);
                        current = 0;
                    }
                }

                return Math.Max(max, current);
            }
        }

        /// <summary> Создать хеш-таблицу </summary>
        /// <param name="size"> Размер хеш-таблицы </param>
        /// <param name="hashProbingType"> Тип класса используемой хеш-функции </param>
        public Hashtable(int size, HashProbingType hashProbingType, params HashMethodType[] hashMethodType)
        {
            if (!IsSizeCorrect(size)) throw new AggregateException(nameof(size));
            _size = size;
            _hashProbingType = hashProbingType;

            if (hashMethodType.Length > 1) _hashMethodType = hashMethodType;
            else _hashMethodType = new[] { hashMethodType[0], HashMethodType.Div };

            _items = new KeyValuePair<TKey, TValue?>[size];
            _removed = new bool[_size];
        }

        /// <summary> Создать хеш-таблицу </summary>
        /// <param name="size"> Размер хеш-таблицы </param>
        public Hashtable(int size)
        {
            if (!IsSizeCorrect(size)) throw new AggregateException(nameof(size));
            _size = size;
            _hashProbingType = HashProbingType.Linear;
            _hashMethodType = new[] { HashMethodType.Div, HashMethodType.Multi };
            _items = new KeyValuePair<TKey, TValue?>[size];
            _removed = new bool[_size];
        }

        /// <summary> Создать хеш-таблицу </summary>
        public Hashtable()
        {
            _size = 1000;
            _hashProbingType = HashProbingType.Linear;
            _hashMethodType = new[] { HashMethodType.Div, HashMethodType.Multi };
            _items = new KeyValuePair<TKey, TValue?>[_size];
            _removed = new bool[_size];
        }

        /// <summary> Проверка пропусков в хеш-таблице </summary>
        protected bool CheckOpenSpace()
        {
            var isOpen = false;
            for (var i = 0; i < _size; i++)
            {
                if (_items[i].Equals(default(KeyValuePair<TKey, TValue?>))) isOpen = true;
            }

            return isOpen;
        }

        /// <summary> Проверить корректен ли размер хеш-таблицы </summary>
        /// <param name="size"> Размер </param>
        protected bool IsSizeCorrect(int size)
        {
            return size > 0;
        }

        /// <summary> Проверить, что ключ уникален </summary>
        /// <param name="key"> Ключ </param>
        protected bool CheckUniqueKey(TKey key)
        {
            foreach (var item in _items)
            {
                if (item.Key != null && item.Key.Equals(key)) return false;
            }

            return true;
        }

        /// <summary> Добавить элемент в хеш-таблицу </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="value"> Значение </param>
        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (!CheckOpenSpace()) throw new ArgumentOutOfRangeException("Хеш-таблица переполнена.");

            if (!CheckUniqueKey(key)) throw new ArgumentException("Элемент по указанному ключу уже существует.");

            Insert(key, value);
        }

        /// <summary> Выполнить вставку значения по ключу в хеш-таблицу </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="value"> Значение </param>
        protected void Insert(TKey key, TValue value)
        {
            var index = 0;
            var hashCode = GetHash(key, index);

            while (!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) && !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = GetHash(key, index);
            }

            _items[hashCode] = new KeyValuePair<TKey, TValue?>(key, value);
            _removed[hashCode] = false;
            Count++;
        }

        /// <summary> Получить хеш-код по заданному ключу и индексу в соответствии с параметрами хеш-таблицы </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="index"> Индекс </param>
        protected int GetHash(TKey key, int index)
        {
            return _hashProbingType switch
            {
                HashProbingType.Linear =>
                    LinearHashing(GetHashMethod(_hashMethodType[0]), key, _size, index),
                HashProbingType.Quadratic => QuadraticHashing(GetHashMethod(_hashMethodType[0]), key, _size,
                    index),
                HashProbingType.Double => DoubleHashing(GetHashMethod(_hashMethodType[0]),
                    GetHashMethod(_hashMethodType[1]), key, _size, index),
                _ => LinearHashing(GetHashMethod(_hashMethodType[0]), key, _size, index)
            };
        }

        /// <summary> Получить метод хеширования в соответствии с типом </summary>
        /// <param name="hashMethodType"> Тип метода хеширования </param>
        protected static Func<object, int, int> GetHashMethod(HashMethodType hashMethodType)
        {
            return hashMethodType switch
            {
                HashMethodType.Div => HashFunctionsExtensions.GetHashCodeDivMethod,
                HashMethodType.Multi => HashFunctionsExtensions.GetHashCodeMultiMethod,
                HashMethodType.Md5 => HashFunctionsExtensions.GetHashCodeHMACMD5,
                HashMethodType.Sha256 => HashFunctionsExtensions.GetHashCodeSHA256,
                HashMethodType.Fnv => HashFunctionsExtensions.GetHashCodeFNV,
                _ => HashFunctionsExtensions.GetHashCodeDivMethod
            };
        }

        /// <summary> Получить значение по ключу </summary>
        /// <param name="key"> Ключ </param>
        public TValue? GetValue(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var index = 0;
            var hashCode = GetHash(key, index);

            while ((!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) || _removed[hashCode]) && !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = GetHash(key, index);
            }

            return _items[hashCode].Value;
        }

        /// <summary> Удалить элемент из Хеш-таблицы по ключу </summary>
        /// <param name="key"> Ключ </param>
        public bool Remove(TKey key)
        {
            var index = 0;
            var hashCode = GetHash(key, index);

            while ((!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) || _removed[hashCode]) && !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = GetHash(key, index);
            }

            if (_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)))
            {
                return false;
            }
            else
            {
                _items[hashCode] = default;
                _removed[hashCode] = true;
                Count--;
                return true;
            }
        }
    }
}
