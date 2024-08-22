using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsuranceApi.Data;
using InsuranceApi.Models;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurosController : ControllerBase
    {
        private readonly InsuranceContext _context;

        public SegurosController(InsuranceContext context)
        {
            _context = context;
        }

        // Endpoint para Crear un Seguro
        [HttpPost]
        public async Task<ActionResult<Seguro>> CreateSeguro(Seguro seguro)
        {
            _context.Seguros.Add(seguro);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSeguroById), new { id = seguro.NumeroIdentificacion }, seguro);
        }

        // Endpoint para Obtener Seguros con Paginación
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seguro>>> GetSeguros(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Seguros
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        // Endpoint para Obtener un Seguro por Número de Identificación
        [HttpGet("{id}")]
        public async Task<ActionResult<Seguro>> GetSeguroById(int id)
        {
            var seguro = await _context.Seguros.FindAsync(id);
            if (seguro == null)
            {
                return NotFound();
            }
            return seguro;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeguro(int id, Seguro seguro)
        {
            if (id != seguro.NumeroIdentificacion)
            {
                return BadRequest("El ID en la URL no coincide con el ID del seguro.");
            }

            var seguroExistente = await _context.Seguros.FindAsync(id);
            if (seguroExistente == null)
            {
                return NotFound("Seguro no encontrado.");
            }

            // Actualizar los campos del seguro existente
            seguroExistente.PrimerNombre = seguro.PrimerNombre;
            seguroExistente.SegundoNombre = seguro.SegundoNombre;
            seguroExistente.PrimerApellido = seguro.PrimerApellido;
            seguroExistente.SegundoApellido = seguro.SegundoApellido;
            seguroExistente.TelefonoContacto = seguro.TelefonoContacto;
            seguroExistente.CorreoElectronico = seguro.CorreoElectronico;
            seguroExistente.FechaNacimiento = seguro.FechaNacimiento;
            seguroExistente.ValorEstimadoSeguro = seguro.ValorEstimadoSeguro;
            seguroExistente.Observaciones = seguro.Observaciones;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SeguroExists(id))
                {
                    return NotFound("Seguro no encontrado.");
                }
                else
                {
                    // Log error details
                    Console.WriteLine($"Error de concurrencia: {ex.Message}");
                    throw;
                }
            }

            return NoContent();
        }


        // Endpoint para Eliminar un Seguro
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeguro(int id)
        {
            var seguro = await _context.Seguros.FindAsync(id);
            if (seguro == null)
            {
                return NotFound();
            }

            _context.Seguros.Remove(seguro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método para verificar si el Seguro existe
        private bool SeguroExists(int id)
        {
            return _context.Seguros.Any(e => e.NumeroIdentificacion == id);
        }
    }
}
