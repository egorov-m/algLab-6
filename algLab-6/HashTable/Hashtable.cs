namespace algLab_6.HashTable
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

        /// <summary> Количество элементов в хеш-таблице </summary>
        public int Count { get; private set; }

        /// <summary> Создать хеш-таблицу </summary>
        /// <param name="size"> Размер хеш-таблицы </param>
        public Hashtable(int size)
        {
            if (!IsSizeCorrect(size)) throw new AggregateException(nameof(size));
            _size = size;
            _items = new KeyValuePair<TKey, TValue?>[size];
        }

        /// <summary> Создать хеш-таблицу </summary>
        public Hashtable()
        {
            _size = 1000;
            _items = new KeyValuePair<TKey, TValue?>[_size];
        }

        /// <summary> Проверка пропусков в хеш-таблице </summary>
        protected bool CheckOpenSpace()
        {
            var isOpen = false;
            for (var i = 0; i < _size; i++)
            {
                if (_items[i].Equals(default (KeyValuePair<TKey, TValue?>))) isOpen = true;
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
            var hashCode = (key.GetHashCodeDivMethod(_size) + index) % _size; //0; // Метод вычисления хеша GetHash(key, size, index);

            while (!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) && !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = (key.GetHashCodeDivMethod(_size) + index) % _size; // Метод вычисления хеша GetHash(key, size, index);
            }

            _items[hashCode] = new KeyValuePair<TKey, TValue?>(key, value);
            Count++;
        }

        /// <summary> Получить значение по ключу </summary>
        /// <param name="key"> Ключ </param>
        public TValue? GetValue(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var index = 0;
            var hashCode = (key.GetHashCodeDivMethod(_size) + index) % _size; //0; // Метод вычисления хеша GetHash(key, size, index);

            while (!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) && !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = (key.GetHashCodeDivMethod(_size) + index) % _size; // Метод вычисления хеша GetHash(key, size, index);
            }

            return _items[hashCode].Value;
        }

        /// <summary> Удалить элемент из Хеш-таблицы по ключу </summary>
        /// <param name="key"> Ключ </param>
        public bool Remove(TKey key)
        {
            var index = 0;
            var hashCode = (key.GetHashCodeDivMethod(_size) + index) % _size; // Метод вычисления хеша GetHash(key, size, index);

            while (!_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)) && !_items[hashCode].Key.Equals(key))
            {
                index++;
                hashCode = (key.GetHashCodeDivMethod(_size) + index) % _size; // Метод вычисления хеша GetHash(key, size, index);
            }

            if (_items[hashCode].Equals(default(KeyValuePair<TKey, TValue>)))
            {
                return false;
            }
            else
            {
                _items[hashCode] = default;
                Count--;
                return true;
            }
        }
    }
}
