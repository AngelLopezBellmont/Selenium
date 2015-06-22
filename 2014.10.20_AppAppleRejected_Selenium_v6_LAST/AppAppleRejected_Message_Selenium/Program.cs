using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

//2014.09.30 Alopez 
using OpenQA.Selenium.Support.UI;



// 2014.09.10 Alopez 
// This program takes the element which are in Table SlitteApp and as well in  SlitteAppAppleRejected. In principle all who are rejected by Apple in the website iTunes.
// We take the Apple_ID, we go to his iTunesWebsite and we copy the Apple information about why this App ist rejected.
// We copy this information in the column [SlitteAppAppleRejected.Message]
// We use Selenium for this Project.


namespace AppAppleRejected_Message_Selenium
{
    using Queries;
    using myNameSpace_csCreateaAndWrite_Log;

    class Program
    {

        private static FirefoxDriver my_FireFoxDriver;
        private static csCreateaAndWrite_Log myLog_AppAppleRejected_Message_Selenium = new csCreateaAndWrite_Log("AppAppleRejected_Message_Selenium_LOG");


        static void Main(string[] args)
        {
            myLog_AppAppleRejected_Message_Selenium.direct_Write_txt(" ---- Star Process ---");

            // 1- We run through recordset FROM SlitteApp WHERE ITC_Status1 = 'Rejected'
            recordset_SlitteApp_Rejects();
            //Console.WriteLine("hallo");

            myLog_AppAppleRejected_Message_Selenium.direct_Write_txt(" ---- Ende Process ---");
        }




        public static void recordset_SlitteApp_Rejects()
        {

            // We open the recordset SlitteApp WHERE ITC_Status1 = 'Rejected'. 

            SqlConnection my_slitte_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            my_slitte_connection.Open();

            Queries myClass_Query = new Queries();
            string my_sqlReadQuery = myClass_Query.my_sqlReadQuery_Reject;
            SqlCommand my_slitte_command_toRead = new SqlCommand(my_sqlReadQuery, my_slitte_connection);
            SqlDataReader my_slitte_Reader = my_slitte_command_toRead.ExecuteReader();

            FirefoxProfile fp = new FirefoxProfile();
            
            // We open Firefox and we logIN in the Apple Website
            my_FireFoxDriver = new FirefoxDriver(fp);
            openFireFoxLogin();


            // We read the Recordset
            while (my_slitte_Reader.Read())
            {
                //object my_SlitteApp_ITC_AppleID = my_slitte_Reader.GetSqlValue(0);
                int my_SlitteApp_ITC_AppleID = my_slitte_Reader.GetInt32(0);


                //my_SlitteApp_ITC_AppleID = 884131324;
                //System.Console.WriteLine(my_slitte_Reader.GetInt32(0));


                // We obtain here the text from the Apple website for each item in the recordset.
                String my_resultBody = surf_FireFoxGetText(my_SlitteApp_ITC_AppleID);

                


                //Now we only need to insert or update in the table [SlitteAppAppleRejected].
                // Here I check if this element is in SlitteAppAppleRejected with function is_Elemt_InRejectedTabelle
                // If my_SlitteApp_ITC_AppleID is in SlitteAppAppleRejected function is_Elemt_InRejectedTabelle = TRUE, ifnot false
                // if yes -> UPDATE;  if not -> INSERT;

                if (is_Elemt_InRejectedTabelle(my_SlitteApp_ITC_AppleID) == true)
                {
                    // Update in BBDD 
                    // 2014.10.02 Alopez when the text is "NO UPDATE" we keep the old message - > we make no update in BBDD
                    //if (my_resultBody =! "NO UPDATE") { UpdateBBDD(my_resultBody, my_SlitteApp_ITC_AppleID); }
                      if (my_resultBody != "NO UPDATE") { UpdateBBDD(my_resultBody, my_SlitteApp_ITC_AppleID); }
                       
                }

                else
                {
                    // Insert in BBDD 
                    InsertBBDD(my_resultBody, my_SlitteApp_ITC_AppleID);
                }

            }

            my_slitte_Reader.Close();
            my_slitte_connection.Close();
            my_FireFoxDriver.Close();

        }


