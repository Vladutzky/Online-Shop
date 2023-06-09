using System.ComponentModel.DataAnnotations.Schema;

namespace productsApp.Models
{
    public class productOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        public int? ProductID { get; set; }
        public int? orderId { get; set; } 

        public virtual product? product { get; set; }   
        public virtual Order? order { get; set; }   

        public DateTime orderDate { get; set; }
    }
}
