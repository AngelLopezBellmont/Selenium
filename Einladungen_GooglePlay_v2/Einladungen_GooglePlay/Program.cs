using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;

//2014.10.13 Alopez; We include this references:2014.11.05_David_Selenium_v7
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Data.SqlClient;
using DvApi32;
//2014.10.14 Alopez we include zhis to use Regex
//using System.Text.RegularExpressions; 
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;


namespace Einladungen_GooglePlay
{
    using myNameSpace_csCreateaAndWrite_Log;

    class Program
    {
        public static DvApi32.DavidAPI myApi_David = new DvApi32.DavidAPI();
        public static DvApi32.Account my_oAccount = myApi_David.GetAccount();

        private static FirefoxDriver my_FireFoxDriver;
        //public static string typeOfEmail;  // We don't need anymore, we use other way to filter each Email

        public static csCreateaAndWrite_Log myLog_David_Selenium = new csCreateaAndWrite_Log("Einladungen_GooglePlay");

        public static bool mark_Emails_As_Read;

        public static string emailAddress;
        public static int ID_Email;
        public static string str_Subject;
        public static DateTime dt_sendtime;
        public static string str_From;



        static void Main(string[] args)
        {

            myLog_David_Selenium.direct_Write_txt(" ---- Start Process David_Selenium");

            lookTheEmails();

            myLog_David_Selenium.direct_Write_txt(" ---- End Process ----- ");
        }



        public static void lookTheEmails()
        {
            //string str_Message;

            //DavidAPI oApi = new DavidAPI();
            //DvApi32.DavidAPI myApi_David = new DvApi32.DavidAPI();
            //DvApi32.Account my_oAccount = myApi_David.GetAccount();

            FirefoxProfile fp = new FirefoxProfile();
            my_FireFoxDriver = new FirefoxDriver(fp);

            //my Personal Mails. It works
            //DvApi32.Archive my_oArchive = my_oAccount.GetSpecialArchive(DvApi32.DvArchiveTypes.DvArchivePersonalIn);

            //Folder [(@"\\david\david\archive\18\4\2\b\0"]  === [David://DAVID/Group/Development/Core Development/Google Externe Accounts/Einladungen]
            string url_Path = @"\\david\david\archive\18\4\2\b\0";
            //url_Path = changeStringToDoubleSlash(url_Path);  //I dont need this function. When we use @"\\bla\bla\"  the string hat already double \. 

            DvApi32.Archive my_oArchive = my_oAccount.ArchiveFromID(url_Path);
            int myCount_Mails = my_oArchive.AllItems.Count;


            //DvApi32.MessageItem2 my_Item2;
            DvApi32.MessageItems my_All_Items;
            DvApi32.MailItem my_Single_Item;

            my_All_Items = my_oArchive.AllItems;


            //int oID = 8;
            //my_Item2 = my_oArchive.GetArchiveEntryByID(oID);

            //string mySubject_Email = my_Item.Subject;
            //Console.WriteLine(mySubject_Email);



            // 2014.10.14 Alope All the subjects
            //Reading all the Emails in Folder [(@"\\david\david\archive\18\4\2\b\0"]  === [David://DAVID/Group/Development/Core Development/Google Externe Accounts/Einladungen]
            for (int j = 0; j < myCount_Mails; j++)
            {
                //my_Item2 = my_oArchive.GetArchiveEntryByID(j);
                //Console.WriteLine(j + " -" + my_Item2.Subject);

                // We initialize teh variables
                emailAddress = "";
                ID_Email = 0;
                str_Subject = "";
                str_From = "";


                my_Single_Item = (DvApi32.MailItem)my_All_Items.Item(j);

                //2014.10.15 Alopez we read only Emails Viewed = false
                if (my_Single_Item.Viewed == false)
                {

                    mark_Emails_As_Read = false;

                    ID_Email = my_Single_Item._ID;
                    //Console.WriteLine(j + " - ID: " + int_ID);


                    str_From = my_Single_Item.From.EMail;
                    //Console.WriteLine(j + " - " + str_From);


                    //"014.10.15 Alopez We check from which country comes the Email
                    str_Subject = my_Single_Item.Subject;
                    //Console.WriteLine(j + " - " + str_Subject);
                    //typeOfEmail = check_WhatKindOfEmail(str_Subject);                                                     // 2014.10.16 Alopez old function


                    String str_Destination = my_Single_Item.Destination;
                    //Console.WriteLine(j + " - " + str_Destination);

                    dt_sendtime = my_Single_Item.SendTime;
                    //Console.WriteLine(j + " - dt_sendtime: " + dt_sendtime);

                    DateTime dt_StatusTime = my_Single_Item.StatusTime;
                    //Console.WriteLine(j + " - dt_StatusTime: " + dt_StatusTime);

                    String str_to = my_Single_Item.To;
                    //Console.WriteLine(j + " - " + str_to);

                    string str_myMessage = my_Single_Item.BodyText.PlainText;
                    //string str_myMessage = my_Single_Item.BodyText.HTMLText;



                    //string myURL = getURL_bodyText(str_myMessage, typeOfEmail);
                    string myURL = getURL_bodyText_2(str_myMessage);                                                        // 2014.10.16 Alopez We get the URL for all Lenguages.
                    emailAddress = get_CustomerEmail(str_myMessage);                                             // 2014.10.16 Alopez we get the customer email address


                    openFireFoxLogin(myURL);


                    //2014.11.04 Alopez we mark as read only when all the prozess was 100% finish
                    // we set this Email to already read
                    if (mark_Emails_As_Read == true) { my_Single_Item.Viewed = true; }

                }


            } // we have finished reading all the Emails in Folder [(@"\\david\david\archive\18\4\2\b\0"]  === [David://DAVID/Group/Development/Core Development/Google Externe Accounts/Einladungen]

            my_FireFoxDriver.Close();
        }



