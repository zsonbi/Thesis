using Newtonsoft.Json;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// An abstarct database element
    /// </summary>
    public abstract record DbElement
    {
        public long ID { get; set; }
        [JsonIgnore]
        public virtual object Serialize { get => JsonConvert.SerializeObject(this); }
    }
}