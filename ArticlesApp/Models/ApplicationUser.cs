using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;


// PASUL 1 - useri si roluri 
namespace productsApp.Models 
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<review>? reviews { get; set; }

        public virtual ICollection<product>? products { get; set; }

        public virtual ICollection<Order>? orders { get; set; }


        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }  

    }
}
