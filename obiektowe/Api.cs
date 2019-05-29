using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace obiektowe
{
    public class Api
    {
        // konfiguracja
        private const string clientID = "161508456889-r3b771b8e5tc630v27bqpj042lko2ki5.apps.googleusercontent.com";
        private const string clientSecret = "47U7L4xdkLqzhtnqIqtn2Ya_";
        private const string URL_autoryzacja = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string URL_token = "https://www.googleapis.com/oauth2/v4/token";
        private const string URL_info = "https://www.googleapis.com/oauth2/v3/userinfo";
        private MainWindow window;
        private User uzytkownik { set; get; }

        public Api(MainWindow window)
        { 
            this.window = window;
            this.uzytkownik = null;
        }

        public User podajUzytkownika() { return this.uzytkownik; }


        // zrodlo http://stackoverflow.com/a/3978040
        // tworzenie url z wolnym portem
        private static int GetFreePort()
        {
            var sluchacz = new TcpListener(IPAddress.Loopback, 0);
            sluchacz.Start();
            var port = ((IPEndPoint)sluchacz.LocalEndpoint).Port;
            sluchacz.Stop();
            return port;
        }

        public  async void Autoryzuj() //dzialanie api
        {
            string stan = randomBase64url(32);
            string weryfikator = randomBase64url(32);
            string weryfikator2 = prettyBase64(sha256(weryfikator));
            const string metoda = "S256";

            // tworzenie url adresu zwrotnego
            string petla = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetFreePort());

            // nasluchiwanie tego url
            var sluchacz = new HttpListener();
            sluchacz.Prefixes.Add(petla); 
            sluchacz.Start();

            // tworzenie autoryzacji oauth 2.0
            // inspiracja https://www.codeproject.com/Articles/1185880/ASP-NET-Core-WebAPI-secured-using-OAuth-Client-Cre 
            string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                URL_autoryzacja,
                Uri.EscapeDataString(petla),
                clientID,
                stan,
                weryfikator2,
                metoda);

            // otowrzenie 'autoryzacji' w przegladarce
            System.Diagnostics.Process.Start(authorizationRequest);
            var context = await sluchacz.GetContextAsync();
            window.Activate();

            // wyslanie odpowiedzi do przegladarki na url adresu zwrotnego 
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='5;url=https://google.com'></head><body><p>Autoryzacja potwierdzona - wróć do aplikacji.</p?</body></html>");
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                sluchacz.Stop(); // zatrzymanie strony adresu zwrotnego
            });

            // wyciagniecie kodu z odpowiedzi
            var code = context.Request.QueryString.Get("code");
            var nowy_status = context.Request.QueryString.Get("state");

            wykonanie_zapytania(code, weryfikator, petla);
        }

        private async void wykonanie_zapytania(string code, string weryfikator, string petla)
        {
            // inspiracja https://stackoverflow.com/questions/4015324/how-to-make-http-post-web-request
            // budowanie requesta 
            string tokenBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                Uri.EscapeDataString(petla),
                clientID,
                weryfikator,
                clientSecret
                );
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(URL_token);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenBody);
            tokenRequest.ContentLength = _byteVersion.Length;

            // wyslanie
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // odpowiedz
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    string responseText = await reader.ReadToEndAsync();
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    pobierz_dane_uzutkownika(access_token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // https://developers.google.com/apis-explorer/#p/
        private async void pobierz_dane_uzutkownika(string access_token)
        {
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(URL_info);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                this.uzytkownik = User.Nowy_Uzytkownik(userinfoResponseText);
                window.Sukces();
            }
        }

        // metody pomocne ze strony https://stackoverflow.com
        private static string randomBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return prettyBase64(bytes);
        }

        private static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        private static string prettyBase64(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            base64 = base64.Replace("=", "");

            return base64;
        }

    }
}
