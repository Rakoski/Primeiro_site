using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace ProjetoC_.UserLogins
{
    public class UserDto
    {
        public string email {  get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;
    }
}
