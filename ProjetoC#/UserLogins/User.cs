using ProjetoC_.UserLogins;

namespace ProjetoC_.UserLogins
{

    // Ok so I willnot store these passwords in plain text, which means I got to hash and salt them first
    public class User
    {
        public int id_user { get; set; }

        public string email { get; set; } = String.Empty!;

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
