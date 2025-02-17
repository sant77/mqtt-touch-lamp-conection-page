using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using userService.Data;
using userService.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace userService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceUserRelationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeviceUserRelationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/DeviceUserRelation
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DeviceUserRelation>> CreateDeviceUserRelation([FromBody] DeviceUserRelationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Extraer el ID del usuario del token
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(nameIdentifier))
            return Unauthorized("Token inválido: no se encontró el usuario.");

        // Convertir el NameIdentifier (string) a Guid
        if (!Guid.TryParse(nameIdentifier, out Guid userId))
        {
            return BadRequest("El ID del usuario en el token no es un GUID válido.");
        }

        // Buscar el dispositivo por nombre
        var device = await _context.Devices.FirstOrDefaultAsync(d => d.Name == request.DeviceName);
        if (device == null)
            return NotFound($"Dispositivo con nombre '{request.DeviceName}' no encontrado.");

        // Crear la relación
        var relation = new DeviceUserRelation
        {
            Id = Guid.NewGuid(),
            UserId = userId, // Usar el Guid convertido
            DeviceId = device.Id
        };

        _context.DeviceUserRelations.Add(relation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeviceUserRelation), new { id = relation.Id }, relation);
    }

    public class DeviceUserRelationRequest
    {
        public string DeviceName { get; set; } // Nombre del dispositivo
    }


        // GET: api/DeviceUserRelation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceUserRelation>>> GetDeviceUserRelations()
        {
            return await _context.DeviceUserRelations
                .Include(du => du.User)
                .Include(du => du.Device)
                .ToListAsync();
        }

        // GET: api/DeviceUserRelation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceUserRelation>> GetDeviceUserRelation(Guid id)
        {
            var relation = await _context.DeviceUserRelations
                .Include(r => r.User)
                .Include(r => r.Device)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (relation == null)
                return NotFound();

            return relation;
        }


        // PUT: api/DeviceUserRelation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeviceUserRelation(Guid id, DeviceUserRelation relation)
        {
            if (id != relation.Id)
                return BadRequest();

            _context.Entry(relation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceUserRelationExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/DeviceUserRelation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceUserRelation(Guid id)
        {
            var relation = await _context.DeviceUserRelations.FindAsync(id);
            if (relation == null)
                return NotFound();

            _context.DeviceUserRelations.Remove(relation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceUserRelationExists(Guid id)
        {
            return _context.DeviceUserRelations.Any(e => e.Id == id);
        }


        
        [HttpGet("by-user")]
        public async Task<ActionResult<IEnumerable<DeviceUserRelationResponse>>> GetDeviceUserRelationsByUserId()
        {
            // Extraer el ID del usuario del token para verificar que el usuario solo acceda a sus propios datos
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier) || !Guid.TryParse(nameIdentifier, out Guid loggedInUserId))
            {
                return Unauthorized("Token inválido: no se encontró el usuario.");
            }

        
            // Buscar las relaciones por UserId
            var relations = await _context.DeviceUserRelations
                .Include(du => du.Device) // Incluir el dispositivo relacionado
                .Where(du => du.UserId == loggedInUserId)
                .ToListAsync();

            if (relations == null || !relations.Any())
            {
                return NotFound($"No se encontraron relaciones para el usuario con ID '{loggedInUserId}'.");
            }

            // Mapear las relaciones al DTO personalizado
            var response = relations.Select(relation => new DeviceUserRelationResponse
            {
                Id = relation.Id,
                Nombre = "sunshine",
                Dispositivo = relation.Device?.Name ?? "Desconocido", // Nombre del dispositivo
                Description = $"Relación entre el usuario {relation.UserId} y el dispositivo {relation.DeviceId}" // Descripción personalizada
            }).ToList();

            return Ok(response);
        }

        public class DeviceUserRelationResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Dispositivo { get; set; }
        public string Description { get; set; } // Campo adicional
    }

    // GET: api/DeviceUserRelation/by-email?email=usuario@example.com
    [HttpGet("by-email")]
    public async Task<ActionResult<IEnumerable<DeviceUserRelationResponse>>> GetDeviceUserRelationsByEmail(string email)
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
            .Include(du => du.Device) // Incluir el dispositivo relacionado
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
            Nombre = "sunshine", // Puedes personalizar este campo
            Dispositivo = relation.Device?.Name ?? "Desconocido", // Nombre del dispositivo
            Description = $"Relación entre el usuario {relation.UserId} y el dispositivo {relation.DeviceId}" // Descripción personalizada
        }).ToList();

        return Ok(response);
    }
    }

}
