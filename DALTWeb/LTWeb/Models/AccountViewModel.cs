using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTWeb.Models
{
    public class AccountViewModel
    {
        public LoginViewModel Login { get; set; } = new LoginViewModel();
        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
    }
}