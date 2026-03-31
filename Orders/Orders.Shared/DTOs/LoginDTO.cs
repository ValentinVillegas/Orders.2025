using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Shared.DTOs;

public class LoginDTO
{
    [Display(Name = "Correo electrónico")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "El campo {0} es requerido.")]
    [MinLength(6, ErrorMessage = "El campo {0} debe tener al menos {1} caracteres.")]
    public string Password { get; set; } = null!;
}