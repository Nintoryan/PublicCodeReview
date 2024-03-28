using System;
using System.Security.Cryptography;
using System.Text;

namespace Web
{
    public class HMAC
    {
        private const string secretKey = "mfVGiunP50jwOOibb0aUJY7QUc7U6ixV";

        public static string CreateHMAC(string input)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}