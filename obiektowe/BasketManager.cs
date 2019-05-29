using System;
using System.Linq;

namespace obiektowe
{
    public class BasketManager
    {
        private readonly ContextDB _context;
        public Basket Basket { get; private set; }

        public bool stworzono { get; set; }

        public BasketManager(ContextDB context)
        {
            _context = context;
            Basket = new Basket();
            stworzono = false;
        }

        public void AddBasketItem(BasketItem item)
        {
            if (Basket.Products.Any(x=>x.Product == item.Product))
            {
                Basket.Products.First(x => x.Product == item.Product).Quantity++;
            }
            else
            {
                Basket.Products.Add(item);
            }
            Basket.ValueGross += item.Product.PriceGross;
            Basket.ValueNet += item.Product.PriceNet;
            _context.Products.First(x => x.Id == item.Product.Id).Amount--;
            save();
        }

        public bool CheckAvailable(Product product)
        {
            return _context.Products.Any(dbProduct => dbProduct.Id == product.Id && dbProduct.Amount > 0);
        }

        public Order CreateOrder(User author)
        {
            //AddAuthor(author);
            var userdb = _context.Users.FirstOrDefault(x => x.sub == author.sub);
            if (userdb == null)
            {
                _context.Users.Add(author);
                save();
                userdb = _context.Users.First(x => x.sub == author.sub);
            }
            Order order = new Order()
            {
                Author = userdb,
                CreationTime = DateTime.UtcNow,
                DeliverTime = DateTime.UtcNow.AddDays(5),
                Products = Basket.Products.Select(product => new OrderItem() { Product = product.Product, Quantity = product.Quantity }).ToList(),
                Status = OrderStatus.New
            };

            _context.Orders.Add(order);
            save();

            this.stworzono = true;
            return order;
        }

        public void removeBasketItem(BasketItem basketItem)
        {
            if (Basket.Products.First(x => x.Product == basketItem.Product).Quantity > 1)
            {
                Basket.Products.First(x => x.Product == basketItem.Product).Quantity--;
            }
            else
            {
                Basket.Products.Remove(basketItem);
            }
            
            Basket.ValueGross -= basketItem.Product.PriceGross;
            Basket.ValueNet -= basketItem.Product.PriceNet;
            _context.Products.First(x => x.Id == basketItem.Product.Id).Amount++;
            save();
            
        }

        public Basket addBasketFromOrder(int number)
        {
            Order order = _context.Orders.Where(dbOrder => dbOrder.Number == number).FirstOrDefault();
            Basket basket = new Basket();
            basket.Products = order.Products.Select(product => new BasketItem() { Product = product.Product, Quantity = product.Quantity }).ToList();
            return basket;
        }

        private void save()
        {
            _context.SaveChanges();
        }

        public void removeBasketItems()
        {
            foreach(var var in Basket.Products)
            {
                _context.Products.First(x => x.Id == var.Product.Id).Amount += var.Quantity;

            }
            save();
        }
    }
}
