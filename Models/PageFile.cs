using Newtonsoft.Json;

namespace CASIO_HariKrishna.Models
{
    public class PageFile
    {
        public string FileName { get; set; }

        public string ObjectName { get; set; }

    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
