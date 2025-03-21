using Microsoft.AspNetCore.Mvc;
using userService.Data;
using userService.Models;
using userService.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace userService.Controllers
{
    [ApiController]
    [Route("userservice/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtSecret = "TuClaveSecretaSuperSeguraDe32CaracteresOmas";
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: /userservice/v1/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, object> request)
        {
            try
            {
                // Validar que el request contiene los campos necesarios
                if (!request.ContainsKey("email") || !request.ContainsKey("password"))
                {
                    return BadRequest(new { error = "El request debe contener 'email' y 'password'." });
                }

                var email = request["email"].ToString();
                var password = request["password"].ToString();

                // Buscar el usuario en la base de datos
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null || !VerifyPassword(password, user.Password))
                {
                    return Unauthorized(new { error = "Credenciales inválidas." });
                }

                if (user.EmailConfirmed != 1)
                {
                    return Unauthorized(new { error = "No has confirmado tu correo." });
                }

                // Generar y devolver el token JWT
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el método Login.");
                return StatusCode(500, new { error = "Ocurrió un error interno." });
            }
        }

        // GET: /userservice/v1/user
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users
                .Select(u => new { u.Email, u.Name })
                .ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios.");
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // GET: /userservice/v1/user/all
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var excludeEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("ExcludeEmail recibido: {ExcludeEmail}", excludeEmail);

                // Obtener usuarios con dispositivos asociados, excluyendo el usuario actual
                var usersWithDevices = await _context.Users
                    .Where(u => u.DeviceUserRelations.Any() && u.Email != excludeEmail)
                    .Select(u => new { u.Email, u.Name })
                    .ToListAsync();

                return Ok(usersWithDevices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios.");
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // GET: /userservice/v1/user/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _context.Users
                    .Select(u => new { u.Id, u.Name, u.Email })
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // POST: /userservice/v1/user
        [HttpPost]                       
        public async Task<IActionResult> CreateUser([FromBody] Dictionary<string, object> userData)
        {
            try
            {
                // check if the json was load with email, password and name
                if (!userData.ContainsKey("email") || !userData.ContainsKey("password") || !userData.ContainsKey("name"))
                {
                    return BadRequest(new {error = "El request debe contener 'email', 'password' y 'name'."});
                }

                // check if the user already exist
                var email = userData["email"].ToString();
                if (_context.Users.Any(u => u.Email == email))
                {
                    return BadRequest(new {error = "Ya existe un usuario con ese email."});
                }

                string token = Guid.NewGuid().ToString();
                // Crear user object
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Password = HashPassword(userData["password"].ToString()),
                    Name = userData["name"].ToString(),
                    EmailConfirmed = false,
                    ConfirmationToken = token
                };

                // Save user in db
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Send email confirmation
                var emailService = new EmailService();
                await emailService.SendConfirmationEmail(user.Email, token);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario.");
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // PUT: /userservice/v1/user/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] Dictionary<string, object> userData)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                // Actualizar el nombre si está presente en el request
                if (userData.ContainsKey("name"))
                {
                    user.Name = userData["name"].ToString();
                }

                // Actualizar la contraseña si está presente en el request
                if (userData.ContainsKey("password"))
                {
                    user.Password = HashPassword(userData["password"].ToString());
                }

                // Guardar los cambios en la base de datos
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID: {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // DELETE: /userservice/v1/user/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(Guid id)
        {
            try
            {
                var usuario = await _context.Users.FindAsync(id);
                if (usuario == null) return NotFound();

                _context.Users.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID: {Id}", id);
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.ConfirmationToken == token);

                if (user == null)
                {
                    return BadRequest("Token inválido.");
                }

                user.EmailConfirmed = true;
                user.ConfirmationToken = "";
                await _context.SaveChangesAsync();

                return Ok("Correo confirmado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar el correo.");
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }


        // Method to generate token
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: "yourapp",
                audience: "yourapp",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Método para verificar la contraseña
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        // Método para hashear la contraseña
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}