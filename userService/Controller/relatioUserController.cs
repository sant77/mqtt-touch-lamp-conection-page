using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using userService.Data;
using userService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


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
        public async Task<ActionResult<IEnumerable<RelationUser>>> GetRelationUsers()
        {
            return await _context.RelationUsers
                .Include(r => r.User1)
                .Include(r => r.User2)
                .Include(r => r.DeviceUserRelation1)
                .Include(r => r.DeviceUserRelation2)
                .ToListAsync();
        }

        // GET: api/RelationUser/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RelationUser>> GetRelationUser(Guid id)
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

            return relationUser;
        }

        // POST: api/RelationUser
        [HttpPost]
        public async Task<ActionResult<RelationUser>> CreateRelationUser(RelationUser relationUser)
        {
            if (relationUser.UserId1 == relationUser.UserId2)
            {
                return BadRequest("User1 and User2 cannot be the same.");
            }
            relationUser.Id = Guid.NewGuid();
            _context.RelationUsers.Add(relationUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRelationUser", new { id = relationUser.Id }, relationUser);
        }

        // PUT: api/RelationUser/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRelationUser(Guid id, RelationUser relationUser)
        {
            if (id != relationUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(relationUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            return NoContent();
        }

        // DELETE: api/RelationUser/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRelationUser(Guid id)
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

        private bool RelationUserExists(Guid id)
        {
            return _context.RelationUsers.Any(r => r.Id == id);
        }

    [Authorize] // Asegura que el usuario esté autenticado
    [HttpPost("create-relation")]
    public async Task<ActionResult<RelationUser>> CreateUserRelation([FromBody] UserRelationRequest request)
    {
        // 1. Obtener el ID del usuario autenticado desde el JWT
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(nameIdentifier) || !Guid.TryParse(nameIdentifier, out Guid loggedInUserId))
        {
            return Unauthorized("Usuario no autenticado o token inválido.");
        }

        // 2. Buscar al usuario que se desea vincular (usando el correo electrónico)
        var userToLink = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.user);
        if (userToLink == null)
        {
            return NotFound($"No se encontró un usuario con el correo electrónico '{request.user}'.");
        }

        // 3. Verificar que el usuario autenticado no esté intentando vincularse a sí mismo
        if (loggedInUserId == userToLink.Id)
        {
            return BadRequest("No puedes vincularte a ti mismo.");
        }


        // 5. Crear la relación entre usuarios
        var relationUser = new RelationUser
        {
            Id = Guid.NewGuid(),
            UserId1 = loggedInUserId, // Usuario autenticado
            UserId2 = userToLink.Id, // Usuario a vincular
            DeviceUserRelationId1 = Guid.Parse(request.deviceOwnerId), // Dispositivo del usuario autenticado
            DeviceUserRelationId2 = Guid.Parse(request.deviceId) // Dispositivo del usuario a vincular
        };

        // 6. Guardar la relación en la base de datos
        _context.RelationUsers.Add(relationUser);
        await _context.SaveChangesAsync();

        // 7. Devolver la relación creada
        return CreatedAtAction(nameof(GetRelationUser), new { id = relationUser.Id }, relationUser);
    }

   
    public class UserRelationRequest
    {
        public string device { get; set; }
        public string deviceId { get; set; }
        public string deviceOwner { get; set; }
        public string deviceOwnerId { get; set; }
        public string user { get; set; }
    }
    [Authorize]
    [HttpGet("by-user")]
    public async Task<ActionResult<IEnumerable<DeviceUserRelationResponse>>> GetUsersRelationsByUserId()
    {
        // 1. Obtener el ID del usuario autenticado desde el token
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(nameIdentifier) || !Guid.TryParse(nameIdentifier, out Guid loggedInUserId))
        {
            return Unauthorized("Token inválido: no se encontró el usuario.");
        }

        // 2. Buscar las relaciones donde el usuario autenticado esté involucrado
        var relations = await _context.RelationUsers
            .Include(r => r.User1) // Incluir el usuario 1
            .Include(r => r.User2) // Incluir el usuario 2
            .Include(r => r.DeviceUserRelation1) // Incluir el dispositivo 1
            .Include(r => r.DeviceUserRelation2) // Incluir el dispositivo 2
            .Where(r => r.UserId1 == loggedInUserId || r.UserId2 == loggedInUserId) // Filtrar por el usuario autenticado
            .ToListAsync();

        if (relations == null || !relations.Any())
        {
            return NotFound($"No se encontraron relaciones para el usuario con ID '{loggedInUserId}'.");
        }

        // 3. Mapear las relaciones al DTO personalizado
        var response = relations.Select(relation => new DeviceUserRelationResponse
        {
            Id = relation.Id,
            Conexion = relation.UserId1 == loggedInUserId ? relation.User2.Name : relation.User1.Name, // Nombre del usuario relacionado
            Dispositivo = relation.DeviceUserRelation1?.Device?.Name ?? "sunshine", // Nombre del dispositivo 1
            DispositivoRelacionado = relation.DeviceUserRelation2?.Device?.Name ?? "Desconocido", // Nombre del dispositivo 2
            Description = $"Relación entre {relation.User1.Name} y {relation.User2.Name}", // Descripción personalizada
            publish = $"{relation.Id}/{relation.User2.Name}",
            subcribe = $"{relation.Id}/{relation.User1.Name}",
            Status = true,
            Tipo = "Lampara test"
        }).ToList();

        return Ok(response);
    }

        public class DeviceUserRelationResponse
    {
        public Guid Id { get; set; }
        public string Conexion { get; set; }
        public string Dispositivo { get; set; }
        public string DispositivoRelacionado {get; set;}
        public string Description { get; set; } // Campo adicional
        public string publish { get; set; }
        public string subcribe { get; set; }
        public bool Status { get; set; }
        public string Tipo { get; set; }

    }
    }

}
