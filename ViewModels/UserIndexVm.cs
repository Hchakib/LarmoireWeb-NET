using System.Collections.Generic;
using LarmoireWeb.Models;

namespace LarmoireWeb.ViewModels
{
    public class UserIndexVm
    {
        public string SearchTerm { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}
