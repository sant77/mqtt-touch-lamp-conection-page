using Microsoft.AspNetCore.Mvc;
using userService.Data;
using userService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;


namespace userService.Controllers
{
    [ApiController]
    [Route("userservice/v1/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
       
        public DeviceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /userservice/v1/usuarios
        [HttpGet]
        public async Task<IActionResult> GetDevice()
        {
            var devices = await _context.Devices.ToListAsync();
            return Ok(devices);
        }      
    }
}