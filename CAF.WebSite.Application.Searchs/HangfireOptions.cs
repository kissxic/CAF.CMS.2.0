using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Searchs
{
    public class HangfireOptions
    {
        public bool StartServer { get; set; }
        public string JobStorageType { get; set; }
        public string DatabaseConnectionStringName { get; set; }
    }
}
