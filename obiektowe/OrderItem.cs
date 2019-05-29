using System.ComponentModel.DataAnnotations;

namespace obiektowe
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
