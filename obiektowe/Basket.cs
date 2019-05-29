using System.Collections.Generic;

namespace obiektowe
{
    public class Basket
    {
        public List<BasketItem> Products { get; set; }
        public decimal ValueNet { get; set; }
        public decimal ValueGross { get; set; }

        public Basket()
        {
            Products = new List<BasketItem>();
            ValueNet = 0;
            ValueGross = 0;
        }
    }
}
