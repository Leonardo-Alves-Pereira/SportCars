using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportCar.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage ="O campo {0} é obrigatório")]
        [StringLength(40, ErrorMessage ="O campo {0} permite no maximo 40 letras")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(15, ErrorMessage = "O campo {0} permite no maximo 15 letras")]
        public string CPF { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Telefone { get; set; }
        public string Foto { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(40, ErrorMessage = "O campo {0} permite no maximo 40 letras")]
        [EmailAddress(ErrorMessage ="O Formato do Email é invalido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Password)]
        [Display(Name ="Confirme sua Senha")]
        [Compare("Senha", ErrorMessage ="As senhas não conferem")]
        public string SenhaConfirmada { get; set; }
    }
}
