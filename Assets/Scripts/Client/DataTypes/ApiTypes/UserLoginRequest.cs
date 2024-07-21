using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thesis_backend.Data_Structures
{
    public struct UserLoginRequest
    {
        public string UserIdentification { get; set; }
        public string Password { get; set; }
    }
}