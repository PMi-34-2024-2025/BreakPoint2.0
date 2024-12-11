using DAL.Models;
using System.Collections.Generic;

public class User
{
    public int UserId { get; set; } // Первинний ключ
    public string UserName { get; set; }
    public string UserPassword { get; set; }
    public string UserEmail { get; set; }

    // Поле для визначення, чи є користувач адміністратором
    public bool IsAdmin { get; set; }

    // Список дружніх зв'язків (багато до багатьох)
    public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();

    // Список сесій (багато до одного)
    public virtual ICollection<Sessions> Sessions { get; set; } = new List<Sessions>();

    // Список налаштувань сесій (багато до одного)
    public virtual ICollection<SettingsSession> SettingsSessions { get; set; } = new List<SettingsSession>();
}
