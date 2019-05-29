using System.ComponentModel.DataAnnotations;

namespace obiektowe
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal PriceNet { get; set; }
        public decimal PriceGross { get; set; }
        public int Amount { get; set; }

        public override string ToString()
        {
            return Name + " - " + PriceGross + " -> " + Amount;
        }
    }
}
