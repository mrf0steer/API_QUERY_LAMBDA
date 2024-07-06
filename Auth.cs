using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS_ORDER_ID_TRAFFIC_LAMBDA {
    public class AuthTraffic_Input {
        public string grant_type = "";
        public string username = "";
        public string password = "";
    }

    public class AuthTraffic_Output {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
