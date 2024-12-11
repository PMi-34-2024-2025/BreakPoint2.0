using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SettingsSession
    {
        public int Id { get; set; } // Унікальний ідентифікатор сесії

        public string MessageText { get; set; } // Текст повідомлення

        public int UserId { get; set; } // ID користувача

        public TimeSpan NotificationFrequency { get; set; } // Частота сповіщення у форматі часу

        // Навігаційна властивість для зв'язку з таблицею Users
        public User User { get; set; } // Користувач, якому належать налаштування сесій
    }
}
