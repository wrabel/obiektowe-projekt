using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace obiektowe
{
    //klasy basket basketitem basketmenager order orderitem ordermenager oraz product zmodyfikowane z cwiczen 2. zadania 3
    public partial class MainWindow : Window
    {
        private ContextDB _context;
        private User zalogowany;
        private Api api;
        private BasketManager basketManager;
        public MainWindow()
        {
            InitializeComponent();
            api = new Api(this);
            this.zalogowany = null;
            this._context = new ContextDB();
            taskNiezalogowany();
        }


        private void Autoryzuj(object sender, RoutedEventArgs e)
        {
            api.Autoryzuj();
        }

        public void Sukces()
        {
            this.zalogowany = api.podajUzytkownika();
            if (this.zalogowany != null) taskZalogowany();
        }

        private void taskNiezalogowany()
        {
            btn_zaloguj.Visibility = Visibility.Visible;
            btn_zamowienie.Visibility = Visibility.Hidden;
            koszyk.Visibility = Visibility.Hidden;
            produkty.Visibility = Visibility.Hidden;
            btn_admin.Visibility = Visibility.Hidden;
            text_welcom.Visibility = Visibility.Hidden;
            kwota.Visibility = Visibility.Hidden;
        }
        private void taskZalogowany()
        {
            text_welcom.Text = "Witaj " + this.zalogowany.given_name + "!";
            basketManager = new BasketManager(_context);
            odswiezProdukty();



            text_welcom.Visibility = Visibility.Visible;
            btn_zaloguj.Visibility = Visibility.Hidden;
            btn_zamowienie.Visibility = Visibility.Visible;
            koszyk.Visibility = Visibility.Visible;
            produkty.Visibility = Visibility.Visible;
            kwota.Visibility = Visibility.Visible;
            btn_admin.Visibility = zalogowany.isAdmin ? Visibility.Visible : Visibility.Hidden;
        }

        private void Btn_admin_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new WindowAdmin(_context,this);
            adminWindow.Show();

        }

        private void Produkty_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = produkty.SelectedItem as Product;
            if (item != null)
            {
                basketManager.AddBasketItem(new BasketItem() { Product = item, Quantity = 1 });

            }
            odswiezProdukty();
            przeladujKoszyk();
        }

        private void przeladujKoszyk()
        {
            koszyk.ItemsSource = basketManager.Basket.Products;
            koszyk.Items.Refresh();
            kwota.Text = "Łączna kwota: " + basketManager.Basket.ValueGross;

        }

        private void Koszyk_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = koszyk.SelectedItem as BasketItem;

            if (item != null)
            {
                basketManager.removeBasketItem(item);
            }
            przeladujKoszyk();
            odswiezProdukty();
        }

        private void Btn_zamowienie_Click(object sender, RoutedEventArgs e)
        {
            var zamowienie = basketManager.CreateOrder(zalogowany);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                HTMLGenerator.GenerujPlik(saveFileDialog.FileName, zamowienie);
                MessageBox.Show("Złożono zamówienie");
                basketManager = new BasketManager(_context);
                odswiezProdukty();
                przeladujKoszyk();
            }
        }

        public void odswiezProdukty()
        {
            produkty.ItemsSource = _context.Products.Where(x => x.Amount > 0).ToList();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (basketManager != null && !basketManager.stworzono)
            {
                basketManager.removeBasketItems();
            }
            base.OnClosed(e);
        }

        


    }
       
}
