using Newtonsoft.Json;

namespace CASIO_HariKrishna.Models
{
    public class UserDetailModel
    {
        public UserDetailModel()
        {

        }

        [JsonProperty("name")]
        public string UserName { get; set; }
    }
}
