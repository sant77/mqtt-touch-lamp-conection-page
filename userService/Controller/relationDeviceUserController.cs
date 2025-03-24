using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using userService.Data;
using userService.Models;
using Microsoft.AspNetCore.Authorization;

namespace userService.Controllers
{
    [Route("userservice/v1/[controller]")]
    [ApiController]
    public class DeviceUserRelationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeviceUserRelationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /userservice/v1/DeviceUserRelation
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDeviceUserRelation([FromBody] Dictionary<string, object> request)
        {
            try
            {
                // Validar que el request contiene los campos necesarios
                if (!request.ContainsKey("deviceName"))
                {
                    return BadRequest("El request debe contener 'deviceName'.");
                }

                // Extraer el ID del usuario del token
                var userId = GetUserIdFromToken();
                if (userId == Guid.Empty)
                {
                    return Unauthorized("Token inválido: no se encontró el usuario.");
                }

                // Buscar el dispositivo por nombre
                var deviceName = request["deviceName"].ToString();
                var device = await _context.Devices.FirstOrDefaultAsync(d => d.Name == deviceName);
                if (device == null)
                {
                    return NotFound($"Dispositivo con nombre '{deviceName}' no encontrado.");
                }

                // Crear la relación
                var relation = new DeviceUserRelation
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DeviceId = device.Id,
                    SetDevice = false
                };

                _context.DeviceUserRelations.Add(relation);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDeviceUserRelation), new { id = relation.Id }, relation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // GET: /userservice/v1/DeviceUserRelation
        [HttpGet]
        public async Task<IActionResult> GetDeviceUserRelations()
        {
            try
            {
                var relations = await _context.DeviceUserRelations
                    .Include(du => du.User)
                    .Include(du => du.Device)
                    .ToListAsync();

                return Ok(relations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // GET: /userservice/v1/DeviceUserRelation/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceUserRelation(Guid id)
        {
            try
            {
                var relation = await _context.DeviceUserRelations
                    .Include(r => r.User)
                    .Include(r => r.Device)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (relation == null)
                {
                    return NotFound();
                }

                return Ok(relation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // PUT: /userservice/v1/DeviceUserRelation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeviceUserRelation(Guid id, [FromBody] DeviceUserRelation relation)
        {
            try
            {
                if (id != relation.Id)
                {
                    return BadRequest();
                }

                _context.Entry(relation).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceUserRelationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // DELETE: /userservice/v1/DeviceUserRelation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceUserRelation(Guid id)
        {
            try
            {
                var relation = await _context.DeviceUserRelations.FindAsync(id);
                if (relation == null)
                {
                    return NotFound();
                }

                _context.DeviceUserRelations.Remove(relation);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // GET: /userservice/v1/DeviceUserRelation/by-user
        [HttpGet("by-user")]
        public async Task<IActionResult> GetDeviceUserRelationsByUserId()
        {
            try
            {
                // Extraer el ID del usuario del token
                var userId = GetUserIdFromToken();
                if (userId == Guid.Empty)
                {
                    return Unauthorized("Token inválido: no se encontró el usuario.");
                }

                // Buscar las relaciones por UserId
                var relations = await _context.DeviceUserRelations
                    .Include(du => du.Device)
                    .Where(du => du.UserId == userId)
                    .ToListAsync();

                if (relations == null || !relations.Any())
                {
                    return NotFound($"No se encontraron relaciones para el usuario con ID '{userId}'.");
                }

                // Mapear las relaciones al DTO personalizado
                var response = relations.Select(relation => new DeviceUserRelationResponse
                {
                    Id = relation.Id,
                    Nombre = "sunshine",
                    Dispositivo = relation.Device?.Name ?? "Desconocido",
                    Configurado = relation.SetDevice,
                    Description = $"Relación entre el usuario {relation.UserId} y el dispositivo {relation.DeviceId}"
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // GET: /userservice/v1/DeviceUserRelation/by-email?email=usuario@example.com
        [HttpGet("by-email")]
        public async Task<IActionResult> GetDeviceUserRelationsByEmail(string email)
        {
            try
            {
                // Validar que el correo electrónico no esté vacío
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("El correo electrónico no puede estar vacío.");
                }

                // Buscar el usuario por su correo electrónico
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return NotFound($"No se encontró un usuario con el correo electrónico '{email}'.");
                }

                // Buscar las relaciones de dispositivos para el usuario encontrado
                var relations = await _context.DeviceUserRelations
                    .Include(du => du.Device)
                    .Where(du => du.UserId == user.Id)
                    .ToListAsync();

                if (relations == null || !relations.Any())
                {
                    return NotFound($"No se encontraron dispositivos vinculados para el usuario con correo electrónico '{email}'.");
                }

                // Mapear las relaciones al DTO personalizado
                var response = relations.Select(relation => new DeviceUserRelationResponse
                {
                    Id = relation.Id,
                    Nombre = "sunshine",
                    Dispositivo = relation.Device?.Name ?? "Desconocido",
                    Configurado = relation.SetDevice,
                    Description = $"dispositivo: {relation.UserId}/{relation.DeviceId}"
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        [HttpPost("validate-device")]
        public async Task<IActionResult> ValidateDeviceForESP32([FromBody] Esp32DeviceValidationRequest request)
        {
            try
            {
                // Validar campos requeridos
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.DeviceToken) || string.IsNullOrEmpty(request.DeviceName))
                {
                    return BadRequest("Email, DeviceToken y DeviceName son campos requeridos.");
                }

                // Buscar usuario por email y device token
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.DeviceToken == request.DeviceToken);

                if (user == null)
                {
                    return Unauthorized("Credenciales inválidas o usuario no encontrado.");
                }

                // Buscar dispositivo por nombre
                var device = await _context.Devices
                    .FirstOrDefaultAsync(d => d.Name == request.DeviceName);

                if (device == null)
                {
                    return NotFound($"Dispositivo con nombre '{request.DeviceName}' no encontrado.");
                }

                // Buscar relación usuario-dispositivo
                var deviceUserRelation = await _context.DeviceUserRelations
                    .Include(dur => dur.Device)
                    .FirstOrDefaultAsync(dur => dur.UserId == user.Id && dur.DeviceId == device.Id);

                if (deviceUserRelation == null)
                {
                    return NotFound("No se encontró relación entre el usuario y el dispositivo especificado.");
                }

                // Buscar relaciones entre usuarios que involucren este dispositivo (opcional)
                var userRelations = await _context.RelationUsers
                    .Include(ru => ru.User1)
                    .Include(ru => ru.User2)
                    .Include(ru => ru.DeviceUserRelation1)
                    .Include(ru => ru.DeviceUserRelation2)
                    .Where(ru => (ru.User1.Id == user.Id || ru.User2.Id == user.Id) &&
                                (ru.DeviceUserRelation1.DeviceId == device.Id || 
                                ru.DeviceUserRelation2.DeviceId == device.Id))
                    .ToListAsync();

                // Actualizar SetDevice a true si se encontró la relación
                deviceUserRelation.SetDevice = true;
                _context.DeviceUserRelations.Update(deviceUserRelation);
                await _context.SaveChangesAsync();

                // Preparar respuesta
                var response = new Esp32DeviceValidationResponse
                {
                    IsValid = true,
                    DeviceUserRelationId = deviceUserRelation.Id,
                    UserRelations = userRelations.Select(ur => new UserRelationInfo
                    {
                        RelationId = ur.Id,
                        OtherUserId = ur.User1.Id == user.Id ? ur.User2.Id : ur.User1.Id,
                        DeviceUserRelationId = ur.DeviceUserRelation1.DeviceId == device.Id ? 
                            ur.DeviceUserRelation1.Id : ur.DeviceUserRelation2.Id
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // Clases para request y response
        public class Esp32DeviceValidationRequest
        {
            public string Email { get; set; }
            public string DeviceToken { get; set; }
            public string DeviceName { get; set; }
        }

        public class Esp32DeviceValidationResponse
        {
            public bool IsValid { get; set; }
            public Guid DeviceUserRelationId { get; set; }
            public List<UserRelationInfo> UserRelations { get; set; } = new List<UserRelationInfo>();
        }

        public class UserRelationInfo
        {
            public Guid RelationId { get; set; }
            public Guid OtherUserId { get; set; }
            public Guid DeviceUserRelationId { get; set; }
        }

        // Método para obtener el ID del usuario desde el token
        private Guid GetUserIdFromToken()
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier) || !Guid.TryParse(nameIdentifier, out Guid userId))
            {
                return Guid.Empty;
            }
            return userId;
        }

        // Método para verificar si existe una relación
        private bool DeviceUserRelationExists(Guid id)
        {
            return _context.DeviceUserRelations.Any(e => e.Id == id);
        }

        // DTO para la respuesta personalizada
        public class DeviceUserRelationResponse
        {
            public Guid Id { get; set; }
            public string? Nombre { get; set; }
            public string? Dispositivo { get; set; }
            public string? Description { get; set; }
            public bool Configurado { get; set; }
        }
    }
}