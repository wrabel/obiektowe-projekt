namespace obiektowe
{
    public class BasketItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public override string ToString()
        {
            return Product.Name + " " + Quantity + "x " + Product.PriceGross;
        }
    }
}
