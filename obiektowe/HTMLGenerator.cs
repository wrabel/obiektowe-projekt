using BarcodeLib;
using IronPdf;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace obiektowe
{
    public class HTMLGenerator
    {
        public static void GenerujPlik(string path, Order zamowienie)
        {
            HtmlToPdf Renderer = new HtmlToPdf();
            Renderer.RenderHtmlAsPdf(GenerujHtml(zamowienie)).SaveAs(path);
        }
        private static string podajBarcode(string numer)
        {
            Barcode barcode = new Barcode()
            {
                IncludeLabel = false,
                Alignment = AlignmentPositions.CENTER,
                Width = 300,
                Height = 100,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            Image img = barcode.Encode(TYPE.CODE128C, numer);
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                ms.Position = 0;
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static string GenerujHtml(Order zamowienie)
        {
            string start = "<h2>Zmaowienie nr OLN" + zamowienie.Id + "</h2><br><table><tr><th>Nazwa</th><th>Cena Netto</th><th>Cena Brutto</th><th>Ilość</th><th>Koszt</th></tr>";
            decimal koszt = 0;
            foreach (var var in zamowienie.Products)
            {
                start += generujwiersz(var);
                koszt += var.Product.PriceGross;
            }
            string end = "</table><br>Całkowity koszt: " + koszt.ToString();

            return start + end + "<br> <br>Możliwość odbioru dnia: " + zamowienie.DeliverTime.ToShortDateString() + "r.<br>Pokaż kod obsłudze oraz przygotuj dokument uwierzytelniający <br> <img src=\"data:image/jpeg;base64," + podajBarcode(zamowienie.Id.ToString()) + "\" />";
        }

        private static string generujwiersz(OrderItem item)
        {
            string separator = "</td><td>";
            return "<tr><td>" + item.Product.Name + separator + item.Product.PriceNet + separator + item.Product.PriceGross + separator + item.Quantity + separator + item.Quantity * item.Product.PriceGross + "</td></tr>";
        }
    }
}
