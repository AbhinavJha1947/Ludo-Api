using LudoGameApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LudoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestController : ControllerBase
    {
        private readonly LudoDbContext _context;

        public ContestController(LudoDbContext context)
        {
            _context = context;
        }

        // GET: api/Contest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contest>>> GetContests()
        {
            return await _context.Contests.ToListAsync();
        }

        // GET: api/Contest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contest>> GetContest(int id)
        {
            var contest = await _context.Contests.FindAsync(id);

            if (contest == null)
            {
                return NotFound();
            }

            return contest;
        }

        // PUT: api/Contest/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContest(int id, [FromBody] Contest contest)
        {
            if (id != contest.Id)
            {
                return BadRequest("Contest ID mismatch.");
            }

            _context.Entry(contest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContestExists(id))
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

        // POST: api/Contest
        [HttpPost]
        public async Task<ActionResult<Contest>> CreateContest([FromBody] Contest contest)
        {
            _context.Contests.Add(contest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContest), new { id = contest.Id }, contest);
        }

        // DELETE: api/Contest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContest(int id)
        {
            var contest = await _context.Contests.FindAsync(id);
            if (contest == null)
            {
                return NotFound();
            }

            _context.Contests.Remove(contest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContestExists(int id)
        {
            return _context.Contests.Any(e => e.Id == id);
        }
    }
}
