using System.Security.Cryptography;
using System.Text;

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

        /// <summary> Получить хеш-код MD5 (HMACMD) </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="sizeHashTable"> Размер Хеш-таблицы </param>
        public static int GetHashCodeHMACMD5(this object key, int sizeHashTable)
        {
            var secrectKey = Encoding.UTF8.GetBytes(key.ToString());
            using var md5 = new HMACMD5(secrectKey);
            var bytes_md5_in = Encoding.UTF8.GetBytes(key.ToString());
            var bytes_md5_out = md5.ComputeHash(bytes_md5_in);
            var str_md5_out = BitConverter.ToString(bytes_md5_out);
            str_md5_out = str_md5_out.Replace("-", "");

            var sum = 0;
            for (var i = 0; i < str_md5_out.Length; i++)
            {
                sum += Convert.ToInt32(str_md5_out[i]);
            }
            return sum % sizeHashTable;
        }

        /// <summary> Получить хеш-код SHA256 </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="sizeHashTable"> Размер Хеш-таблицы </param>
        public static int GetHashCodeSHA256(this object key, int sizeHashTable)
        {
            using var sha256 = SHA256.Create();
            var bytes_sha256_in = Encoding.UTF8.GetBytes(key.ToString());
            var bytes_sha256_out = sha256.ComputeHash(bytes_sha256_in);
            var resultSHA256 = BitConverter.ToInt32(bytes_sha256_out, 0);

            return resultSHA256 % sizeHashTable;
        }

        /// <summary> Получить хеш-код FNV </summary>
        /// <param name="key"> Ключ </param>
        /// <param name="sizeHashTable"> Размер Хеш-таблицы </param>
        public static int GetHashCodeFNV(this object key, int sizeHashTable)
        {
            const uint fnv_prime = 0x811C9DC5;
            uint hash = 0;
            var str = key.ToString();

            for (uint i = 0; i < str.Length; i++)
            {
                hash *= fnv_prime;
                hash ^= ((byte)str[(int)i]);
            }

            return (int) hash % sizeHashTable;
        }
    }
}
