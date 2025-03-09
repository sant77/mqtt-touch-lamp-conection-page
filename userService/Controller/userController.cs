using Microsoft.AspNetCore.Mvc;
using userService.Data;
using userService.Models;
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
    [Route("api/[controller]")]
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

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, object> request)
        {
            try
            {
                // Validar que el request contiene los campos necesarios
                if (!request.ContainsKey("email") || !request.ContainsKey("password"))
                {
                    return BadRequest("El request debe contener 'email' y 'password'.");
                }

                var email = request["email"].ToString();
                var password = request["password"].ToString();

                // Buscar el usuario en la base de datos
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || !VerifyPassword(password, user.Password))
                {
                    return Unauthorized("Credenciales inválidas.");
                }

                // Generar y devolver el token JWT
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el método Login.");
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // GET: api/user
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
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

        // GET: api/user/all
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

        // GET: api/user/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(Guid id)
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

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] Dictionary<string, object> userData)
        {
            try
            {
                // Validar que el request contiene los campos necesarios
                if (!userData.ContainsKey("email") || !userData.ContainsKey("password") || !userData.ContainsKey("name"))
                {
                    return BadRequest("El request debe contener 'email', 'password' y 'name'.");
                }

                // Verificar si ya existe un usuario con el mismo email
                var email = userData["email"].ToString();
                if (_context.Users.Any(u => u.Email == email))
                {
                    return BadRequest("Ya existe un usuario con ese email.");
                }

                // Crear el nuevo usuario
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Password = HashPassword(userData["password"].ToString()),
                    Name = userData["name"].ToString()
                };

                // Guardar el usuario en la base de datos
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsuario), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario.");
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(Guid id, [FromBody] Dictionary<string, object> userData)
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

        // DELETE: api/user/{id}
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

        // Método para generar el token JWT
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