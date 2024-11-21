using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL // Замініть на ваш простір імен
{
    public class UserService
    {
        // Список для зберігання користувачів (імітація бази даних)
        private readonly List<User> _users = new List<User>
        {
            new User { Email = "example@example.com", FirstName = "John", UserName = "john_doe", PasswordHash = "old_password_hash" },
            new User { Email = "jane@example.com", FirstName = "Jane", UserName = "jane_doe", PasswordHash = "old_password_hash" }
        };

        /// <summary>
        /// Оновлює пароль користувача
        /// </summary>
        /// <param name="email">Електронна пошта користувача</param>
        /// <param name="newPassword">Новий пароль</param>
        /// <returns>Повертає true, якщо оновлення успішне, інакше false</returns>
        public bool UpdatePassword(string email, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
            {
                return false; // Перевірка на порожні значення
            }

            // Пошук користувача за email
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return false; // Користувач не знайдений
            }

            // Оновлення пароля
            user.PasswordHash = HashPassword(newPassword);
            return true;
        }

        /// <summary>
        /// Хешування пароля (імітація)
        /// </summary>
        private string HashPassword(string password)
        {
            // Для спрощення повертаємо пароль у зворотному вигляді (замініть на реальне хешування)
            char[] passwordArray = password.ToCharArray();
            Array.Reverse(passwordArray);
            return new string(passwordArray);
        }
        // Метод для отримання користувача за email
        public User GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        // Метод для отримання користувача за username
        public User GetUserByUserName(string username)
        {
            return _users.FirstOrDefault(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

    }

    /// <summary>
    /// Модель користувача
    /// </summary>
    public class User
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