        //public static void insertThisInformation_inDataBase(string emailAddress, int ID_Email, string str_Subject, DateTime dt_sendtime, string str_From)
        public static void insertThisInformation_inDataBase()
        {
            // emailAddress = "info@mgm-technik.de";
            // 2014.10.16 Alopez we insert in [BarForce- dbo.ChaynsProductionStatus]  in column [GoogleAccountInvited]  Data Type bit ( true = 1  or false= 0)
            // -> we search for  emailAddress  in  Column [Playstore_AccountName] and insert 1 in column [GoogleAccountInvited]

            SqlConnection my_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            my_connection.Open();

            string my_sqlReadQuery = " USE BarForce SELECT ID, SiteID FROM dbo.[ChaynsProductionStatus]  (NOLOCK) WHERE [Playstore_AccountName] = '" + emailAddress + "'";

            SqlCommand my_command_toRead = new SqlCommand(my_sqlReadQuery, my_connection);
            SqlDataReader my_Reader = my_command_toRead.ExecuteReader();



            if (my_Reader.HasRows)
            {
                while (my_Reader.Read())
                {
                    int ID_Table = my_Reader.GetInt32(0);
                    string my_SiteID = my_Reader.GetString(1);


                    //In case we want to draw the Recordset and the type of Field in each Spalte
                    //for (int i = 0; i < my_Reader.FieldCount; i++)
                    //{
                    //    System.Console.WriteLine("{0}, ", my_Reader.GetSqlValue(i));
                    //    System.Console.Write("{0}, ", my_Reader.GetDataTypeName(i));
                    //}

                    update_to_1_table_ChaynsProductionStatus_colum_GoogleAccountInvited(ID_Table);                          //2014.10.16 Alopez we write always 1 in the Column
                    function_copyApp_toAnotherFolder(my_SiteID);
                }

                // 2014.10.15 Alopez we write a Log.
                // 2014.11.04 Alopez we dont write here the LOG we do in  check_Which_GooglePage_we_Get
                //string toWriteInLog = "alles OK. ID_Email: " + ID_Email + "  email: " + emailAddress + " Subject: " + str_Subject + "  Time: " + dt_sendtime + " from: " + str_From;
                //myLog_David_Selenium.direct_Write_txt(toWriteInLog);
            }

            else
            {
                //2014.10.16 Alopez  we write in LOG that this emailAddress is not in our DDBB
                string toWriteInLog = "Email not inside DDBB: " + ID_Email + "  Date: " + dt_sendtime + "  email: " + emailAddress + " Subject: " + str_Subject + " from: " + str_From;
                myLog_David_Selenium.direct_Write_txt(toWriteInLog);

            }



            my_Reader.Close();
            my_connection.Close();

        }



