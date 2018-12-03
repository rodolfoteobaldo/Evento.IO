using System;
using System.ComponentModel.DataAnnotations;

namespace Eventos.IO.Application.ViewModels
{
  public class OrganizadorViewModel
  {
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O nome é requirido")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O CPF é requirido")]
    [StringLength(11)]
    public string CPF { get; set; }

    [Required(ErrorMessage = "O e-mail é requirido")]
    [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
    public string Email { get; set; }
  }
}
