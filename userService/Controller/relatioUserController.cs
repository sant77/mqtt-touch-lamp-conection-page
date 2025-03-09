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
    [Route("api/[controller]")]
    [ApiController]
    public class RelationUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RelationUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RelationUser
        [HttpGet]
        public async Task<IActionResult> GetRelationUsers()
        {
            try
            {
                var relationUsers = await _context.RelationUsers
                    .Include(r => r.User1)
                    .Include(r => r.User2)
                    .Include(r => r.DeviceUserRelation1)
                    .Include(r => r.DeviceUserRelation2)
                    .ToListAsync();

                return Ok(relationUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // GET: api/RelationUser/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRelationUser(Guid id)
        {
            try
            {
                var relationUser = await _context.RelationUsers
                    .Include(r => r.User1)
                    .Include(r => r.User2)
                    .Include(r => r.DeviceUserRelation1)
                    .Include(r => r.DeviceUserRelation2)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (relationUser == null)
                {
                    return NotFound();
                }

                return Ok(relationUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // POST: api/RelationUser
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRelationUser([FromBody] Dictionary<string, object> request)
        {
            try
            {
                // Validar que el request contiene los campos necesarios
                if (!request.ContainsKey("userId1") || !request.ContainsKey("userId2"))
                {
                    return BadRequest("El request debe contener 'userId1' y 'userId2'.");
                }

                // Obtener los IDs de los usuarios
                var userId1 = Guid.Parse(request["userId1"].ToString());
                var userId2 = Guid.Parse(request["userId2"].ToString());

                // Verificar que los usuarios no sean el mismo
                if (userId1 == userId2)
                {
                    return BadRequest("User1 y User2 no pueden ser el mismo.");
                }

                // Crear la relación
                var relationUser = new RelationUser
                {
                    Id = Guid.NewGuid(),
                    UserId1 = userId1,
                    UserId2 = userId2,
                    DeviceUserRelationId1 = request.ContainsKey("deviceUserRelationId1") ? Guid.Parse(request["deviceUserRelationId1"].ToString()) : (Guid?)null,
                    DeviceUserRelationId2 = request.ContainsKey("deviceUserRelationId2") ? Guid.Parse(request["deviceUserRelationId2"].ToString()) : (Guid?)null
                };

                _context.RelationUsers.Add(relationUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRelationUser), new { id = relationUser.Id }, relationUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // PUT: api/RelationUser/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRelationUser(Guid id, [FromBody] RelationUser relationUser)
        {
            try
            {
                if (id != relationUser.Id)
                {
                    return BadRequest();
                }

                _context.Entry(relationUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelationUserExists(id))
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

        // DELETE: api/RelationUser/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRelationUser(Guid id)
        {
            try
            {
                var relationUser = await _context.RelationUsers.FindAsync(id);
                if (relationUser == null)
                {
                    return NotFound();
                }

                _context.RelationUsers.Remove(relationUser);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // POST: api/RelationUser/create-relation
        [Authorize]
        [HttpPost("create-relation")]
        public async Task<IActionResult> CreateUserRelation([FromBody] Dictionary<string, object> request)
        {
            try
            {
                // Validar que el request contiene los campos necesarios
                if (!request.ContainsKey("user") || !request.ContainsKey("deviceOwnerId") || !request.ContainsKey("deviceId"))
                {
                    return BadRequest("El request debe contener 'user', 'deviceOwnerId' y 'deviceId'.");
                }

                // Obtener el ID del usuario autenticado desde el token
                var loggedInUserId = GetUserIdFromToken();
                if (loggedInUserId == Guid.Empty)
                {
                    return Unauthorized("Token inválido: no se encontró el usuario.");
                }

                // Buscar al usuario que se desea vincular (usando el correo electrónico)
                var userToLink = await _context.Users.FirstOrDefaultAsync(u => u.Email == request["user"].ToString());
                if (userToLink == null)
                {
                    return NotFound($"No se encontró un usuario con el correo electrónico '{request["user"]}'.");
                }

                // Verificar que el usuario autenticado no esté intentando vincularse a sí mismo
                if (loggedInUserId == userToLink.Id)
                {
                    return BadRequest("No puedes vincularte a ti mismo.");
                }

                // Crear la relación entre usuarios
                var relationUser = new RelationUser
                {
                    Id = Guid.NewGuid(),
                    UserId1 = loggedInUserId, // Usuario autenticado
                    UserId2 = userToLink.Id, // Usuario a vincular
                    DeviceUserRelationId1 = Guid.Parse(request["deviceOwnerId"].ToString()), // Dispositivo del usuario autenticado
                    DeviceUserRelationId2 = Guid.Parse(request["deviceId"].ToString()) // Dispositivo del usuario a vincular
                };

                // Guardar la relación en la base de datos
                _context.RelationUsers.Add(relationUser);
                await _context.SaveChangesAsync();

                // Devolver la relación creada
                return CreatedAtAction(nameof(GetRelationUser), new { id = relationUser.Id }, relationUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // GET: api/RelationUser/by-user
        [Authorize]
        [HttpGet("by-user")]
        public async Task<IActionResult> GetUsersRelationsByUserId()
        {
            try
            {
                // Obtener el ID del usuario autenticado desde el token
                var loggedInUserId = GetUserIdFromToken();
                if (loggedInUserId == Guid.Empty)
                {
                    return Unauthorized("Token inválido: no se encontró el usuario.");
                }

                // Buscar las relaciones donde el usuario autenticado esté involucrado
                var relations = await _context.RelationUsers
                    .Include(r => r.User1)
                    .Include(r => r.User2)
                    .Include(r => r.DeviceUserRelation1)
                    .Include(r => r.DeviceUserRelation2)
                    .Where(r => r.UserId1 == loggedInUserId || r.UserId2 == loggedInUserId)
                    .ToListAsync();

                if (relations == null || !relations.Any())
                {
                    return NotFound($"No se encontraron relaciones para el usuario con ID '{loggedInUserId}'.");
                }

                // Mapear las relaciones al DTO personalizado
                var response = relations.Select(relation => new DeviceUserRelationResponse
                {
                    Id = relation.Id,
                    Conexion = relation.UserId1 == loggedInUserId ? relation.User2.Name : relation.User1.Name,
                    Dispositivo = relation.DeviceUserRelation1?.Device?.Name ?? "sunshine",
                    DispositivoRelacionado = relation.DeviceUserRelation2?.Device?.Name ?? "Desconocido",
                    Description = $"Relación entre {relation.User1.Name} y {relation.User2.Name}",
                    Publish = $"{relation.Id}/{relation.User2.Name}",
                    Subcribe = $"{relation.Id}/{relation.User1.Name}",
                    Status = true,
                    Tipo = "Lampara test"
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
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
        private bool RelationUserExists(Guid id)
        {
            return _context.RelationUsers.Any(r => r.Id == id);
        }

        // DTO para la respuesta personalizada
        public class DeviceUserRelationResponse
        {
            public Guid Id { get; set; }
            public string Conexion { get; set; }
            public string Dispositivo { get; set; }
            public string DispositivoRelacionado { get; set; }
            public string Description { get; set; }
            public string Publish { get; set; }
            public string Subcribe { get; set; }
            public bool Status { get; set; }
            public string Tipo { get; set; }
        }
    }
}