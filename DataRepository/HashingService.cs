namespace BidRestAPI.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using BidRestAPI.Interfaces;

    public class HashingService : IHashingService
    {
        public string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256())
            {
                var hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedPassword);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == hashedPassword;
        }
    }
}
