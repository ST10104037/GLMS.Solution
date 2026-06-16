using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GLMS.API.Data;
using GLMS.API.Models;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.ServiceRequests
                .Include(s => s.Contract)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sr = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sr == null) return NotFound();
            return Ok(sr);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequest serviceRequest)
        {
            var contract = await _context.Contracts.FindAsync(serviceRequest.ContractId);

            if (contract == null)
                return BadRequest("Contract not found.");

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
                return BadRequest($"Cannot create request. Contract is {contract.Status}.");

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = serviceRequest.Id }, serviceRequest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sr = await _context.ServiceRequests.FindAsync(id);
            if (sr == null) return NotFound();
            _context.ServiceRequests.Remove(sr);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}