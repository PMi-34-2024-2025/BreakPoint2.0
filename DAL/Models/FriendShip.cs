public class Friendship
{
    public int FriendshipId { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }

    // Навігаційні властивості
    public virtual User User1 { get; set; }
    public virtual User User2 { get; set; }
}
