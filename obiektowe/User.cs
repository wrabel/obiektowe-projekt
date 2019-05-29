using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace obiektowe
{
    public class User // clasa user wygenerowana za pomoca http://json2csharp.com -> json dostarczony przez google
    {
        [Key]
        public string sub { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string profile { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public bool isAdmin => sub == "102111755522341138310";

        public ICollection<Order> Orders { get; set; }

        public static User Nowy_Uzytkownik(string json)
        {
            User uzytkownik = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(json);
            return uzytkownik;
        }

        public override string ToString()
        {
            return this.name + " " + this.sub; 
        }
    }
}
