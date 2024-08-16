using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thesis_backend.Data_Structures
{
    public abstract record DbElement
    {
        public long ID { get; set; }
        [JsonIgnore]
        public virtual object Serialize { get => JsonConvert.SerializeObject(this); }
    }
}