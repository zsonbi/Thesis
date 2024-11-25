#nullable enable

using System.Collections.Generic;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// A single game record
    /// </summary>
    public record Game : DbElement
    {
        /// <summary>
        /// The player's current level (not used yet)
        /// </summary>
        public int Lvl { get; set; }
        /// <summary>
        /// The id of the user
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// Reference to the user
        /// </summary>
        public User? User { get; set; }
        /// <summary>
        /// The current xp of the plyer
        /// </summary>
        public long CurrentXP { get; set; }
        /// <summary>
        /// How much xp until level up
        /// </summary>
        public int NextLVLXP { get; set; }
        /// <summary>
        /// How many coins does the player has currently
        /// </summary>
        public int Currency { get; set; }
        /// <summary>
        /// The cars which the player owns
        /// </summary>
        public List<OwnedCar>? OwnedCars { get; set; }

        public override object Serialize => new { ID, Lvl, CurrentXP, NextLVLXP, UserId, Currency, OwnedCars };
    }
}