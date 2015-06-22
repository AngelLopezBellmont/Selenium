using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queries
{
    class Queries
    {
        //public string myText;
        //public object myID;


        public string my_sqlReadQuery_Reject = " USE BarForce " +
                                               " SELECT ITC_AppleID, ProcessID, SiteID, ITC_Status1 , LocationID " +
                                               " FROM [SlitteApp] (NOLOCK) WHERE (ITC_Status1 = 'Rejected' OR ITC_Status1 = 'Metadata Rejected') ORDER BY ITC_AppleID";

        public string my_sqlCheckRejects = " USE BarForce " +
                                           " SELECT a.LocationID, a.SiteID,  a.ITC_AppleID, a.ITC_Status1, a.InternalComment, b.AppleID " +
                                           " FROM [SlitteApp]  a (NOLOCK) " +
                                           " INNER JOIN [SlitteAppAppleRejected] b (NOLOCK)" +
                                           " ON a.ITC_AppleID = b.AppleID " +
                                           " WHERE (a.ITC_Status1 = 'Rejected' OR a.ITC_Status1 = 'Metadata Rejected')";


        //public string my_sqlReadQuery_demoTesting = " USE BarForce " +
        //                                    " SELECT LocationID, SiteID, ITC_AppleID, ITC_Status1 " +
        //                                    " FROM [SlitteApp] WHERE ITC_Status1 = 'Rejected' " +
        //                                    " AND ITC_AppleID = 623232968";

        public Queries()
        {
        }

        //public string QueryInsert(string newText, object newID)
        //{


        //    string my_sqlquery_Insert = " USE BarForce Insert  into dbo.[SlitteAppAppleRejected] ([ID], [AppleID], [Message], [DateTime], [CreationTime]) " +
        //                                   "VALUES (@ID, @AppleID, @Message, @DateTime, @CreationTime) ";
        //    return my_sqlquery_Insert;
        //}


        public string QueryUpdate(string newText, object newID)
        {


            DateTime myNow = DateTime.Now;
            //2014.09.23 Alopez DateTime.Now  comes in format [dd-mm-yyyy hh:mm:ss mmm] and we want [yyyy-mm-dd.....]
            string st_myNow = myNow.ToString("yyyy-MM-dd HH:mm:ss.fff");





            //string my_sqlquery_update = "USE BarForce update  dbo.[SlitteAppAppleRejected] SET " + 
            //                            "Message = '" + myText + "' " +
            //                            "WHERE AppleID = " + myID + "";


            // 2014.09.23 Alopez  we add DateTime.Now  in the columm Date Time

            //myText = "demo hallo";
            //string my_sqlquery_update = "USE BarForce update  dbo.[SlitteAppAppleRejected] SET " +
            //                "Message = '" + myText + "' , " +
            //                "DateTime = '" + string_myDayHour + "' " +
            //                "WHERE AppleID = " + myID + "";

            //string my_sqlquery_update = "USE BarForce update  dbo.[SlitteAppAppleRejected] SET " +
            //    "Message = '" + myText + "' , " +
            //    "DateTime = '2014-09-27 18:01:58' " +
            //    "WHERE AppleID = " + myID + "";


            //last one good in mein computer
            //string my_sqlquery_update = "USE BarForce update  dbo.[SlitteAppAppleRejected] SET " +
            //    "Message = '" + newText + "' , " +
            //    "DateTime = '" + st_myNow + "' " +
            //    "WHERE AppleID = " + newID + "";


            string my_sqlquery_update = "USE BarForce update  dbo.[SlitteAppAppleRejected] SET " +
            "Message = '" + newText + "' , " +
            "DateTime = convert(DateTime, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',120) " +
            "WHERE AppleID = " + newID + "";


            //Console.WriteLine(my_sqlquery_update);
            return my_sqlquery_update;

        }





        public static string getUrlFirefox(int appleID)
        {

            string myURL = "https://itunesconnect.apple.com/WebObjects/iTunesConnect.woa/da/rejectionReasons?adamId=" + appleID + "";
            return myURL;
        }





        public string Query_is_Elemt_InRejectedTabelle(object newID)
        {
            //string mySQL_is_Elemt_InRejectedTabelle = "USE BarForce " +
            //             "SELECT a.LocationID, a.SiteID,  a.ITC_AppleID, a.ITC_Status1, a.InternalComment, b.AppleID " +
            //              "FROM [SlitteApp]  a (NOLOCK) " +
            //              "INNER JOIN [SlitteAppAppleRejected] b (NOLOCK)" +
            //              "ON a.ITC_AppleID = b.AppleID " +
            //              "WHERE a.ITC_Status1 = 'Rejected' OR a.ITC_Status1 = 'Metadata Rejected'" +
            //              "AND a.ITC_AppleID = " + newID + " ";

            string mySQL_is_Elemt_InRejectedTabelle = "USE BarForce " +
             "SELECT * FROM [SlitteAppAppleRejected]  a (NOLOCK) " +
             "WHERE a.AppleID =" + newID + " ";


            return mySQL_is_Elemt_InRejectedTabelle;
        }


    }
}
