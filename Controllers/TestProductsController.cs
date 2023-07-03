using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestEntities.Contexts;
using TestEntities.Models.Entities;

namespace EfCrudAuthorizationAuthentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestProductsController : ControllerBase
    {
        private readonly TestEntitiesDbContext _context;

        public TestProductsController(TestEntitiesDbContext context)
        {
            _context = context;
        }

        // GET: api/TestProducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestProducts>>> GetTestProducts()
        {
          if (_context.TestProducts == null)
          {
              return NotFound();
          }
            return await _context.TestProducts.ToListAsync();
        }

        // GET: api/TestProducts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestProducts>> GetTestProducts(long id)
        {
          if (_context.TestProducts == null)
          {
              return NotFound();
          }
            var testProducts = await _context.TestProducts.FindAsync(id);

            if (testProducts == null)
            {
                return NotFound();
            }

            return testProducts;
        }

        // PUT: api/TestProducts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestProducts(long id, TestProducts testProducts)
        {
            if (id != testProducts.Id)
            {
                return BadRequest();
            }

            _context.Entry(testProducts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestProductsExists(id))
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

        // POST: api/TestProducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TestProducts>> PostTestProducts(TestProducts testProducts)
        {
          if (_context.TestProducts == null)
          {
              return Problem("Entity set 'TestEntitiesDbContext.TestProducts'  is null.");
          }
            _context.TestProducts.Add(testProducts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestProducts", new { id = testProducts.Id }, testProducts);
        }

        // DELETE: api/TestProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestProducts(long id)
        {
            if (_context.TestProducts == null)
            {
                return NotFound();
            }
            var testProducts = await _context.TestProducts.FindAsync(id);
            if (testProducts == null)
            {
                return NotFound();
            }

            _context.TestProducts.Remove(testProducts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestProductsExists(long id)
        {
            return (_context.TestProducts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
