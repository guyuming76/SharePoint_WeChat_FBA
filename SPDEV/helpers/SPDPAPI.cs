using Microsoft.SharePoint;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SharePoint.Helpers
{
    //public static class Entropy
    //{
    //    public static string key
    //    {
    //        get
    //        {
    //            return "Wicresoft";
    //        }
    //    }
    //}
    public class EncryptTool
    {
     
        public static string Encrypt(string text, string key,bool InteractiveUser)
        {
            //DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            //byte[] bytes = Encoding.UTF8.GetBytes(text);
            //dESCryptoServiceProvider.Key = (Encoding.ASCII.GetBytes(key));
            //dESCryptoServiceProvider.IV = (Encoding.ASCII.GetBytes(key));

            ////dESCryptoServiceProvider.Padding = PaddingMode.Zeros;

            //MemoryStream memoryStream = new MemoryStream();
            //CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            //cryptoStream.Write(bytes, 0, bytes.Length);
            //cryptoStream.FlushFinalBlock();
            //byte[] array = memoryStream.ToArray();
            if (string.IsNullOrEmpty(text))
            { return string.Empty; }
            else
            {
                byte[] array = null;
                if (InteractiveUser)
                {
                    array = ProtectedData.Protect(Encoding.UTF8.GetBytes(text), Encoding.UTF8.GetBytes(key), DataProtectionScope.CurrentUser);
                }
                else
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        array = ProtectedData.Protect(Encoding.UTF8.GetBytes(text), Encoding.UTF8.GetBytes(key), DataProtectionScope.CurrentUser);

                    });
                }
                return Convert.ToBase64String(array);
            }
         }

        internal static string Decrypt(string text, string key,bool InteractiveUser)
        {
            //return text;

            //DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            //byte[] array = Convert.FromBase64String(text);
            //dESCryptoServiceProvider.Key = (Encoding.ASCII.GetBytes(key));
            //dESCryptoServiceProvider.IV = (Encoding.ASCII.GetBytes(key));

            ////dESCryptoServiceProvider.Padding = PaddingMode.Zeros;

            //MemoryStream memoryStream = new MemoryStream();
            //CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
            //cryptoStream.Write(array, 0, array.Length);
            //cryptoStream.FlushFinalBlock();
            //StringBuilder stringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(text))
            { return string.Empty; }
            else if (text.Trim().Equals(string.Empty))
            { return string.Empty; }
            else
            {
                byte[] array = null;
                //return Encoding.UTF8.GetString(memoryStream.ToArray());
                if (InteractiveUser)
                {
                    array = ProtectedData.Unprotect(Convert.FromBase64String(text), Encoding.UTF8.GetBytes(key), DataProtectionScope.CurrentUser);
                }
                else
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        array = ProtectedData.Unprotect(Convert.FromBase64String(text), Encoding.UTF8.GetBytes(key), DataProtectionScope.CurrentUser);
                    });
                }
                return Encoding.UTF8.GetString(array);
            }
        }
    }
}

