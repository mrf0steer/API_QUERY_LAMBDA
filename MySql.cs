using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace OMS_ORDER_ID_TRAFFIC_LAMBDA {
    public class MySql_class {
        MySqlConnection mycon;
        /// <summary>
        /// connect
        /// </summary>
        /// <param name="dbname">name db</param>
        /// <param name="dbuser">user</param>
        /// <param name="dbpass">pass</param>
        /// <param name="dSource">adress</param>
        public MySql_class(string dbname ="", string dbuser = "", string dbpass = "", string dSource = "", string dPort = "") {
            
            string connStr = "";
            //var region = Environment.GetEnvironmentVariable("AWS_REGION");
            //if (region == "eu-west-3") //DEV 
            //    connStr = Environment.GetEnvironmentVariable("MYSQLDBDEV");       //MYSQLDBDEV
            //else
            //    connStr = Environment.GetEnvironmentVariable("MYSQLDBPROD");  //MYSQLDBPROD

            connStr = "db volume for coonnect";


            mycon = new MySqlConnection(connStr);

            if (mycon.State != ConnectionState.Open)
                try {
                    mycon.Open();
                } catch (MySqlException ex) {
                    throw (ex);
                }
        }


        public ArrayList Query(string SQLQuery) {
            ArrayList records = new ArrayList(); // create an array of lists
            MySqlCommand myCommand = new MySqlCommand(SQLQuery, mycon);
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();

            while (MyDataReader.Read()) {
                //string result = MyDataReader.GetString(0); //Get the string
                // int id = MyDataReader.GetInt32(1); //Get an integer
                Hashtable row = new Hashtable(); //create an associative array of strings
                //------------------------------------------------------------------------------------------------------------------------------------------
                for (int i = 0; i < MyDataReader.FieldCount; i++)  // цикл по стобцам, счетчик - i 
                    row.Add(MyDataReader.GetName(i), MyDataReader[i]); // an entry with the column name and content is added to the associative array
                //------------------------------------------------------------------------------------------------------------------------------------------
                records.Add(row); //add the list to the array of lists 


            }
            MyDataReader.Close();
            return records;
        }


        /// <summary>
        /// Method of reading into the Dictionary for one request
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public Dictionary<string, string> QueryDict(string sqlQuery) {
            var dictionary = new Dictionary<string, string>();

            using (var mySqlCommand = new MySqlCommand(sqlQuery, mycon))
            using (var mySqlDataReader = mySqlCommand.ExecuteReader()) {
                if (mySqlDataReader.Read()) 
                {
                    for (int i = 0; i < mySqlDataReader.FieldCount; i++) {
                        dictionary.Add(mySqlDataReader.GetName(i), mySqlDataReader[i].ToString());
                    }
                }
            }
            return dictionary;
        }


        public void QueryNoResult(string SQLQuery) {
            MySqlCommand myCommand = new MySqlCommand(SQLQuery, mycon);
            myCommand.ExecuteNonQuery();
            myCommand.Dispose();
        }

        public void Dispose() {
            mycon.Close();
            mycon.Dispose();
        }
    }
}