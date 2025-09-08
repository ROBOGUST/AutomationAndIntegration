using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;

namespace AutomationAndIntegration.Services
{
    public class AuthService
    {
        private readonly WebshopContext _db;

        public AuthService(WebshopContext db)
        {
            _db = db;
        }

        public User? Login()
        {
            Console.Write("Användarnamn: ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Lösenord: ");
            string password = ReadPassword();

            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("Användaren finns inte!");
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("Fel lösenord!");
                return null;
            }

            Console.WriteLine($"Välkommen {user.Username} ({user.Role})");
            return user;
        }

        public void Register()
        {
            Console.Write("Välj användarnamn: ");
            string username = Console.ReadLine() ?? "";

            if (_db.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Användarnamnet är upptaget!");
                return;
            }

            Console.Write("Välj lösenord: ");
            string password = ReadPassword();

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                PasswordHash = hash,
                Role = "User"
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            Console.WriteLine("Registrering klar! Du kan nu logga in.");
        }

        // Method to hide the password in console
        private string ReadPassword()
        {
            string pass = "";
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }
    }
}