        public static void function_copyApp_toAnotherFolder(string my_SiteID)
        //2014.10.17 Alopez we make here the new Aufgabe the Jan gave me today.
        {
            DateTime? my_FirstDownloadBtnAndroidDate = null;
            int my_ProcessID;

            SqlConnection my_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            my_connection.Open();


            //string my_sqlReadQuery = "USE BarForce SELECT ProcessID, SiteID, FirstDownloadBtnAndroidDate  FROM dbo.[SlitteApp]  (NOLOCK) WHERE [SiteID] = '" + my_SiteID + "' ";
            string my_sqlReadQuery = "USE BarForce SELECT ProcessID, SiteID, FirstDownloadBtnAndroidDate  FROM dbo.[SlitteApp]  (NOLOCK) WHERE [SiteID] = '" + my_SiteID + "' and DownloadBtnAPK = 1";

            SqlCommand my_command_toRead = new SqlCommand(my_sqlReadQuery, my_connection);
            SqlDataReader my_Reader = my_command_toRead.ExecuteReader();



            if (my_Reader.HasRows)
            {
                while (my_Reader.Read())
                {
                    my_ProcessID = my_Reader.GetInt32(0);

                    try
                    {
                        my_FirstDownloadBtnAndroidDate = my_Reader.GetDateTime(2);                              //2014.10.17 Alopez here we recieve another value in case it is.
                    }

                    catch
                    {
                        my_FirstDownloadBtnAndroidDate = null;
                    }

                }

            }


            if (my_FirstDownloadBtnAndroidDate == null)
            {
                make_theCopyApp_toAnotherFolder(my_SiteID);
            }

            my_Reader.Close();
            my_connection.Close();
        }




        public static void make_theCopyApp_toAnotherFolder(string my_SiteID)
        {

            // 2014.10.17 Alopez we get the real App  and  copy/paste (never cut/paste)
            // from \\rfx-cloud2\APKs  in  Q:\#Android\Market\slitte\Ready for Upload

            bool exits_File;

            //string path_Origin = "\\rfx-cloud2\APKs";
            //string path_Destiny = "Q:\#Android\Market\slitte\Ready for Upload";

            string path_Origin = Path.GetFullPath("\\\\rfx-cloud2\\APKs");
            string path_Destiny = Path.GetFullPath("Q:\\#Android\\Market\\slitte\\Ready for Upload");

            string nameApp = my_SiteID + ".apk";


            exits_File = ExistsFile_inFolder(path_Origin, nameApp);

            if (exits_File == true)                                                   // 2014.10.17 Alopez if the File my_SiteID+".apk"  exits, we copy it.
            {
                string sourceFile = System.IO.Path.Combine(path_Origin, nameApp);
                string destFile = System.IO.Path.Combine(path_Destiny, nameApp);
                System.IO.File.Copy(sourceFile, destFile, true);
            }


        }



        private static bool ExistsFile_inFolder(string rootpath, string filename)
        {
            if (File.Exists(Path.Combine(rootpath, filename)))
                return true;

            foreach (string subDir in Directory.GetDirectories(rootpath, "*", SearchOption.AllDirectories))
            {
                if (File.Exists(Path.Combine(rootpath, filename)))
                    return true;
            }

            return false;
        }



