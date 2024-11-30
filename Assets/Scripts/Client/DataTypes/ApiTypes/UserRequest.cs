using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thesis_backend.Data_Structures
{
    /// <summary>
    /// User create request object
    /// </summary>
    public struct UserRequest
    {
        /// <summary>
        /// The username of the user
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; }
    }
}