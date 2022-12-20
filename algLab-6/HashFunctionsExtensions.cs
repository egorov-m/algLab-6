using System.Data.SqlTypes;
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


        public static int GetHashCodeHMACMD5(object key, int sizeHashTable)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key.ToString());
            using (HMACMD5 md5 = new HMACMD5(secrectKey))
            {
                byte[] bytes_md5_in = Encoding.UTF8.GetBytes(key.ToString());
                byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);
                string str_md5_out = BitConverter.ToString(bytes_md5_out);
                str_md5_out = str_md5_out.Replace("-", "");

                int sum = 0;
                for (int i = 0; i < str_md5_out.Length; i++)
                {
                    sum += Convert.ToInt32(str_md5_out[i]);
                }
                return sum % sizeHashTable;
            }
        }

        public static int GetHashCodeSHA256(object key, int sizeHashTable)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes_sha256_in = Encoding.UTF8.GetBytes(key.ToString());
                byte[] bytes_sha256_out = sha256.ComputeHash(bytes_sha256_in);
                string str_sha256_out = BitConverter.ToString(bytes_sha256_out);
                str_sha256_out = str_sha256_out.Replace("-", "");

                int resultSHA256 = BitConverter.ToInt32(bytes_sha256_out, 0);

                return resultSHA256 % sizeHashTable;
            }
        }

        public static int GetHashCodeFNV(object key, int sizeHashTable)
        {

            const uint fnv_prime = 0x811C9DC5;
            uint hash = 0;
            uint i = 0;
            string str = key.ToString();


            for (i = 0; i < str.Length; i++)
            {
                hash *= fnv_prime;
                hash ^= ((byte)str[(int)i]);
            }

            return (int)hash % sizeHashTable;
        }
    }
}
