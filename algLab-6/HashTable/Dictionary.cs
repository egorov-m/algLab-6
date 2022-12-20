using System.Collections;

namespace algLab_6.HashTable
{
    /// <summary> Словарь: хеш-таблица, разрешение коллизий методом цепочек </summary>
    /// <typeparam name="TKey"> Тип ключа </typeparam>
    /// <typeparam name="TValue"> Тип значения </typeparam>
    public class Dictionary<TKey, TValue> : IDictionary<TKey,TValue>, 
                                            IDictionary, 
                                            IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary> Размер хеш-таблицы </summary>
        private readonly int _size;

        /// <summary> Получить коэффициент заполнения </summary>
        public double FillFactor => (double) Count / _size;

        /// <summary> Получить максимальную длину цепочки для хеш-таблицы </summary>
        public int MaxLengthChain => _items.Where(x => x != null).Max(x => x.Count);

        /// <summary> Получить минимальную длину цепочки для хеш-таблицы </summary>
        public int MinLengthChain => _items.Where(x => x != null).Min(x => x.Count);

        /// <summary> Получить длины цепочек для хеш-таблицы </summary>
        public IEnumerable<int> LengthsChains => _items.Where(x => x != null).Select(x => x.Count);

        /// <summary> Количество элементов в словаре </summary>
        public int Count { get; private set; }

        /// <summary> Метод хеширования </summary>
        private readonly HashMethodType _hashMethodType;

        /// <summary> Элементы хеш-таблицы </summary>
        private readonly LinkedList<KeyValuePair<TKey, TValue?>>?[] _items;

        private object? _syncRoot;

        /// <summary> Создать словарь </summary>
        /// <param name="size"> Размер хеш-таблицы </param>
        /// <param name="hashMethodType"> Метод Хеширования </param>
        public Dictionary(int size, HashMethodType hashMethodType)
        {
            if (!IsSizeCorrect(size)) throw new AggregateException(nameof(size));
            _size = size;
            _hashMethodType = hashMethodType;
            _items = new LinkedList<KeyValuePair<TKey, TValue?>>[size];
        }

        /// <summary> Создать словарь </summary>
        /// <param name="size"> Размер хеш-таблицы </param>
        public Dictionary(int size)
        {
            if (!IsSizeCorrect(size)) throw new AggregateException(nameof(size));
            _size = size;
            _hashMethodType = HashMethodType.Div;
            _items = new LinkedList<KeyValuePair<TKey, TValue?>>[size];
        }

        /// <summary> Создать словарь </summary>
        public Dictionary()
        {
            _size = 1000;
            _hashMethodType = HashMethodType.Div;
            _items = new LinkedList<KeyValuePair<TKey, TValue?>>[_size];
        }

