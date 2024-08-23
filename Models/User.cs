namespace LudoGameApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Wallet Wallet { get; set; } = new Wallet(); // Initialize with a new Wallet

    }

}