        public static void update_to_1_table_ChaynsProductionStatus_colum_GoogleAccountInvited(int ID_Table)
        {

            SqlConnection my_connection = new SqlConnection("server=sql3; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            //SqlConnection my_connection = new SqlConnection("server=XD-AT-W8; database=BarForce; Trusted_Connection=yes; connection timeout=120");
            SqlCommand my_command_Update;

            // ej: USE BarForce update  dbo.[ChaynsProductionStatus] SET [GoogleAccountInvited] = 0  WHERE ID = '9760';

            string my_sqlquery_update = "USE BarForce update  dbo.[ChaynsProductionStatus] SET " +
                                        "[GoogleAccountInvited] = 1 " +
                                        "WHERE ID = " + ID_Table + "";


            my_connection.Open();
            my_command_Update = new SqlCommand(my_sqlquery_update, my_connection);
            my_command_Update.ExecuteNonQuery();
            my_connection.Close();

        }



        //public static string check_WhatKindOfEmail(string myText)                                      // 2014.10.16 Alopez old function
        //{
        //    string myFirstWord;
        //    myFirstWord = myText.Substring(0, myText.IndexOf(" "));

        //    if (myFirstWord == "Invitation") { myFirstWord = "England"; }
        //    if (myFirstWord == "Einladung")  { myFirstWord = "Deutschland";}
        //    if (myFirstWord == "Uitnodiging ") { myFirstWord = "Holland"; }

        //    return myFirstWord;
        //}




        public static void openFireFoxLogin(string myURL)
        {

            //bool isEmailField;
            //bool isAlreadyEmailField;
            //bool is_Ungultige_Einladung;






            //2014.10.14 Alopez we need  using OpenQA.Selenium.Support.UI for WebDriverWait;
            WebDriverWait my_Wait = new WebDriverWait(my_FireFoxDriver, TimeSpan.FromSeconds(5));

            try
            {
                my_FireFoxDriver.Navigate().GoToUrl(myURL);
            }
            catch
            {
                myLog_David_Selenium.direct_Write_txt("Problem:  Firefox can not open URL (openFireFoxLogin).  Email:" + ID_Email + " Date: " + dt_sendtime + "  Subject:" + str_Subject + " emailAddress:" + emailAddress);
            }

            //2014.11.04 Alopz here we have a new Problem. Sometimes google Play make us eather of this options:
            // 1- we have to login with user and password. And after that we need to logIn again only with password
            // 2- we havre to login only password
            // 3- we dont have to login and fireFox jumt to the next  email(URL) without any login.

            check_if_Page_needLogin_and_do_it();        // 2014.11.04 Alopez this is new, because sometime we need to logIn and sometimes not. Before we didnot have to log in always
            check_if_Page_needLogin_and_do_it();        // Some time we need to log 2 times one before 

            check_Which_GooglePage_we_Get();             // 2014.11.04 Alopez here we make the rest of the program: insert DDBB, copy Folder etc.


            //my_FireFoxDriver.Close();  // we close fireFox at the int_End of the Program, instead open a new one each time.
        }


        public static void check_if_Page_needLogin_and_do_it()
        {
            bool isEmailField;
            bool isAlreadyEmailField;


            //2014.10.15 Alopez we check if there is user Field (email)  or not
            try
            {
                var userNameField = my_FireFoxDriver.FindElementById("Email");
                isEmailField = true;

            }
            catch { isEmailField = false; }


            //2014.10.15 Alopez We get that User field (email), user is already in:
            try
            {
                var userName_alreadyIN = my_FireFoxDriver.FindElementById("reauthEmail");
                isAlreadyEmailField = true;
            }
            catch { isAlreadyEmailField = false; }



            //2014.10.15 Alopez. In case there is no user and pass.
            if ((isEmailField == true) && (isAlreadyEmailField == false))
            {
                //2014.10.15 Alopez The first time We have to logIn 2 times :(
                //2014.11.04 Alopez we pass more parameters to make the insert of the ddbb
                //logIn_FirstTime();
                //logIn_AlreadyUser();
                //logIn_FirstTime(emailAddress, ID_Email, str_Subject, dt_sendtime, str_From);
                //logIn_AlreadyUser(emailAddress, ID_Email, str_Subject, dt_sendtime, str_From);

                logIn_FirstTime();

            }


            //2014.10.15 Alopez. In case there is already user and NO pass.
            if (isAlreadyEmailField == true)
            {
                //2014.11.04 Alopez we pass more parameters to make the insert of the ddbb
                //logIn_AlreadyUser();
                logIn_AlreadyUser();
            }

        }






        //public static void check_Which_GooglePage_we_Get(string emailAddress, int ID_Email, string str_Subject, DateTime dt_sendtime, string str_From)
        public static void check_Which_GooglePage_we_Get()
        {
            //2014.11.04 Alopez now Google changes and we have to logIn every time, so we have to use this function check what we recieve.

            bool is_Ungultige_Einladung = false;
            bool is_google_Error_Page = false;
            bool is_normal_Page = false;

            string myString;



            WebDriverWait my_Wait = new WebDriverWait(my_FireFoxDriver, TimeSpan.FromSeconds(5));



            try //2014.11.04 Alopez, NORMAL or Ungultig Page
            {
                my_Wait.Until((d) => { try { return d.FindElement(By.ClassName("teaser")); } catch { return null; } });
                myString = my_FireFoxDriver.FindElementByClassName("teaser").Text;

                //Normal Website
                int myInt_1 = (myString.IndexOf("EINLADUNG", StringComparison.InvariantCultureIgnoreCase));
                int myInt_2 = (myString.IndexOf("ANGENOMMEN", StringComparison.InvariantCultureIgnoreCase));
                int myInt_3 = (myString.IndexOf("SIE", StringComparison.InvariantCultureIgnoreCase));

                if ((myInt_1 > -1) && (myInt_2 > -1) && (myInt_3 > -1))
                {
                    is_normal_Page = true;
                }




                //ungultig webpage
                int myInt_1b = (myString.IndexOf("keine", StringComparison.InvariantCultureIgnoreCase));
                int myInt_2b = (myString.IndexOf("gültige", StringComparison.InvariantCultureIgnoreCase));
                int myInt_3b = (myString.IndexOf("UNGÜLTIGE", StringComparison.InvariantCultureIgnoreCase));

                if ((myInt_1b > -1) && (myInt_2b > -1) && (myInt_3b > -1))
                {
                    is_Ungultige_Einladung = true;
                }

            }

            catch
            {
                is_Ungultige_Einladung = false;
                is_normal_Page = false;
            }




            try   //2014.11.04 Alopez.  Error_Page
            {
                // my_Wait.Until((d) => { try { return d.FindElement(By.ClassName("af-error-container")); } catch { return null; } });
                my_Wait.Until((d) => { try { return d.FindElement(By.Id("af-error-container")); } catch { return null; } });
                //myString = my_FireFoxDriver.FindElementByClassName("af-error-container").Text;

                is_google_Error_Page = true;
            }

            catch
            {
                is_google_Error_Page = false;
            }




            //2014.11.04 Alopez Here we know already in which page we are. Now We can: Log user pass, log only pass,  insert in BBDD ,  write in the LOG, etc.

            if (is_normal_Page == true)
            {
                // hacer insert DDBB aqui   
                //insertThisInformation_inDataBase(emailAddress, ID_Email, str_Subject, dt_sendtime, str_From);
                insertThisInformation_inDataBase();
                mark_Emails_As_Read = true;

                myLog_David_Selenium.direct_Write_txt("ok. Mail:" + ID_Email + "   Date:" + dt_sendtime + " emailAddress: " + emailAddress + " Einladung akzeptiert.");
            }


            if (is_Ungultige_Einladung == true)
            {
                //insertThisInformation_inDataBase(emailAddress, ID_Email, str_Subject, dt_sendtime, str_From);
                insertThisInformation_inDataBase();

                myLog_David_Selenium.direct_Write_txt("Ungultige_Einladung. Alles ok.  Mail" + ID_Email + "   Date:" + dt_sendtime + ":  emailAddress: " + emailAddress + " Die Einladung war schon bereits akzeptiert.");
                mark_Emails_As_Read = true;

            }


            if (is_google_Error_Page == true)
            {
                myLog_David_Selenium.direct_Write_txt("Error Google_page. Error 500. Mail:" + ID_Email + "   Date:" + dt_sendtime + " emailAddress: " + emailAddress + " Einladung unmanaged");
                mark_Emails_As_Read = false;
            }





        }




        //public static void logIn_FirstTime(string emailAddress, int ID_Email, string str_Subject, DateTime dt_sendtime, string str_From)
        public static void logIn_FirstTime()
        {

            try
            {

                //We get User Email field
                var userNameField = my_FireFoxDriver.FindElementById("Email");
                var userPasswordField = my_FireFoxDriver.FindElementById("Passwd");
                var loginButton = my_FireFoxDriver.FindElementByName("signIn");

                userNameField.SendKeys("playstore.upload@chayns.net");
                userPasswordField.SendKeys("Pleonasmus913");


                loginButton.Submit();

                // hacer insert DDBB aqui
                // insertThisInformation_inDataBase(myCustomer_Email, int_ID, str_Subject, dt_sendtime, str_From);         // 2014.10.16 Alopez we insert Info in DDBB

                //2014.11.04 Alopez We make the insert only by logIn_AlreadyUser, beacuse ALWAYS AFTER ONE logIn_FirstTime,  the webSite goes to logIn_AlreadyUser
                // the process to logIn is alwways in 2 steps.
                //insertThisInformation_inDataBase(emailAddress, ID_Email, str_Subject, dt_sendtime, str_From);

                // mark_Emails_As_Read = true;

            }

            catch
            {
                mark_Emails_As_Read = false;
                myLog_David_Selenium.direct_Write_txt("Problem:  We can not logIn the  first Time.  Email:" + ID_Email + " Date: " + dt_sendtime + "  Subject:" + str_Subject + " emailAddress:" + emailAddress);
            }

        }



        //public static void logIn_AlreadyUser(string emailAddress, int ID_Email, string str_Subject, DateTime dt_sendtime, string str_From) 
        public static void logIn_AlreadyUser()                                                          //2014.10.15 Alopez. we make log in when user is already in.
        {


            try
            {
                //We get User Email field
                //var userNameField = my_FireFoxDriver.FindElementById("Email");
                //userNameField.SendKeys("playstore.upload@chayns.net");

                var userPasswordField = my_FireFoxDriver.FindElementById("Passwd");
                var loginButton = my_FireFoxDriver.FindElementByName("signIn");

                userPasswordField.SendKeys("Pleonasmus913");
                loginButton.Submit();


                //check_Which_GooglePage_we_Get(emailAddress, ID_Email, str_Subject, dt_sendtime, str_From);  // we do it after we log

            }

            catch
            {

                mark_Emails_As_Read = false;
                myLog_David_Selenium.direct_Write_txt("Problem:  We can not logIn.  Email:" + ID_Email + " Date: " + dt_sendtime + "  Subject:" + str_Subject + " emailAddress:" + emailAddress);

            }



        }




        public static string get_CustomerEmail(string myText)
        {
            myText = myText.Replace("\r", " ");                                                         //2014.10.16 Alopez we take out the character "\r" and "\n"
            myText = myText.Replace("\n", " ");

            int i1_at = myText.IndexOf("@");

            int i2_at = myText.IndexOf(" ", i1_at);

            string finalPart = myText.Substring((i1_at), (i2_at - i1_at));
            string allTextBefore = myText.Substring(0, i1_at);

            int int_lastSpace = allTextBefore.LastIndexOf(" ");                                         // we get always index of last " ", so no worries in  i +1 we are in @emailaddress

            string firstPart = myText.Substring(int_lastSpace + 1, (i1_at - int_lastSpace) - 1);

            string URL = firstPart + finalPart;

            return URL;
        }


        public static string getURL_bodyText_2(string myText)
        {
            bool myTest;


            myText = myText.Replace("\r", " ");
            myText = myText.Replace("\n", " ");


            int int_Pos_1 = myText.IndexOf("https://play.google.com/apps/publish/acceptInvitation?");   //2014.10.16 Alopez we get the index where starts the URL.
            int int_Pos_2 = myText.IndexOf(" ", int_Pos_1);                                             //2014.10.16 Alopez get index os the next "".

            myText = myText.Substring(int_Pos_1, int_Pos_2 - int_Pos_1);                                //2014.10.16 Alopez we get what it's between index int_Pos_1 and int_Pos_2


            //some clean of the URL
            myTest = myText.Substring(myText.Length - 1, 1).Equals("_");                                //2014.10.16 Alopez in case we have one last character _
            if (myTest == true) { myText = myText.Substring(0, myText.Length - 1); }

            myTest = myText.Substring(myText.Length - 1, 1).Equals(".");                                //2014.10.16 Alopez in case we have one last character .
            if (myTest == true) { myText = myText.Substring(0, myText.Length - 1); }

            return myText;
        }


        //public static string getURL_bodyText(string myText, string myFirstWord)                        // 2014.10.16 Alopez old function
        //{
        //    //string strStart;
        //string strEnd;
        //int int_Start, int_End;
        //bool flag_stringHasText_1;
        //bool flag_stringHasText_2;

        //if (typeEmail == "Einladung")
        //{
        //    strStart = "Um die Einladung anzunehmen, besuchen Sie";
        //    strEnd = "Falls Sie auf keinen der oben aufgeführten Links klicken können";
        //}

        //2014.10.15 Alopez we start assign that we are in Einladung
        //strStart = "Um die Einladung anzunehmen, besuchen Sie";
        //strEnd = "Falls Sie auf keinen der oben aufgeführten Links klicken können";

        //if (myFirstWord == "England")
        //{
        //    strStart = "To accept your invitation, visit";
        //    strEnd = "If you're unable to click on any";
        //}

        //if (myFirstWord == "Holland")
        //{
        //    strStart = "Als u de uitnodiging wilt accepteren, gaat u naar";
        //    strEnd = "Als geen enkele van";
        //}



        //flag_stringHasText_1 = myText.Contains(strStart);
        //flag_stringHasText_2 = myText.Contains(strEnd);

        //if ((flag_stringHasText_1) == true && (flag_stringHasText_2 == true))
        //{
        //    int_Start = myText.IndexOf(strStart, 0) + strStart.Length;
        //    int_End = myText.IndexOf(strEnd, int_Start);
        //    myText = myText.Substring(int_Start, int_End - int_Start);
        //}
        //else
        //{
        //    return "";
        //}

        //string demo = "demo";

        //const string quote = "\"";
        //myText = quote + myText + quote;





        ////Last check ArgumentOutOfRangeException the text
        //myText = myText.TrimEnd('.');
        //myText = myText.Trim();
        //bool test1 = myText.Substring(myText.Length - 2, 2).Equals("\r\n"); // 2014.10.15 Alopez If is true we take out the last 2 characters \r\n. Always comes with them
        //if (test1 == true) { myText = myText.Substring(0, myText.Length - 2); }

        ////Console.WriteLine(myText);
        //return myText;
        //}

        public static string changeStringToDoubleSlash(string myText)
        {
            myText = myText.Replace(@"\", @"\\");
            return myText;
        }


    }
}
 
