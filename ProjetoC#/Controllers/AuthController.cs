using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using ProjetoC_.UserLogins;
using System.Security.Cryptography;

namespace ProjetoC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();

        [HttpPost("register")]

        public async Task<ActionResult<User>> Register(UserDto request)
        {
            CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);

            user.email = request.email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return Ok(user);
        }
        [HttpPost("login")]

        // so basically this means that both the user email and the user password have to be correct
        // for the "Token" message to be generated on swagger. I'll try more experimenting with Tokens to see if I
        // can get them to connect me to my webpage
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (user.email != request.email)
            {
                return BadRequest("User not found");
            }

            if(!VerifyPasswordHash(request.password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Incorrect password");
            }

            return Ok("Token");
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
