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
    public class ContractsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] ContractStatus? status)
        {
            var query = _context.Contracts.Include(c => c.Client).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(c => c.StartDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(c => c.EndDate <= endDate.Value);
            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null) return NotFound();
            return Ok(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ContractStatus newStatus)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            contract.Status = newStatus;
            await _context.SaveChangesAsync();
            return Ok(contract);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}