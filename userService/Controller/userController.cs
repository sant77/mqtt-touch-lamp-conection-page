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

namespace userService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class userController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtSecret = "TuClaveSecretaSuperSeguraDe32CaracteresOmas";
        private readonly ILogger<userController> _logger; // Declarar el campo _logger

        public userController(ApplicationDbContext context, ILogger<userController> logger) // Inyectar ILogger
        {
            _context = context;
            _logger = logger; // Inicializar el campo _logger
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                return Unauthorized("Credenciales inválidas");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
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

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Users.ToListAsync();
            return Ok(usuarios);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var excludeEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            _logger.LogInformation("ExcludeEmail recibido: {ExcludeEmail}", excludeEmail); // Usar _logger

            var usersQuery = _context.Users.AsQueryable();

          
            usersQuery = usersQuery.Where(u => u.Email != excludeEmail);
            

            var usersWithDevices = await _context.Users
                .Where(u => u.DeviceUserRelations.Any()) // Solo usuarios con relaciones en DeviceUserRelation
                .Where(u => u.Email != excludeEmail) // Excluir el email del usuario autenticado
            .Select(u => new 
            { 
                u.Email, 
                u.Name 
            })
            .ToListAsync();

            return Ok(usersWithDevices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(Guid id)
        {
            var user = await _context.Users
                .Select(u => new { u.Id, u.Name, u.Email }) // Excluir la contraseña
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return Ok(user);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<IActionResult> CrearUsuario(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest("Ya existe un usuario con ese email.");
            }

            // Generar un UUID para el usuario
            user.Id = Guid.NewGuid();

            // Hashear la contraseña
            user.Password = HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsuario), new { id = user.Id }, user);
        }

        // Método para hashear contraseñas
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(Guid id, User user)
        {
            if (id != user.Id) return BadRequest();

            // Si se envía una nueva contraseña, la hasheamos
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = HashPassword(user.Password);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(Guid id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Users.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}