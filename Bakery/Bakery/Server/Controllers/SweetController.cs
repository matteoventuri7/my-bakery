using Bakery.Server.Data;
using Bakery.Shared.DataModel;
using Bakery.Shared.TransferModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SweetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SweetController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sweet>>> GetSweets()
        {
            return await _context.Sweets
                .Include(s => s.Recipe)
                .ThenInclude(s => s.Ingredient)
                .ToListAsync();
        }

        [HttpGet("selling")]
        public async Task<ActionResult<IEnumerable<Sweet>>> GetSellingSweets()
        {
            var res = await _context.Sweets
                .Include(s => s.Recipe)
                .ThenInclude(s => s.Ingredient)
                .Where(s => s.StartSellingDate.HasValue)
                .ToListAsync();

            return res.Where(x => x.IsEatable())
            .ToList();
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Sweet>> GetSweet(int id)
        {
            var sweet = await _context.Sweets
                .Include(s => s.Recipe)
                .ThenInclude(s => s.Ingredient)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (sweet == null)
            {
                return NotFound();
            }

            return sweet;
        }

        [Authorize]
        [HttpPut("withimage")]
        public async Task<IActionResult> PutSweet(SweetWithImage m)
        {
            _context.Entry(m.Sweet).State = EntityState.Modified;

            if (m.ImageFile is not null)
            {
                string oldImage = m.Sweet.ImageFileName;
                m.Sweet.ImageFileName = StoreFile(m.ImageFile);
                System.IO.File.Delete(Path.Combine("wwwroot", "sweet", oldImage));
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize]
        [HttpPut("simple")]
        public async Task<IActionResult> PutSweet(Sweet s)
        {
            _context.Entry(s).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Sweet>> PostSweet(SweetWithImage m)
        {
            m.Sweet.ImageFileName = StoreFile(m.ImageFile);
            _context.Sweets.Add(m.Sweet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSweet), new { id = m.Sweet.Id }, m.Sweet);
        }
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSweet(int id)
        {
            var sweet = await _context.Sweets.FindAsync(id);
            if (sweet == null)
            {
                return NotFound();
            }

            _context.Sweets.Remove(sweet);
            await _context.SaveChangesAsync();

            System.IO.File.Delete(Path.Combine("wwwroot", "sweet", sweet.ImageFileName));

            return NoContent();
        }

        private bool SweetExists(int id)
        {
            return _context.Sweets.Any(e => e.Id == id);
        }

        private string StoreFile(UploadedFile uploadedFile)
        {
            if (uploadedFile is null) return null;

            var pathDir = Path.Combine("wwwroot", "sweet");
            Directory.CreateDirectory(pathDir);
            var fullPath = Path.Combine(pathDir, uploadedFile.FileName);
            int i = 0;
            while (System.IO.File.Exists(fullPath))
            {
                fullPath = Path.Combine(pathDir, ++i + uploadedFile.FileName);
            }
            using var fs = System.IO.File.Create(fullPath);
            fs.Write(uploadedFile.FileContent, 0, uploadedFile.FileContent.Length);

            return uploadedFile.FileName;
        }
    }
}
