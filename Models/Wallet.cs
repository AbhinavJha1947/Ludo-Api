namespace LudoGameApi.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int Coins { get; set; } = 0; // Default coins to 0
        public int UserId { get; set; } // Foreign key
        public User User { get; set; }
    }
}
