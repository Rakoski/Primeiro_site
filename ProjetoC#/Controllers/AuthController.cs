using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System;
using System.Text.RegularExpressions;
using ProjetoC_.UserLogins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace ProjetoC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]

        public async Task<ActionResult<User>> Register([FromBody]UserDto request)
        {

            CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);

            string server = "localhost";
            string database = "cadastro_cliente";
            string username = "root";
            string password = "";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                            "UID=" + username + ";" + "PASSWORD=" + password + ";";

            string passwordHashString = Convert.ToBase64String(passwordHash);
            string passwordSaltString = Convert.ToBase64String(passwordSalt);

            string email = request.email; 

            bool isValidEmail = IsValidEmail(email);

            if (!isValidEmail)
            {
                return BadRequest("Invalid Email");
            }

            static bool IsValidEmail(string email)
            {
                string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

                return Regex.IsMatch(email, pattern);
            }

            using (MySqlConnection connection = new MySqlConnection(constring))
            {
                connection.Open();

                string query = "INSERT INTO Users (email, password_Hash, password_salt) VALUES (@email, @password_hash, @password_salt)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", request.email);
                    command.Parameters.AddWithValue("@password_hash", passwordHashString);
                    command.Parameters.AddWithValue("@password_salt", passwordSaltString);

                    command.ExecuteNonQuery();
                }
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody]UserDto request)
        {
            string server = "localhost";
            string database = "cadastro_cliente";
            string username = "root";
            string password = "";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                            "UID=" + username + ";" + "PASSWORD=" + password + ";";

            var conexao = new MySqlConnection(constring);

            using (MySqlConnection connection = new MySqlConnection(constring))
            {
                connection.Open();

                var query = "SELECT email, password_hash, password_salt FROM Users WHERE email = @email;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", request.email);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Email found in the database
                            string email = reader.GetString("email");
                            byte[] storedPasswordHash = Convert.FromBase64String(reader.GetString("password_hash"));
                            byte[] storedPasswordSalt = Convert.FromBase64String(reader.GetString("password_salt"));

                            if (!VerifyPasswordHash(request.password, storedPasswordHash, storedPasswordSalt))
                            {
                                return BadRequest("Incorrect password");
                            }

                            string token = CreateToken(user);
                            return Ok(new { token });
                        }
                        else
                        {
                            return BadRequest("User not found");
                        }
                    }
                }
            }
        }

        private string CreateToken(User user)
        { // how tf do I create a token???
            // Figured it out, basically I have to make claims, which are basically properties that describe/store
            // the user's information in the token (criptographed)
            // I will add just the email to the list of claim to be token-criptographed
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.email)
            }; // now need a simmetric security key

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
                // now the fun part. Key creation! And putting it in the token obv
                // This is how to create the JWT I guess, hope my laptop doesn't explode

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                // i'll also need to put an expiration date so it can't be used forever or meddled with
                expires: DateTime.Now.AddHours(6),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt)) 
                // comparing and checking bytes between the salting and hashing, if they are equal,
                // return their normal selfs password 
                // if the normal hash, the normal salt and the new ones as well as the normal password
                // turns out to be the same values, then the user has entered the correct email and password
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

    }
}
