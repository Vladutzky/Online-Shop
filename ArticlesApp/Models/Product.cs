using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace productsApp.Models
{
    public class product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Poza este obligatorie")]
       
        public string Poza { get; set; }

        [Required(ErrorMessage = "Stelele sunt obligatorii")]
        [Range(1, 5, ErrorMessage = "Stelele trebuie să fie între 1 și 5")]
        public int Stele { get; set; }

        [Required(ErrorMessage = "Pretul este obligatoriu")]
        [Range(0, int.MaxValue, ErrorMessage = "Pretul trebuie sa fie cel putin 0")]
        public int Pret { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }

        public string? UserId { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }

        public virtual Category? Category { get; set; }

        public virtual ICollection<review>? reviews { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

        public virtual ICollection<productOrder>? productorders { get; set; }
    }

}


