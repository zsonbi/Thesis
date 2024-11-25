#nullable enable
using Newtonsoft.Json;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// Shop item record
    /// </summary>
    public record Shop : DbElement
    {
        /// <summary>
        /// The name of the product
        /// </summary>
        public string? ProductName { get; set; }
        /// <summary>
        /// The cost of the item
        /// </summary>
        public int Cost { get; set; }
        /// <summary>
        /// The type of the car
        /// </summary>
        public CarType CarType { get; set; }

        [JsonIgnore]
        public override object Serialize => new { ID, ProductName, Cost, CarType };
    }
}