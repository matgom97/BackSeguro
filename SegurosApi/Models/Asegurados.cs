using System;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.Models
{
    public class Seguro
    {
        [Key]
        public int NumeroIdentificacion { get; set; }

        [Required]
        public required string PrimerNombre { get; set; }

        public required string SegundoNombre { get; set; }

        [Required]
        public required string PrimerApellido { get; set; }

        [Required]
        public required string SegundoApellido { get; set; }

        [Required]
        public required string TelefonoContacto { get; set; }

        [Required]
        [EmailAddress]
        public required string CorreoElectronico { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal ValorEstimadoSeguro { get; set; }

        public required string Observaciones { get; set; }
    }
}
