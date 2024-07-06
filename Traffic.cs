using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS_ORDER_ID_TRAFFIC_LAMBDA {
    public class TrafficObj {
        public PageInfo page_info { get; set; }
        public List<Item> items { get; set; }
    }

    public class PageInfo {
        public int total_results { get; set; }
        public int results_per_page { get; set; }
        public int current_page { get; set; }
    }

    public class Item {
        public string kind { get; set; }
        public string id { get; set; }
        public string human_friendly_id { get; set; }
        public DateTime created_at { get; set; }
        public Status status { get; set; }
        public DateTime status_updated_at { get; set; }
        public List<StatusHistory> status_history { get; set; }
        public string type { get; set; }
        public string cargo_owning_company_id { get; set; }
        public string author_id { get; set; }
        public string client_category { get; set; }
        public string comment { get; set; }
        public Contact contact { get; set; }
        public ResourceRequirements resource_requirements { get; set; }
        public string cargo_description { get; set; }
        public double cargo_tonnage { get; set; }
        public double cargo_volume { get; set; }
        public double? cargo_cost { get; set; }
        public List<RouteSegment> route_segments { get; set; }
        public List<RoutePoint> route_points { get; set; }
        public string route_direction_id { get; set; }
        public RouteDirection route_direction { get; set; }
        public bool circular_route { get; set; }
        public bool is_interval { get; set; }
        public DateTime offer_deadline { get; set; }
        public string auction_type { get; set; }
        public bool many_executors_allowed { get; set; }
        public double? bid_step { get; set; }
        public List<object> bids { get; set; }
        public bool expedited_payment_available { get; set; }
        public double? price { get; set; }
        public int? expedited_payment_fee { get; set; }
        public double? instant_reservation_price { get; set; }
        public double? initial_bid { get; set; }
        public double? expedited_payment_initial_bid { get; set; }
        public double? min_bid { get; set; }
        public double? expedited_payment_min_bid { get; set; }
        public double? max_bid { get; set; }
        public double? expedited_payment_max_bid { get; set; }
        public bool is_bid_winning { get; set; }
        public bool dynamic_pricing { get; set; }
        public bool rebidding { get; set; }
        public bool change_requests_allowed { get; set; }
        public bool terms_by_partnership_contract { get; set; }
        public List<object> tc_requirements_changes { get; set; }
    }

    public class Status {
        public string code_name { get; set; }
        public string comment { get; set; }
    }

    public class StatusHistory {
        public Status status { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Contact {
        public string full_name { get; set; }
        public string phone { get; set; }
        public string extension_number { get; set; }
        public string email { get; set; }
    }

    public class ResourceRequirements {
        public List<string> transport_body_type { get; set; }
        public object transport_type { get; set; }
        public List<string> lifting_capacity { get; set; }
        public List<LiftingCapacity> lifting_capacities { get; set; }
        public bool has_russian_citizen { get; set; }
        public bool has_driver_coveralls { get; set; }
        public bool has_no_criminal_records { get; set; }
        public bool has_rigid_board { get; set; }
        public bool has_medical_book { get; set; }
        public bool has_removable_upper_beam { get; set; }
        public bool has_removable_side_racks { get; set; }
        public bool two_sided { get; set; }
        public bool is_express { get; set; }
        public int? number_of_belts { get; set; }
        public bool has_shipping_power_of_attorney_original { get; set; }
        public bool is_disinfected { get; set; }
        public bool has_temperature_check { get; set; }
        public bool own_transport_only { get; set; }
        public bool timber_bunks { get; set; }
        public bool tir_document { get; set; }
        public bool cmr_document { get; set; }
        public object temperature_condition { get; set; }
    }

    public class LiftingCapacity {
        public string id { get; set; }
        public double tonnage { get; set; }
        public double volume { get; set; }
        public string transport_type { get; set; }
        public string transport_body_type { get; set; }
    }

    public class RouteSegment {
        public int id { get; set; }
        public int from_route_point_id { get; set; }
        public int to_route_point_id { get; set; }
        public double distance { get; set; }
        public Loading loading { get; set; }
        public Unloading unloading { get; set; }
    }

    public class Loading {
        public List<string> type { get; set; }
        public int duration { get; set; }
    }

    public class Unloading {
        public List<string> type { get; set; }
        public int duration { get; set; }
    }

    public class RoutePoint {
        public string kind { get; set; }
        public int id { get; set; }
        public List<CarSupplyRange> car_supply_range { get; set; }
        public List<CarSupplyGroupedRange> car_supply_grouped_range { get; set; }
        public DateTime? car_supply_at { get; set; }
        public bool need_document_work { get; set; }
        public List<object> additional_services { get; set; }
        public string cargo_receiver_sender_address { get; set; }
        public CounterAgent counter_agent { get; set; }
        public List<Contact> contacts { get; set; }
        public CargoReceiverSenderInfo cargo_receiver_sender_info { get; set; }
        public Location location { get; set; }
        public object radius { get; set; }
    }

    public class CarSupplyRange {
        public DateTime from { get; set; }
        public DateTime until { get; set; }
    }

    public class CarSupplyGroupedRange {
        public List<Time> times { get; set; }
        public DateTime date { get; set; }
    }

    public class Time {
        public DateTime from { get; set; }
        public DateTime until { get; set; }
    }

    public class CounterAgent {
        public bool? private_person { get; set; }
        public string legal_entity_title { get; set; }
        public string legal_entity_inn { get; set; }
        public string legal_entity_kpp { get; set; }
        public string legal_entity_opf_name { get; set; }
        public string private_person_full_name { get; set; }
        public string private_person_passport_type { get; set; }
        public string private_person_passport_number { get; set; }
        public string private_person_passport_issuer { get; set; }
        public DateTime? private_person_passport_issued_at { get; set; }
    }

    public class CargoReceiverSenderInfo {
        public string phone { get; set; }
        public string extension_number { get; set; }
        public string company_name { get; set; }
        public string contact_name { get; set; }
        public string contact_inn { get; set; }
        public string contact_kpp { get; set; }
        public string contact_opf_name { get; set; }
        public string passport_number { get; set; }
        public DateTime? passport_issued_at { get; set; }
        public string passport_who_issued { get; set; }
        public string address { get; set; }
    }

    public class Location {
        public string kind { get; set; }
        public string id { get; set; }
        public int country_id { get; set; }
        public Country country { get; set; }
        public string country_iso_code { get; set; }
        public string federal_district { get; set; }
        public string region { get; set; }
        public string region_type { get; set; }
        public string region_with_type { get; set; }
        public string region_fias_id { get; set; }
        public string region_kladr_id { get; set; }
        public string region_iso_code { get; set; }
        public string area { get; set; }
        public string area_type { get; set; }
        public string area_with_type { get; set; }
        public string area_fias_id { get; set; }
        public string area_kladr_id { get; set; }
        public string city { get; set; }
        public string city_type { get; set; }
        public string city_with_type { get; set; }
        public string city_fias_id { get; set; }
        public string city_kladr_id { get; set; }
        public string locality { get; set; }
        public string settlement { get; set; }
        public string settlement_type { get; set; }
        public string settlement_with_type { get; set; }
        public string settlement_fias_id { get; set; }
        public string settlement_kladr_id { get; set; }
        public string street { get; set; }
        public string street_type { get; set; }
        public string street_with_type { get; set; }
        public string street_fias_id { get; set; }
        public string street_kladr_id { get; set; }
        public int? fraction { get; set; }
        public int? housing { get; set; }
        public int? structure { get; set; }
        public string literature { get; set; }
        public string house { get; set; }
        public string house_fias_id { get; set; }
        public string house_kladr_id { get; set; }
        public string apartment { get; set; }
        public string office { get; set; }
        public string code { get; set; }
        public string fias_id { get; set; }
        public string kladr_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string zip_code { get; set; }
        public int local_time_zone { get; set; }
        public string timezone_name { get; set; }
        public string level { get; set; }
        public string comment { get; set; }
        public string full_string_address { get; set; }
        public string value { get; set; }
    }

    public class RouteDirection {
        public string kind { get; set; }
        public string id { get; set; }
        public int country_id { get; set; }
        public Country country { get; set; }
        public object country_iso_code { get; set; }
        public object federal_district { get; set; }
        public string region { get; set; }
        public object region_type { get; set; }
        public object region_with_type { get; set; }
        public object region_fias_id { get; set; }
        public object region_kladr_id { get; set; }
        public object region_iso_code { get; set; }
        public string area { get; set; }
        public object area_type { get; set; }
        public object area_with_type { get; set; }
        public object area_fias_id { get; set; }
        public object area_kladr_id { get; set; }
        public string city { get; set; }
        public object city_type { get; set; }
        public object city_with_type { get; set; }
        public object city_fias_id { get; set; }
        public object city_kladr_id { get; set; }
        public string locality { get; set; }
        public object settlement { get; set; }
        public object settlement_type { get; set; }
        public object settlement_with_type { get; set; }
        public object settlement_fias_id { get; set; }
        public object settlement_kladr_id { get; set; }
        public object street { get; set; }
        public object street_type { get; set; }
        public object street_with_type { get; set; }
        public object street_fias_id { get; set; }
        public object street_kladr_id { get; set; }
        public object fraction { get; set; }
        public object housing { get; set; }
        public object structure { get; set; }
        public object literature { get; set; }
        public object house { get; set; }
        public object house_fias_id { get; set; }
        public object house_kladr_id { get; set; }
        public object apartment { get; set; }
        public object office { get; set; }
        public string code { get; set; }
        public object fias_id { get; set; }
        public object kladr_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public object lat { get; set; }
        public object lng { get; set; }
        public string zip_code { get; set; }
        public int local_time_zone { get; set; }
        public object timezone_name { get; set; }
        public object level { get; set; }
        public object comment { get; set; }
        public string full_string_address { get; set; }
        public string value { get; set; }
    }
}
