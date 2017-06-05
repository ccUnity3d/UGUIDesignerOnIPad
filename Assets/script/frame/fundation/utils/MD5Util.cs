using System;
using System.Security.Cryptography;
using System.Text;

namespace foundation
{
    public class MD5Util
    {

        /// <summary>
        /// MD5 加密字符串，返回加密后32位小写.字符串
        /// </summary>
        public static string ToMD5String(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }

            return sb.ToString();
        }


        /// <summary>
        /// MD5 加密,加密后密码默认为32位小写.
        /// </summary>
        public static string hash(string intputString, bool isUppercase = false, int bit = 32)
        {
            string md5Str = "";

            if (bit == 16)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                md5Str = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(intputString)), 4, 8);
                md5Str = md5Str.Replace("-", "");

                if (!isUppercase)
                {
                    md5Str = md5Str.ToLower();
                }

                return md5Str;
            }

            System.Security.Cryptography.MD5 md5_ = System.Security.Cryptography.MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5_.ComputeHash(Encoding.UTF8.GetBytes(intputString));

            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                if (isUppercase)
                {
                    md5Str = md5Str + s[i].ToString("X");
                }
                else
                {
                    md5Str = md5Str + s[i].ToString("x");
                }
            }

            return md5Str;
        }


    }

}

