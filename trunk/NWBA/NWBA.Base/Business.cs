using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWBA.Base
{
    public class Business
    {
        public string ConnectionString { get; set; }

        public Business()
        {
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[Consts.CONNECTION_STRING_NAME].ConnectionString;
        }
    }
}
