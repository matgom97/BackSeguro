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
            // Realizamos una inyección de dependencias
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
            // Validación en la cual se valida el id que se esta suministrando con el id almacenado en la BD
            // de no ser asi se enviara un mensaje de bad request en la cual se informa que el id no coincide 
            // con el id almacenado
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

            // Try catch para almacenar de haber un error nos mostrara dos resultados
            // puede ser que no se encuentre el seguro o un error de la base de datos
            // sea conexion u otro factor
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
                    // Mediante este error podemos conocer los detalles del fallo
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
            // Validamos que el seguro exista, para esto se hace la consulta a la BD
            // y en caso de ser null dejara un mensaje de no encontrado

            var seguro = await _context.Seguros.FindAsync(id);
            if (seguro == null)
            {
                return NotFound();
            }

            // Si existe el seguro en este caso el (id) se procede a eliminar el registro 
            // utilizando el metodo remove y posteriormente se guardan los cambios
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
