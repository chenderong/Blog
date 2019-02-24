using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dncblogs.Web.ConfigureServices
{
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }
}
