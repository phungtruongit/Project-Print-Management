using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.EmailHub.Models {
    public class EmailBody {
        public string BannerImageCID { get; set; }
        public string RecipientName { get; set; }
        public string MainContent { get; set; }
    }
}