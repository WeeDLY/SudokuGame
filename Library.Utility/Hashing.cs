using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Library.Utility
{
    /// <summary>
    /// Hashing utility class
    /// </summary>
    public sealed class Hashing
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Hashing"/> class from being created.
        /// </summary>
        private Hashing()
        {

        }

        /// <summary>
        /// The random
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// The salt characters
        /// </summary>
        private const string SaltCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        /// <summary>
        /// The salt length
        /// </summary>
        private const int SaltLength = 16;

        /// <summary>
        /// Gets the sha256 hash.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="salt">The salt.</param>
        /// <returns></returns>
        public static string GetSha256Hash(string input, string salt)
        {
            string pw = salt + input;

            SHA256 sha = SHA256.Create();
            var inputBytes = Encoding.ASCII.GetBytes(pw);
            var hashed = sha.ComputeHash(inputBytes);

            StringBuilder output = new StringBuilder();
            foreach (byte b in hashed)
                output.Append(b.ToString("x2"));

            return output.ToString();
        }

        /// <summary>
        /// Generates the salt.
        /// </summary>
        /// <returns></returns>
        public static string GenerateSalt()
        {
            return new string(Enumerable.Repeat(SaltCharacters, SaltLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}