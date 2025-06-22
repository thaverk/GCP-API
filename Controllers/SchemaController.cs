using Microsoft.AspNetCore.Mvc;
using PhasePlayWeb.Data;

namespace PhasePlayWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchemaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchemaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{schemaId}")]
        public IActionResult GetSchemaAttributes(int schemaId)
        {
            var schemaAttributes = _context.SchemaAttributes
                .Where(x => x.SchemaID == schemaId)
                .Select(x => new { x.Reps, x.PercentRM, x.RPE })
                .ToList();

            return Ok(schemaAttributes);
        }


    }
}
