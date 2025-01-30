using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_App
{
    class Notification
    {
        public string Title { get; set; }
        public string Desc { get; set; }
        public DateTime DueDate { get; set; }
        public string OutDated {  get; set; }
        public Guid UserID { get; set; }
    }
}
