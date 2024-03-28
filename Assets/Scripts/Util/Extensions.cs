using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using VillageGame.Data;

namespace Util
{
    public static class Extensions
    {
        public static DateTime AsDateTime(this DayData dayData) =>
            new DateTime(year:dayData.Year,month: dayData.Month,day: dayData.Day);

        public static bool TryParseDate(string date, out DateTime time)
        {
            string[] formats =
            {
                "MM/dd/yyyy HH:mm:ss",
                "MM/dd/yyyy H:mm:ss",
                "yyyy-MM-ddTHH:mm:ssZ",
                "dd/MM/yyyy H:mm:ss",
                "dd/MM/yyyy HH:mm:ss",
                "dd.MM.yyyy HH:mm:ss",
                "dd.MM.yyyy H:mm:ss"
            };
            return DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
        }
        
        public static string GenerateRandomIdempotencyKey()
        {
            var randomBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(randomBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        
        public static void Shuffle<T>(this T[] array)
        {
            var random = new Random();

            for (var i = array.Length - 1; i > 0; i--)
            {
                var randomIndex = random.Next(0, i + 1);
                (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            }
        }
        
        public static string EncodeString(string input)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(input);
            string encodedText = Convert.ToBase64String(bytesToEncode);
            return encodedText;
        }

        public static string DecodeString(string encodedText)
        {
            if (encodedText == null)
            {
                return "";
            }
            byte[] decodedBytes = Convert.FromBase64String(encodedText);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);
            return decodedText;
        }
        
    }
}