        public static  string surf_FireFoxGetText(int appleID)
        {
            //bool has_rejectionReasonList;
            //bool is_WaitingForReview;
            //Queries myClass_Query = new Queries();
            //System.Console.WriteLine(appleID);
            string myText_toReturn = "NO UPDATE";

            string myURL = Queries.getUrlFirefox(appleID);
            Console.WriteLine(myURL);

            //status_website[0] =  1 -> The website opens;   status_website[0] =  0 -> The website NOT opens; 
            //status_website[1] =  1 -> has head;   status_website[1] =  0 -> has NO head;
            //status_website[2] =  1 -> has body;   status_website[2] =  0 -> has NO body;
            //status_website[3] =  1 -> is waiting for review;   status_website[3] =  0 ->is NOT waiting for review;
            //status_website[4] =  1 -> is approved;   status_website[4] =  0 ->is NOT is approved

            int[] status_website = new int[5];

            WebDriverWait my_Wait = new WebDriverWait(my_FireFoxDriver, TimeSpan.FromSeconds(5));

            //my_FireFoxDriver.Navigate().Refresh();
            my_FireFoxDriver.Navigate().GoToUrl(myURL);

            ////2014.09.31 Alopez if is the presentattion website after login we go direct to the first App if
            //if (my_FireFoxDriver.Url == "https://itunesconnect.apple.com/WebObjects/iTunesConnect.woa/ra/ng/") { my_FireFoxDriver.Navigate().GoToUrl(myURL); }


            //2014.09.31 We check if the WEBSITE is: OPEN, HEAD, BODY, WITING FOR REVIEW. Using a WebDriverWait (which is included in -> using OpenQA.Selenium.Support.UI;)

            try //Open the website;   status_website[0]
            {
                //my_Wait.Until((d) => { try { return d.CurrentWindowHandle; } catch { return null; } });
                status_website[0] = 1;
            }
            catch { status_website[0] = 0;}

            try //Hat head;  status_website[1]
            {
                my_Wait.Until((d) => { try { return d.FindElement(By.ClassName("rejectionReasonList")); } catch { return null; } });
                status_website[1] = 1;
            }
            catch { status_website[1] = 0; }

            try //Hat body; status_website[2]
            {
                my_Wait.Until((d) => { try { return d.FindElement(By.ClassName("body")); } catch { return null; } });
                status_website[2] = 1;
            }
            catch { status_website[2] = 0; }

            try //Waiting for Review; status_website[3]
            {
                // Makes to slowly the check because wits alway 5 seconds
                //my_Wait.Until((d) => { try { return d.FindElement(By.ClassName("outsetbox app-res-info")); } catch { return null; } });
                string my_result_WaitingForReview = my_FireFoxDriver.FindElementByClassName("outsetbox app-res-info").Text;
                status_website[3] = 1;
            }
            catch { status_website[3] = 0; }


            try //Approved Apps; status_website[4]
            {
                // Makes to slowly the check because wits alway 5 seconds
                //my_Wait.Until((d) => { try { return d.FindElement(By.ClassName("outsetbox app-res-info")); } catch { return null; } });
                string my_result_WaitingForReview = my_FireFoxDriver.FindElementByClassName("instructional").Text;
                status_website[4] = 1;
            }
            catch { status_website[4] = 0; }


            // If Website OPEN & HEAD & BODY -> We return text from Apple.
            // If Website OPEN & WAITING FOR REVIEW -> We return text = "WAITING FOR REVIEW".
            // Any other possibility -> We return "NO UPDATE".
            try
            {

                if (status_website[0] == 1 & status_website[1] == 1 & status_website[2] == 1 & status_website[3] == 0)
                {

                    string my_result_Head = my_FireFoxDriver.FindElementByClassName("rejectionReasonList").Text;
                    string my_resultBody = my_FireFoxDriver.FindElementByClassName("body").Text;

                    myText_toReturn = my_result_Head + my_resultBody;
                    // Screenshot and save it into screen.png;  my_FireFoxDriver.GetScreenshot().SaveAsFile(@"screen_"+appleID +".png", ImageFormat.Png);
                    my_FireFoxDriver.GetScreenshot().SaveAsFile(@"" + appleID + ".png", ImageFormat.Png);
                }


                if (status_website[0] == 1 & status_website[1] == 0 & status_website[2] == 0 & status_website[3] == 1)
                {

                    my_FireFoxDriver.GetScreenshot().SaveAsFile(@"" + appleID + ".png", ImageFormat.Png);

                    myText_toReturn = "WAITING FOR REVIEW";
                }
            }

            catch { myText_toReturn = "NO UPDATE"; }


            //2014.10.02 Alopez approved
            if (status_website[0] == 1 & status_website[4] == 1)
            {
                string my_result_WaitingForReview = my_FireFoxDriver.FindElementByClassName("instructional").Text;
                myText_toReturn = "Approved";
            }
          


            return myText_toReturn;
        }



