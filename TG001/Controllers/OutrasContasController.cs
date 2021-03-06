﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TG001.Data;
using TG001.Models;

namespace TG001.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OutrasContasController : ControllerBase
    {
        private readonly ContasAPagarContext _context;

        public OutrasContasController(ContasAPagarContext context)
        {
            _context = context;
        }

        // GET: api/OutrasContas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OutraConta>>> GetOutrasContas()
        {
            return await _context.OutrasContas.Include(u=>u.Usuario).Include(f=>f.Fornecedor).ToListAsync();
        }

        // GET: api/OutrasContas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OutraConta>> GetOutraConta(int id)
        {
            OutraConta outraConta = await _context.OutrasContas.Include(o => o.Fornecedor).Include(b => b.Usuario).SingleOrDefaultAsync(o => o.ID == id);

            if (outraConta == null)
            {
                return NotFound();
            }

            return outraConta;
        }

        // PUT: api/OutrasContas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOutraConta(int id, OutraConta outraConta)
        {
            if (id != outraConta.ID)
            {
                return BadRequest();
            }

            _context.Entry(outraConta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OutraContaExists(id))
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

        // POST: api/OutrasContas
        [HttpPost]
        public async Task<ActionResult<OutraConta>> PostOutraConta(OutraConta outraConta)
        {
            outraConta.DataCriacao = DateTime.Now;
            _context.OutrasContas.Add(outraConta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOutraConta", new { id = outraConta.ID }, outraConta);
        }

        // DELETE: api/OutrasContas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OutraConta>> DeleteOutraConta(int id)
        {
            OutraConta outraConta = await _context.OutrasContas.FindAsync(id);
            if (outraConta == null)
            {
                return NotFound();
            }

            _context.OutrasContas.Remove(outraConta);
            await _context.SaveChangesAsync();

            return outraConta;
        }

        private bool OutraContaExists(int id)
        {
            return _context.OutrasContas.Any(e => e.ID == id);
        }
    }
}
