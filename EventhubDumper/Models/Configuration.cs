using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventhubDumper.Models
{
    class Configuration
    {
        public string ConnectionString { get; set; }
        public string EventHubName { get; set; }
        public DateTime StartTimestamp { get; set; }
        public string WriteTo { get; set; }
    }
}
