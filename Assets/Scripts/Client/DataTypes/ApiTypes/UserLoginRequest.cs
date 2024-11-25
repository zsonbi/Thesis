
namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// User login request
    /// </summary>
    public struct UserLoginRequest
    {
        /// <summary>
        /// Some identification of the user (username or email)
        /// </summary>
        public string UserIdentification { get; set; }
        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; }
    }
}