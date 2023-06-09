using System.ComponentModel.DataAnnotations;

namespace productsApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string CategoryName { get; set; }

        public virtual ICollection<product>? products { get; set; }
    }

}
