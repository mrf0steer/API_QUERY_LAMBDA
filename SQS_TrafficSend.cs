using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS_ORDER_ID_TRAFFIC_LAMBDA {
    public class SQS_TrafficSend {

        public Traffic_Client client { get; set; }
        public string ID = "";
        public Item request { get; set; }
    }
}
