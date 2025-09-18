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
        private readonly LoggerService _logger;

        public AuthService(WebshopContext db, LoggerService logger)
        {
            _db = db;
            _logger = logger;
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
                Console.WriteLine("Användare eller lösenord är fel!");
                _logger.Log("LoginFailed", "Försök att logga in med ogiltigt användarnamn", username);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("Användare eller lösenord är fel!");
                _logger.Log("LoginFailed", "Fel lösenord", username);
                return null;
            }

            Console.WriteLine($"Välkommen {user.Username} ({user.Role})");
            _logger.Log("LoginSuccess", "Användaren loggade in", username);
            return user;
        }

        public void Register()
        {
            Console.Write("Välj användarnamn: ");
            string username = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                Console.WriteLine("Användarnamnet måste vara minst 3 tecken.");
                _logger.Log("RegisterFailed", "För kort användarnamn", username); 
                return;
            }

            if (_db.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Användarnamnet är upptaget!");
                _logger.Log("RegisterFailed", "Användarnamnet är upptaget", username);
                return;
            }

            Console.Write("Välj lösenord: ");
            string password = ReadPassword();

            if (password.Length < 6)
            {
                Console.WriteLine("Lösenordet måste vara minst 6 tecken långt.");
                _logger.Log("RegisterFailed", "För kort lösenord", username);
                return;
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                PasswordHash = hash,
                Role = "User"
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            _logger.Log("RegisterSuccess", "Ny användare registrerad", username);
            Console.WriteLine("Registrering klar! Du kan nu logga in.");
        }

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
