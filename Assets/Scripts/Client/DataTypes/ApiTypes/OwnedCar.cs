using Newtonsoft.Json;

namespace Thesis_backend.Data_Structures
{
    public record OwnedCar : DbElement
    {
        public long GameId { get; set; } // Foreign key to Game
        public Game Game { get; set; } // Navigation property to Game

        public long ShopId { get; set; } // Foreign key to Shop
        public Shop Shop { get; set; } // Navigation property to Shop

        [JsonIgnore]
        public override object Serialize => new
        {
            ID,
            GameId,
            ShopId
        };
    }
}