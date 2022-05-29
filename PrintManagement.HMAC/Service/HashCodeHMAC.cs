using System;
using System.Security.Cryptography;
using System.Text;

namespace VCSLib.HMAC {
    public class HashCodeHMAC : IHashCodeHMAC {
        private static readonly string SecretKey = "914aeb3ecc4f459d8b07825cf1a3cfb2";
        public string ComputeHashCodeHMACSHA1(string content, string nonce, string timeStamp, string secretKey = "") {
            // Encrypt the string to an array of bytes.
            if (secretKey.Equals("")) {
                secretKey = SecretKey;
            }
            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
            var hashContent = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(content));
            var dataToHMAC = String.Format("{0}:{1}:{2}", nonce, timeStamp, content);
            
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToHMAC)));
        }

        public string ComputeHashCodeHMACSHA256(string content, string nonce, string timeStamp, string secretKey = "") {
            // Encrypt the string to an array of bytes.
            if (secretKey.Equals("")) {
                secretKey = SecretKey;
            }
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hashContent = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(content));
            var dataToHMAC = String.Format("{0}:{1}:{2}", nonce, timeStamp, content);

            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToHMAC)));
        }

        public string ComputeSignatureHMACSHA256(string content, string userId, string machineName, string secretKey = "") {
            if (secretKey.Equals("")) {
                secretKey = SecretKey;
            }
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hashContent = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(content));
            var dataToHMAC = String.Format("{0}:{1}:{2}", userId, machineName, content);

            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToHMAC)));
        }
    }
}
