using ManejoPresupuesto.Controllers;
using ManejoPresupuesto.Views.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta : IValidatableObject
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "El campo Nombre es requerido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //[StringLength(maximumLength:50, MinimumLength =3, ErrorMessage = "La longitud del campo {0} debe de estar entre {2} y {1}")]
        //[Display(Name = "Nombre del tipo de cuenta")]
        //[PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Nombre is not null && Nombre.Length > 0)
            {
                var primeraLetra = Nombre[0].ToString();

                if(primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe de ser mayúscula", 
                        new[] { nameof(Nombre)});
                }
            }
        }

        /*Pruebas de otras validaciones por defecto*/

        //[Required(ErrorMessage = "El campo {0} es requerido")]
        //[EmailAddress(ErrorMessage = "El campo debe de ser un correo válido")]
        //public string Email { get; set; }
        //[Range(minimum:18, maximum:120, ErrorMessage = "El valor debe de estar entre {1} y {2}")]
        //public int Edad { get; set; }
        //[Url(ErrorMessage = "El valor debe de ser una URL válida.")]
        //public string URL { get; set; }
        //[CreditCard(ErrorMessage = "La tarjeta de crédito no es válida.")]
        //[Display(Name = "Tarjeta de crédito")]
        //public string CreditCard { get; set; }


    }
}
