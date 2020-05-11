using Newtonsoft.Json;
using System;

namespace DonationManagerWebHooks.Models
{
    public partial class TotalGivingPageHeader
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(string))]
        public string id { get; set; }

        [JsonProperty("object")]
        public string objectTotalGivingr { get; set; }

        [JsonProperty("event")]
        public string eventTotalGiving { get; set; }

        [JsonProperty("updated")]
        public string updated { get; set; }

        [JsonProperty("totalgivingpage")]
        public string totalgivingpage { get; set; }

        [JsonProperty("supporter")]
        public Supporter supporter { get; set; }

        [JsonProperty("appeal")]
        public Appeal appeal { get; set; }
    }

    public partial class Totalgivingpage
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(string))]
        public string id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("url")]
        public Uri url { get; set; }

        [JsonProperty("showpage")]
        [JsonConverter(typeof(string))]
        public string showpage { get; set; }

        [JsonProperty("allowdonations")]
        [JsonConverter(typeof(string))]
        public string allowdonations { get; set; }

        [JsonProperty("deleted")]
        [JsonConverter(typeof(string))]
        public string deleted { get; set; }
    }
}