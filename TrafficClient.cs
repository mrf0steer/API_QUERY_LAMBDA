using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS_ORDER_ID_TRAFFIC_LAMBDA {
    public class Opf {
        public int id { get; set; }
        public string code { get; set; }
        public string kind { get; set; }
        public string name { get; set; }
    }

    public class RealAddress {
        public string city { get; set; }
        public string room { get; set; }
        public string street { get; set; }
        public string zip_code { get; set; }
        public string house_number { get; set; }
    }

    public class LegalAddress {
        public string city { get; set; }
        public string room { get; set; }
        public string street { get; set; }
        public string zip_code { get; set; }
        public string house_number { get; set; }
    }

    public class Props {
        public string bik { get; set; }
        public string inn { get; set; }
        public string kpp { get; set; }
        public Opf opf { get; set; }
        public string ogrn { get; set; }
        public List<string> okvds { get; set; }
        public string title { get; set; }
        public int opf_id { get; set; }
        public string tax_type { get; set; }
        //public string bank_name { get; set; }
        public object promo_code { get; set; }
        public string action_type { get; set; }
        public RealAddress real_address { get; set; }
        public LegalAddress legal_address { get; set; }
        public string basis_for_sign { get; set; }
        public DateTime ogrn_created_at { get; set; }
        public string settlement_account { get; set; }
        public string correspondent_account { get; set; }
        public bool legal_address_eq_real { get; set; }
    }

    public class Country {
        public int id { get; set; }
        public string name { get; set; }
        public string alpha_2 { get; set; }
        public string code_name { get; set; }
        public int num { get; set; }
    }

    public class Traffic_Client {
        public string id { get; set; }
        public List<string> kinds { get; set; }
        public Props props { get; set; }
        public object rating { get; set; }
        public Country country { get; set; }
        public string logo_url { get; set; }
        public int country_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string human_friendly_id { get; set; }
    }
}
