using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ThoughtFocus.Common
{
    public static class HashHelper
    {
        public static string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public static string GenerateStringHash(string stringForHashing, string salt)
        {
            return GenerateStringHash(stringForHashing, salt, HashAlgorithm.Create("SHA1"));
        }

        public static string GenerateStringHash(string stringForHashing)
        {
            return GenerateStringHash(stringForHashing, string.Empty, HashAlgorithm.Create("MD5"));
        }

        public static string GenerateStringHash(string stringForHashing, HashAlgorithm hashAlgorithm)
        {
            if (hashAlgorithm is KeyedHashAlgorithm)
                throw new NotSupportedException("It is impossible to create Hash with KeyedHashAlgorithm and empty Salt");

            return GenerateStringHash(stringForHashing, string.Empty, hashAlgorithm);
        }

        public static string GenerateStringHash(string stringForHashing, string salt, HashAlgorithm hashAlgorithm)
        {
            return Convert.ToBase64String(GenerateBinaryHash(stringForHashing, salt, hashAlgorithm));
        }

        public static byte[] GenerateBinaryHash(string stringForHashing)
        {
            return GenerateBinaryHash(stringForHashing, string.Empty, HashAlgorithm.Create("MD5"));
        }

        public static byte[] GenerateBinaryHash(string stringForHashing, string salt, HashAlgorithm hashAlgorithm)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(stringForHashing);
            byte[] src = Convert.FromBase64String(salt);
            byte[] inArray;
            if (hashAlgorithm is KeyedHashAlgorithm)
            {
                var keyedHashAlgorithm = (KeyedHashAlgorithm) hashAlgorithm;
                if (keyedHashAlgorithm.Key.Length == src.Length)
                {
                    keyedHashAlgorithm.Key = src;
                }
                else if (keyedHashAlgorithm.Key.Length < src.Length)
                {
                    var dst = new byte[keyedHashAlgorithm.Key.Length];
                    Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
                    keyedHashAlgorithm.Key = dst;
                }
                else
                {
                    int count;
                    var buffer = new byte[keyedHashAlgorithm.Key.Length];
                    for (int i = 0; i < buffer.Length; i += count)
                    {
                        count = Math.Min(src.Length, buffer.Length - i);
                        Buffer.BlockCopy(src, 0, buffer, i, count);
                    }
                    keyedHashAlgorithm.Key = buffer;
                }

                inArray = keyedHashAlgorithm.ComputeHash(bytes);
            }
            else
            {
                var buffer = new byte[src.Length + bytes.Length];
                Buffer.BlockCopy(src, 0, buffer, 0, src.Length);
                Buffer.BlockCopy(bytes, 0, buffer, src.Length, bytes.Length);
                inArray = hashAlgorithm.ComputeHash(buffer);
            }

            return inArray;
        }

        /// <summary>
        /// Вычисляет хэш MD5 для метаинформации метода.
        /// </summary>
        /// <param name="methodInfo">
        /// Метаинформация метода
        /// <see cref="System.Reflection.MethodInfo">MethodInfo</see>
        /// , для которого вычисляется хэш MD5.
        /// </param>
        /// <returns>
        /// Хэш MD5, преобразованный в
        /// <see cref="System.Guid">Guid</see>
        /// .
        /// </returns>
        /// <remarks>
        /// Хеш вычисляется с учетом сигнатуры метода.
        /// </remarks>
        public static Guid FromMethodInfo(MethodInfo methodInfo)
        {
            string assemblyAndClassAndMethod = methodInfo.ReflectedType.Assembly.FullName.Split(',')[0] + methodInfo.ReflectedType.FullName + methodInfo.Name;
            string result = assemblyAndClassAndMethod + "(";
            foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
                result += paramInfo.ParameterType + ",";
            if (methodInfo.GetParameters().Length > 0)
                string.Format("{0})", result.Remove(result.Length - 1, 1));
            else
                result += ")";
            return new Guid(GenerateBinaryHash(result));
        }

        /// <summary>
        /// Вычисляет хэш MD5 для метаинформации параметра.
        /// </summary>
        /// <param name="parameterInfo">
        /// Метаинформация параметра
        /// <see cref="System.Reflection.ParameterInfo">ParameterInfo</see>
        /// , для которого вычисляется хэш MD5.
        /// </param>
        /// <returns>
        /// Хэш MD5, преобразованный в
        /// <see cref="System.Guid">Guid</see>
        /// .
        /// </returns>
        /// <remarks>
        /// Хеш вычисляется по сигнатуре метода и типу параметра.
        /// </remarks>
        public static Guid FromParameterInfo(ParameterInfo parameterInfo)
        {
            MethodInfo methodInfo = (MethodInfo)parameterInfo.Member;
            string assemblyAndClassAndMethod = methodInfo.ReflectedType.Assembly.FullName.Split(',')[0] + methodInfo.ReflectedType.FullName + methodInfo.Name;
            string result = assemblyAndClassAndMethod + "(";
            foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
                result += paramInfo.ParameterType + ",";
            if (methodInfo.GetParameters().Length > 0)
                string.Format("{0})", result.Remove(result.Length - 1, 1));
            else
                result += ")";
            return new Guid(GenerateBinaryHash(result + parameterInfo.ParameterType.FullName));
        }
    }
}