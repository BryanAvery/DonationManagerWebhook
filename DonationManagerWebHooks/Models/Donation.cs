using Newtonsoft.Json;

namespace DonationManagerWebHooks.Models
{
    public partial class DonationHeader
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string objectDonationManager { get; set; }

        [JsonProperty("event")]
        public string eventDonationManager { get; set; }

        [JsonProperty("created")]
        public string created { get; set; }

        [JsonProperty("donation")]
        public Donation donation { get; set; }

        [JsonProperty("supporter")]
        public Supporter supporter { get; set; }

        [JsonProperty("totalgivingpage")]
        public Totalgivingpage totalgivingpage { get; set; }

        [JsonProperty("appeal")]
        public Appeal appeal { get; set; }
    }

    public partial class Donation
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(string))]
        public string id { get; set; }

        [JsonProperty("amount")]
        [JsonConverter(typeof(string))]
        public string amount { get; set; }

        [JsonProperty("currency")]
        public string currency { get; set; }

        [JsonProperty("exchangerate")]
        public string exchangerate { get; set; }

        [JsonProperty("datetime")]
        public string datetime { get; set; }

        [JsonProperty("giftaid")]
        [JsonConverter(typeof(string))]
        public string giftaid { get; set; }

        [JsonProperty("displayname")]
        public string displayname { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("repeat")]
        public string repeat { get; set; }

        [JsonProperty("cancelled")]
        [JsonConverter(typeof(string))]
        public string cancelled { get; set; }
    }
}