using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortner.Data
{
    public class UserRepo
    {
        private string _connectionString;
        public UserRepo(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void NewUser (User user, string password)
        {
            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(password, salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            using(var context = new UrlsDataContext(_connectionString))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }
        }
        public User Login (string email,string password)
        {
            var user = GetByEmail(email);
            if(user == null)
            {
                return null;
            }
            var isCorrect = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!isCorrect)
            {
                return null; 
            }
            return user;
        }

        public User GetByEmail(string email)
        {
            using (var context = new UrlsDataContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }
    }
    public static class PasswordHelper
    {
        public static string GenerateSalt()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[10];
            provider.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt)
        {
            SHA256Managed crypt = new SHA256Managed();
            string combinedString = password + salt;
            byte[] combined = Encoding.Unicode.GetBytes(combinedString);

            byte[] hash = crypt.ComputeHash(combined);
            return Convert.ToBase64String(hash);
        }

        public static bool PasswordMatch(string userInput, string salt, string passwordHash)
        {
            string userInputHash = HashPassword(userInput, salt);
            return passwordHash == userInputHash;
        }
    }
}