        public static void InsertBBDD(string myText, object myID)
        {

            //myID = 824217811;
            //// To make demos and clean the field.
            //myText = "";

            myText = myText.Replace("'", ""); // we take out all this ' symbols from the text. Because they give us errors when we pass the information accross the program.

            SqlConnection my_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            //SqlConnection my_connection = new SqlConnection("server= XD-AT-W8; database=BarForce; Trusted_Connection=yes; connection timeout=120");

           
            my_connection.Open();


            //Queries myClass_Query = new Queries();
            //string my_sqlquery_Insert = myClass_Query.QueryUpdate(myText, myID); 

            //string my_sqlquery_Insert = " USE BarForce Insert  into dbo.[SlitteAppAppleRejected] ([AppleID], [Message], [DateTime], [CreationTime]) " +
            //                            " VALUES ( @AppleID, @Message, @DateTime, @CreationTime) ";

            string my_sqlquery_Insert = " USE BarForce Insert  into dbo.[SlitteAppAppleRejected] ([AppleID], [Message], [DateTime], [CreationTime]) " +
                            " VALUES ( " + myID + " , '" + myText + "', '" + DBNull.Value + "', '" + DateTime.Now + "') ";

            SqlCommand my_command_Insert = new SqlCommand(my_sqlquery_Insert, my_connection);


            //my_command_Insert.Parameters.Add("@AppleID", myID);
            //my_command_Insert.Parameters.Add("@Message", myText);
            //// my_command_Insert.Parameters.Add("@DateTime", DateTime.MinValue);
            //my_command_Insert.Parameters.Add("@DateTime", DBNull.Value);
            //my_command_Insert.Parameters.Add("@CreationTime", DateTime.Now);

            my_command_Insert.ExecuteNonQuery();
            my_connection.Close();

            string myString_forLog = "AppleID: " + myID + "  Insert in the DDBB" ;

            //csCreateaAndWrite_Log myLog_AppAppleRejected_Message_Selenium = new csCreateaAndWrite_Log("AppAppleRejected_Message_Selenium_LOG");
            myLog_AppAppleRejected_Message_Selenium.direct_Write_txt(myString_forLog);
        }




        public static void UpdateBBDD(string myText, object myID)
        {


            // To make demos and clean the field.
            //myText = "";


            SqlConnection my_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            SqlCommand my_slitte_command_Update;

            myText = myText.Replace("'", "");

            //string my_sqlquery_update = " USE BarForce update  dbo.[SlitteAppAppleRejected] SET Message = '" + myText + "' WHERE AppleID = " + myID + "";
            Queries myClass_Query = new Queries();
            string my_sqlquery_update = myClass_Query.QueryUpdate(myText, myID);


            my_connection.Open();
            my_slitte_command_Update = new SqlCommand(my_sqlquery_update, my_connection);
            my_slitte_command_Update.ExecuteNonQuery();
            my_connection.Close();

            string myString_forLog = "AppleID: " + myID + "  Update in the DDBB";
            myLog_AppAppleRejected_Message_Selenium.direct_Write_txt(myString_forLog);
        }



        public static void openFireFoxLogin()
        {


            int my_adamId = 860011466; // deosn´t matter which ID is here it´s only to open and login in the Apple website

            //Queries myClass_Query = new Queries();
            string myURL = Queries.getUrlFirefox(my_adamId);

            //System.Console.WriteLine(myURL);
            my_FireFoxDriver.Navigate().GoToUrl(myURL);

            // Get User Name field, Password field and Login Button
            var userNameField = my_FireFoxDriver.FindElementById("accountname");
            var userPasswordField = my_FireFoxDriver.FindElementById("accountpassword");

            //var loginButton = driver.FindElementByXPath("//input[@value='Login']");
            var loginButton = my_FireFoxDriver.FindElementByName("1.Continue");

            // Type user name and password
            userNameField.SendKeys("slitte.tobit@tobit.com");
            userPasswordField.SendKeys("Tobit913");

            // and click the login button loginButton.Click();
            // 2014.09.08 Alopez, I change to Submit. The event click() doesn´t work 
            loginButton.Submit();
        }





        public static Boolean AllRejectItemsAreInRejectTable()
        {
            Boolean yesAllAreIn = false;

            Queries myClass_Query = new Queries();
            string my_sqlReadQuery = myClass_Query.my_sqlCheckRejects;

            SqlConnection my_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            my_connection.Open();


            SqlCommand my_slitte_command_toRead = new SqlCommand(my_sqlReadQuery, my_connection);
            SqlDataReader my_slitte_Reader = my_slitte_command_toRead.ExecuteReader();

            if (my_slitte_Reader.HasRows)
            {
                yesAllAreIn = false;
            }
            else
            {
                yesAllAreIn = true;
            }

            my_connection.Close();
            return yesAllAreIn;

        }





        public static Boolean is_Elemt_InRejectedTabelle(object myID)
        {
            bool myFlag;
            Queries myClass_Query = new Queries();

            string my_sqlReadQuery = myClass_Query.Query_is_Elemt_InRejectedTabelle(myID);

            SqlConnection my_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            my_connection.Open();

            SqlCommand my_slitte_command_toRead = new SqlCommand(my_sqlReadQuery, my_connection);
            SqlDataReader my_slitte_Reader = my_slitte_command_toRead.ExecuteReader();

            if (my_slitte_Reader.HasRows)
            {
                myFlag = true;
            }
            else
            {
                myFlag = false;
            }

            my_connection.Close();

            return myFlag;
        }


    }
}