        /// <summary> Добавить элемент в словарь </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="value"> Значение </param>
        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var item = new KeyValuePair<TKey, TValue?>(key, value);
            Insert(item);
        }

        /// <summary> Выполнить вставку элемента в словарь (гарантия уникальности ключей) </summary>
        /// <param name="item"> Элемент для вставки </param>
        protected void Insert(KeyValuePair<TKey, TValue?> item)
        {
            if (item.Key == null) throw new ArgumentNullException(nameof(item.Key));
            var position = GetListPosition(item.Key);
            var linkedList = GetLinkedList(position);

            foreach (var pair in linkedList)
            {
                if (pair.Key != null && pair.Key.Equals(item.Key)) 
                    throw new ArgumentException("Элемент по указанному ключу уже существует.");
            }

            linkedList.AddLast(item);
            Count++;
        }

        /// <summary> Проверить корректен ли размер хеш-таблицы </summary>
        /// <param name="size"> Размер </param>
        protected bool IsSizeCorrect(int size)
        {
            return size > 0;
        }

        /// <summary> Установить значение по ключу </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="value"> Новое значение </param>
        public void SetValue(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);

            var foundItem = false;
            foreach (var item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key))
                {
                    foundItem = true;
                    if (item.Value == null || !item.Value.Equals(value))
                    {
                        linkedList.Remove(item);
                        linkedList.AddLast(new KeyValuePair<TKey, TValue?>(key, value));
                    }
                    break;
                }
            }

            if (!foundItem) throw new ArgumentOutOfRangeException(nameof(key));
        }

        /// <summary> Удалить элемент из словаря по ключу </summary>
        /// <param name="key"> Ключ </param>
        public bool Remove(TKey key)
        {
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);
            var itemFound = false;
            var foundItem = default(KeyValuePair<TKey, TValue?>);
            foreach (var item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key))
                {
                    itemFound = true;
                    foundItem = item;
                }
            }

            if (itemFound)
            {
                linkedList.Remove(foundItem);
                Count--;
            }
            return itemFound;
        }

        /// <summary> Проверить содержится ли ключ в словаре </summary>
        /// <param name="key"> Ключ </param>
        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);

            var foundItem = false;
            foreach (var item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key))
                {
                    foundItem = true;
                    break;
                }
            }

            return foundItem;
        }

        /// <summary> Получить значение по ключу </summary>
        /// <param name="key"> Ключ </param>
        public TValue? GetValue(TKey key)
        {
            var position = GetListPosition(key);
            var linkedList = GetLinkedList(position);
            foreach (var item in linkedList)
            {
                if (item.Key != null && item.Key.Equals(key)) return item.Value;
            }

            return default;
        }

        /// <summary> Попробовать получить значение по ключу </summary>
        /// <param name="key"> Ключ </param>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var t = GetValue(key);
            value = t;
            return t != null;
        }

        public TValue this[TKey key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                foreach (var item in _items.Where(x => x != null))
                {
                    foreach (var pair in item)
                    {
                        yield return pair.Key;
                    }
                }
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                foreach (var item in _items.Where(x => x != null))
                {
                    foreach (var pair in item)
                    {
                        yield return pair.Value;
                    }
                }
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => GetKeys();

        ICollection IDictionary.Values => GetValues();

        ICollection IDictionary.Keys => GetKeys();

        ICollection<TValue> IDictionary<TKey, TValue>.Values => GetValues();

        private List<TKey> GetKeys()
        {
            var collection = new List<TKey>();
            foreach (var item in _items.Where(x => x != null))
            {
                collection.AddRange(item.Select(pair => pair.Key));
            }

            return collection;
        }

        private List<TValue> GetValues()
        {
            var collection = new List<TValue>();
            foreach (var item in _items.Where(x => x != null))
            {
                collection.AddRange(item.Select(pair => pair.Value));
            }

            return collection;
        }

        /// <summary> Получить позицию списка </summary>
        /// <param name="key"> Ключ </param>
        protected int GetListPosition(TKey key)
        {
            return _hashMethodType switch
            {
                HashMethodType.Div => key.GetHashCodeDivMethod(_size),
                HashMethodType.Multi => key.GetHashCodeMultiMethod(_size),
                _ => key.GetHashCodeDivMethod(_size)
            };
        }

        /// <summary> Получить связный список </summary>
        /// <param name="position"> Позиция в таблице </param>
        protected LinkedList<KeyValuePair<TKey, TValue?>> GetLinkedList(int position)
        {
            var linkedList = _items[position];
            if (linkedList == null)
            {
                linkedList = new LinkedList<KeyValuePair<TKey, TValue?>>();
                _items[position] = linkedList;
            }

            return linkedList;
        }

        /// <summary> Проверить совместим ли ключ </summary>
        /// <param name="key"> Ключ </param>
        private static bool IsCompatibleKey(object? key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return key is TKey;
        }

        /// <summary> Проверить совместимо ли значение </summary>
        /// <param name="value"> Значение</param>
        private static bool IsCompatibleValue(object? value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value is TValue;
        }

        /// <summary> Проверить содержится ли ключ в словаре </summary>
        /// <param name="key"> Ключ </param>
        public bool Contains(object? key)
        {
            return IsCompatibleKey(key) && ContainsKey((TKey) key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            // Псевдореализация
            return _items.Where(item => item != null).SelectMany(item => item).ToDictionary(x => (x.Key, x.Value)).GetEnumerator();
        }

        /// <summary> Удалить элемент из словаря по ключу </summary>
        /// <param name="key"> Ключ </param>
        public void Remove(object key)
        {
            if (IsCompatibleKey(key)) Remove((TKey) key);
        }

        /// <summary> Фиксированный ли размер хеш-таблицы </summary>
        public bool IsFixedSize => true;

        /// <summary> Коллекция только для чтения ? </summary>
        public bool IsReadOnly => false;

        public object? this[object key]
        {
            get
            {
                if (IsCompatibleKey(key)) return GetValue((TKey) key);
                return null;
            }
            set
            {
                if (IsCompatibleKey(key) && IsCompatibleValue(value)) SetValue((TKey) key, (TValue) value);
            }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            // Псевдореализация
            return _items.Where(item => item != null).SelectMany(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Псевдореализация
            return _items.Where(item => item != null).SelectMany(item => item).GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(object key, object? value)
        {
            if (IsCompatibleKey(key) && IsCompatibleValue(value)) Add((TKey) key, (TValue) value);
        }

        /// <summary> Очистить словарь </summary>
        public void Clear()
        {
            if (Count > 0)
            {
                for (var i = 0; i < _items.Length; i++)
                {
                    _items[i] = null;
                }
            }
        }

        /// <summary> Проверить содержится ли ключ в словаре </summary>
        /// <param name="item"> Пара ключ значение </param>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var t = GetValue(item.Key);
            if ((t == null && item.Value == null) || t.Equals(item.Value)) return Contains(item.Key);
            return false;
        }

        /// <summary> Копировать элементы коллекции в массив </summary>
        /// <param name="array"> Массив </param>
        /// <param name="arrayIndex"> Индекс (позиция) </param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < Count) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            foreach (var item in _items)
            {
                if (item != null)
                {
                    foreach (var pair in item)
                    {
                        array[arrayIndex++] = new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
                    }
                }
            }
        }

        /// <summary> Удалить элемент из словаря по ключу </summary>
        /// <param name="item"> Пара ключ значение </param>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var t = GetValue(item.Key);
            if ((t == null && item.Value == null) || t.Equals(item.Value)) return Remove(item.Key);
            return false;
        }

        /// <summary> Копировать элементы коллекции в массив </summary>
        /// <param name="array"> Массив </param>
        /// <param name="index"> Индекс (позиция) </param>
        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            if (array.Rank != 1) throw new ArgumentException(nameof(array.Rank));

            if (array.GetLowerBound(0) != 0) throw new ArgumentNullException(nameof(array.GetLowerBound));

            if (index < 0 || index > array.Length) throw new ArgumentOutOfRangeException(nameof(index));

            if (array.Length - index < Count) throw new ArgumentOutOfRangeException(nameof(index));

            if (array is KeyValuePair<TKey, TValue>[] pairs) {
                CopyTo(pairs, index);
            }
            else
            {
                throw new AggregateException(nameof(array));
            }
        }

        /// <summary> Синхронизирована ли коллекция ? </summary>
        public bool IsSynchronized => false;

        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null) 
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);    
                }

                return _syncRoot;
            }
        }
    }
}
