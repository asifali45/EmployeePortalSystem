using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;

namespace EmployeePortalSystem.Controllers
{
    [Route("api/[controller]")]
    public class AwardController : Controller  
    {
        private readonly AwardRepository _repository;

        public AwardController(AwardRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Award>>> GetAll()
        {
            var awards = await _repository.GetAllAsync();
            return Ok(awards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Award>> Get(int id)
        {
            var award = await _repository.GetByIdAsync(id);
            if (award == null)
                return NotFound();

            return Ok(award);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(Award award)
        {
            try
            {
                var newId = await _repository.CreateAsync(award);
                return Ok(newId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Insert failed: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Award award)
        {
            award.AwardId = id;
            var updated = await _repository.UpdateAsync(award);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("/Award/award")]
        public IActionResult award ()
        {
            return View();
        }
    }
}
