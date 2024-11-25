using Newtonsoft.Json;

namespace Thesis_backend.Data_Structures
{
    public record OwnedCar : DbElement
    {
        /// <summary>
        /// Foreign key to Game
        /// </summary>
        public long GameId { get; set; } 
        /// <summary>
        /// Navigation property to Game
        /// </summary>
        public Game Game { get; set; }
        /// <summary>
        /// Foreign key to Shop
        /// </summary>
        public long ShopId { get; set; }  
        /// <summary>
        /// Navigation property to Shop
        /// </summary>
        public Shop Shop { get; set; }  

        [JsonIgnore]
        public override object Serialize => new
        {
            ID,
            GameId,
            ShopId
        };
    }
}