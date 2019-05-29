using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace obiektowe
{
    public partial class WindowAdmin : Window
    {

        private ContextDB context;
        private MainWindow parent;
        public WindowAdmin(ContextDB context, MainWindow window)
        {
            InitializeComponent();
            this.context = context;
            this.parent = window;
            ZaladujProdukty();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            context.Products.Add(InputToProduct());
            context.SaveChanges();
            ZaladujProdukty();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var item = InputToProduct();
            Product prodToDEL = context.Products.Where(p => p.Id == item.Id).FirstOrDefault();

            if (prodToDEL != null)
            {
                context.Products.Remove(prodToDEL);
                context.SaveChanges();
            }
            ZaladujProdukty();
        }

        //inspiracja https://stackoverflow.com/questions/9591165/ef-4-how-to-properly-update-object-in-dbcontext-using-mvc-with-repository-patte
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var item = InputToProduct();
            Product prodToUpdate = context.Products.Where(p => p.Id == item.Id).FirstOrDefault();

            if (prodToUpdate != null)
            {
                context.Entry(prodToUpdate).CurrentValues.SetValues(item);
                context.SaveChanges();
            }
            ZaladujProdukty();
            
        }

        private void Produkty_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = produkty.SelectedItem as Product;
            if (item != null)
            {
                productToInput(item);
            }
        }

        private void ZaladujProdukty()
        {
            produkty.ItemsSource = this.context.Products.ToList();
        }

        //zrodlo https://social.msdn.microsoft.com/Forums/vstudio/en-US/758796b5-0020-4d14-941b-9320c817167a/wpf-textbox-to-accept-decimal-numbers?forum=wpf
        private void Net_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            {
                Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[,]{0,1}[0-9]{0,2}$");
                e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
            }
        }

        //zmodyfikowane to co wyzej
        private void Count_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void productToInput(Product pro)
        {
            id.Text = pro.Id.ToString();
            name.Text = pro.Name;
            gross.Text = pro.PriceGross.ToString();
            net.Text = pro.PriceNet.ToString();
            count.Text = pro.Amount.ToString();

        }
        private Product InputToProduct()
        {
            return new Product { Id= int.Parse(id.Text == String.Empty ? "0" : id.Text), Name = name.Text, PriceGross = Decimal.Parse(gross.Text), PriceNet = Decimal.Parse(net.Text), Amount = int.Parse(count.Text) };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            parent.odswiezProdukty();
        }
    }
}
