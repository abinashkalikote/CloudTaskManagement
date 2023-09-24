using App.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Providers.Interface
{
    public interface IUserProvider
    {
        public string? GetUsername();
        public int? GetUserId();
        public bool IsAdmin();
    }
}
