using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_App
{
    public class Client
    {
        public Guid ClientID { get; set; }
        public string ClientName { get; set; }
        public string ReqStatus { get; set; }
        public string ClientAddress { get; set; }
        public string PhoneNumber { get; set; }
        public int NrClient { get; set; }
        public string OLClient { get; set; }
    }
}
