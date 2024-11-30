
namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// Settings record of the user
    /// </summary>
    public record UserSettings : DbElement
    {
        /// <summary>
        /// Reference to the user
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// The id of the user
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// The profile picture path of the user
        /// </summary>
        public string ProfilePic { get; set; }
        /// <summary>
        /// The privacy values
        /// </summary>
        public long privacy { get; set; }
    }
}