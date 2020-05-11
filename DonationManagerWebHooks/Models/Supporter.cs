using Newtonsoft.Json;

namespace DonationManagerWebHooks.Models
{
    public partial class SupporterHeader
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(string))]
        public string id { get; set; }


        [JsonProperty("object")]
        public string objectSupporter { get; set; }

        [JsonProperty("event")]
        public string eventSupporter { get; set; }

        [JsonProperty("updated")]
        public string updated { get; set; }

        [JsonProperty("supporter")]
        public Supporter supporter { get; set; }
    }

    public partial class Supporter
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(string))]
        public string id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("firstname")]
        public string firstname { get; set; }

        [JsonProperty("surname")]
        public string surname { get; set; }

        [JsonProperty("address1")]
        [JsonConverter(typeof(string))]
        public string address1 { get; set; }

        [JsonProperty("address2")]
        public string address2 { get; set; }

        [JsonProperty("town")]
        public string town { get; set; }

        [JsonProperty("county")]
        public string county { get; set; }

        [JsonProperty("postcode")]
        public string postcode { get; set; }

        [JsonProperty("country")]
        public string country { get; set; }

        [JsonProperty("telephone")]
        public string telephone { get; set; }

        [JsonProperty("fax")]
        public string fax { get; set; }

        [JsonProperty("mobile")]
        public string mobile { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("mailinglist")]
        [JsonConverter(typeof(string))]
        public string mailinglist { get; set; }

        [JsonProperty("deleted")]
        [JsonConverter(typeof(string))]
        public string deleted { get; set; }
    }
}