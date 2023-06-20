using ProjetoC_.UserLogins;

namespace ProjetoC_.UserLogins
{
    public class User
    {
        public int id_user { get; set; }

        public string email { get; set; } = String.Empty!;
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
