using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Amazon.Lambda.Core;
using System.Net;
using System.Collections;
using System.Net.Http;
using System.Security.Authentication;
using MySqlX.XDevAPI;
using JsonDiffer;
using Newtonsoft.Json.Linq;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.SQS.Model;
using Amazon.SQS;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace OMS_ORDER_ID_TRAFFIC_LAMBDA {
    public class Function {
        public void FunctionHandler(INPUT input, ILambdaContext context) {
            MySql_class msql1 = new MySql_class();

            string AccessToken = Auth();
            string order_id = Parser(input.HTML);

            List<Item> All_items = new List<Item>();

            string link = "url to API" + order_id + "&status=resources_waiting";
            var client = new RestClient(link);
            var request = new RestRequest();

            request.AddHeader("Authorization", "Bearer " + AccessToken);
            var response = client.Get(request);

            string JSON_LEADS = WebUtility.HtmlDecode(response.Content);
            TrafficObj tobj = JsonConvert.DeserializeObject<TrafficObj>(JSON_LEADS);

            All_items.AddRange(tobj.items);

            if (All_items.Count > 0) 
            {
                Item requst = All_items[0];

                string sql_p = "SELECT PORTAL_ORDER_ID,INPUT_JSON FROM OMS_ORDERS_INPUT WHERE OMS_PORTALS_ID=2 AND DELETED=0 AND PORTAL_ORDER_ID='" + requst.id + "';";
                Dictionary<string, string> order = msql1.QueryDict(sql_p);

                string clientData = GetClient(AccessToken, requst.cargo_owning_company_id);  //receive clients for applications

                Traffic_Client tc1 = JsonConvert.DeserializeObject<Traffic_Client>(clientData);

                if (order.ContainsKey(requst.id)) 
                {
                    SQS_TrafficSend sas_new = new SQS_TrafficSend();
                    sas_new.request = requst;
                    sas_new.client = tc1;

                    //Initial track of the request object
                    string str = JsonConvert.SerializeObject(sas_new);
                    string checkstr = str.Replace("\\\\", " ").Replace("\\t", " ").Replace("\\r", " ").Replace("\\n", " ");
                    string dbstr = GetJsonFromS3("2", requst.id);
                    //If there are JSON requests from the database
                    if (dbstr != "") 
                    {
                        JToken diff = JsonDifferentiator.Differentiate(JToken.Parse(checkstr), JToken.Parse(dbstr));
                        //If something has changed in the application
                        if (diff != null) 
                        {
                            str = str.Replace("\\\\", " ").Replace("'", "\\'").Replace("\\\"", "\\\\\"").Replace("\\t", " ").Replace("\\r", " ").Replace("\\n", " ");
                            //Marking the old entry for deletion
                            msql1.Query("UPDATE `OMS_ORDERS_INPUT` SET DELETED=1 WHERE PORTAL_ORDER_ID='" + requst.id + "' AND DELETED=0;");

                            SQS_TrafficSend sas1 = new SQS_TrafficSend();
                            sas1.request = requst;
                            sas1.client = tc1;

                            string LINK_S3 = SaveJsonS3("2", requst.id, JsonConvert.SerializeObject(sas1));
                            //Create a new entry
                            string sql1 = "INSERT INTO `OMS_ORDERS_INPUT`(`OMS_PORTALS_ID`,`INPUT_JSON_S3_URL`,PORTAL_ORDER_ID)VALUES(2,'" + LINK_S3 + "','" + requst.id + "');SELECT last_insert_id() AS LID";
                            string LAST_ID = ((Hashtable)msql1.Query(sql1)[0])["LID"].ToString();

                            sas1.ID = LAST_ID;

                            string MessageId = SendMessageSQS(JsonConvert.SerializeObject(sas1), sas1.request.id);

                            msql1.QueryNoResult("UPDATE OMS_ORDERS_INPUT SET SQS_ID='" + MessageId + "' WHERE ID=" + LAST_ID);
                        }
                    }
                } 
                else 
                {
                    //Here is a new application to the database
                    string str = JsonConvert.SerializeObject(requst).Replace("\\\\", " ").Replace("'", "\\'").Replace("\\\"", "\\\\\"").Replace("\\t", " ").Replace("\\r", " ").Replace("\\n", " ");

                    //We delete old entries for this application, because from the API applications may come again
                    msql1.Query("UPDATE `OMS_ORDERS_INPUT` SET DELETED=1 WHERE PORTAL_ORDER_ID='" + requst.id + "' AND DELETED=0;");

                    SQS_TrafficSend sas1 = new SQS_TrafficSend();
                    sas1.request = requst;
                    sas1.client = tc1;

                    string LINK_S3 = SaveJsonS3("2", requst.id, JsonConvert.SerializeObject(sas1));

                    string sql1 = "INSERT INTO `OMS_ORDERS_INPUT`(`OMS_PORTALS_ID`,`INPUT_JSON_S3_URL`,PORTAL_ORDER_ID)VALUES(2,'" + LINK_S3 + "','" + requst.id + "');SELECT last_insert_id() AS LID";
                    string LAST_ID = ((Hashtable)msql1.Query(sql1)[0])["LID"].ToString();
                    order.Add(requst.id, "");

                    sas1.ID = LAST_ID;

                    string MessageId = SendMessageSQS(JsonConvert.SerializeObject(sas1), sas1.request.id);
                    msql1.QueryNoResult("UPDATE OMS_ORDERS_INPUT SET SQS_ID='" + MessageId + "' WHERE ID=" + LAST_ID);
                }
            }
            msql1.Dispose();
        }

        /// <summary>
        ///Parser for extracting application ID from HTML.
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public string Parser(string HTML) {
            INPUT input = new INPUT();
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(input.HTML);

            var order = htmlDoc.DocumentNode.SelectSingleNode("//p[contains(@class, 'caption')]");

            if (order != null) {
                string text = order.InnerText;

                //I pull out the application number from the line
                int first = text.IndexOf("№ ") + 2;
                int last = text.IndexOf(" от", first);

                if (first != -1 && last != -1) {
                    return text.Substring(first, last - first).Trim();
                }
            }
            return null;
        }

        /// <summary>
        ///Method for sending a message to a queue
        /// </summary>
        /// <param name="SQSName"></param>
        /// <param name="Message"></param>
        /// <returns>Номер сообщения в очереди </returns>
        public string SendMessageSQS(string Message, string MessageGroupId) {
            string SQSName = "";
            var region = Environment.GetEnvironmentVariable("AWS_REGION");
            if (region == "eu-central-1")
                SQSName = "sqs url";
            else
                SQSName = "sqs url";

            AmazonSQSClient client = new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(region));
            SendMessageRequest smr = new SendMessageRequest() {
                DelaySeconds = 0,
                MessageBody = Message,
                QueueUrl = SQSName,
                MessageGroupId = MessageGroupId
            };
            Task<SendMessageResponse> resultTask = client.SendMessageAsync(smr);
            resultTask.Wait();
            SendMessageResponse result = resultTask.Result;
            return result.MessageId;
        }

        /// <summary>
        /// Method for obtaining a client
        /// </summary>
        /// <param name = "AccessToken" ></ param >
        /// < param name="client_id"></param>
        /// <returns></returns>
        public static string GetClient(string AccessToken, string client_id) {
            string link = "url to API" + client_id;
            var client = new RestClient(link);
            var request = new RestRequest();
            request.AddHeader("Authorization", "Bearer " + AccessToken);
            var response = client.Get(request);
            return response.Content.Replace("\\\\", " ").Replace("'", "\\'").Replace("\\t", " ").Replace("\\r", " ").Replace("\\n", " ");
        }

        /// <summary>
        ///Method for getting JSON from S3
        /// </summary>
        /// <param name = "PORTAL_ID" > Unique portal number</param>
        /// <param name = "PORTAL_ORDER_ID" > Unique application number in the portal</param>
        /// <param name = "BUCKETNAME" > Bucket name</param>
        /// <returns>File contents</returns>
        public string GetJsonFromS3(string PORTAL_ID = "", string PORTAL_ORDER_ID = "", string BUCKETNAME = "omsordersinput") {
            string contents = "";
            try {
                string keyName = PORTAL_ID + "/" + PORTAL_ORDER_ID + ".json";
                AmazonS3Client s3Client = new AmazonS3Client(Amazon.RegionEndpoint.EUCentral1);
                GetObjectRequest gor1 = new GetObjectRequest();
                GetObjectRequest request = new GetObjectRequest {
                    BucketName = BUCKETNAME,
                    Key = keyName
                };
                Task<GetObjectResponse> t1 = s3Client.GetObjectAsync(request);
                t1.Wait();
                GetObjectResponse response = t1.Result;
                StreamReader reader = new StreamReader(response.ResponseStream);
                contents = reader.ReadToEnd();
            } catch (Exception e1) { }
            return contents;
        }

        /// <summary>
        /// Method for saving JSON to S3
        /// </summary>
        /// <param name="PORTAL_ID">Unique portal number</param>
        /// <param name="PORTAL_ORDER_ID">Unique application number in the portal</param>
        /// <param name="JSON">Application content</param>
        /// <param name="BUCKETNAME">Bucket name</param>
        /// <returns>File link</returns>
        public string SaveJsonS3(string PORTAL_ID = "", string PORTAL_ORDER_ID = "", string JSON = "", string BUCKETNAME = "") {
            //BUCKETNAME=
            string keyName = PORTAL_ID + "/" + PORTAL_ORDER_ID + ".json";

            try {
                TransferUtility fileTransferUtility = new
                TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.EUCentral1));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JSON));
                fileTransferUtility.Upload(ms, BUCKETNAME, keyName);

                string LINK = "https://s3.eu-central-1.amazonaws.com/" + BUCKETNAME + "/" + keyName + "";

                return LINK;
            } catch (AmazonS3Exception s3Exception) {
                Console.WriteLine("Upload s3 not completed " + s3Exception.Message + "  " + s3Exception.InnerException);
                return "";
            }
        }

        public static string Auth() {
            string URL = "url to API";

            MySql_class msql1 = new MySql_class();
            ArrayList auths = msql1.Query("SELECT access_token FROM `OMS_TRAFFIC_AUTH` WHERE expires_in > current_timestamp;");
            if (auths.Count != 0) {
                Hashtable auth = (Hashtable)auths[0];
                return auth["access_token"].ToString();
            } else {
                AuthTraffic_Input at1 = new AuthTraffic_Input();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler();
                handler.SslProtocols = SslProtocols.Tls13;

                HttpClient client = new HttpClient(handler);

                string responseSTR = "";
                // Call asynchronous network methods in a try/catch block to handle exceptions.
                try {
                    var content = new StringContent(JsonConvert.SerializeObject(at1), System.Text.Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> responseTask = client.PostAsync(URL, content);
                    responseTask.Wait();
                    var response = responseTask.Result;
                    response.EnsureSuccessStatusCode();
                    Task<string> responseBodyTask = response.Content.ReadAsStringAsync();
                    responseBodyTask.Wait();

                    string responseBody = responseBodyTask.Result;

                    responseSTR = responseBody;
                } catch (HttpRequestException e) {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }

                AuthTraffic_Output ao1 = JsonConvert.DeserializeObject<AuthTraffic_Output>(responseSTR);

                DateTime dt1 = DateTime.Now;
                DateTime expires_in = dt1.AddSeconds(ao1.expires_in);
                msql1.Query(" INSERT INTO `OMS_TRAFFIC_AUTH` (`access_token`,`token_type`,`expires_in`,`refresh_token`) VALUES ('" + ao1.access_token + "','" + ao1.token_type + "',STR_TO_DATE('" + expires_in.ToString("dd.MM.yyyy HH:mm:ss") + "','%d.%m.%Y %H:%i:%s'),'" + ao1.refresh_token + "');");
                msql1.Dispose();
                return ao1.access_token;
            }
        }
    }
}












