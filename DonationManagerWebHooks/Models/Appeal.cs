using Newtonsoft.Json;

namespace DonationManagerWebHooks.Models
{

    public partial class AppealHeader
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(string))]
        public string id { get; set; }

        [JsonProperty("object")]
        public string objectAppeal { get; set; }

        [JsonProperty("event")]
        public string eventAppeal { get; set; }

        [JsonProperty("updated")]
        public string updated { get; set; }

        [JsonProperty("appeal")]
        public Appeal appeal { get; set; }
    }

    public partial class Appeal
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(string))]
        public string id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("deleted")]
        [JsonConverter(typeof(string))]
        public string deleted { get; set; }
    }
}