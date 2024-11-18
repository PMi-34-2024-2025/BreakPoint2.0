using DAL.Models;

public class Game
{
    public int GameId { get; set; } // Первинний ключ
    public string GameName { get; set; }

    // Зв'язок з сесіями (одна гра має багато сесій)
    public virtual ICollection<Sessions> Sessions { get; set; } = new List<Sessions>();
}
