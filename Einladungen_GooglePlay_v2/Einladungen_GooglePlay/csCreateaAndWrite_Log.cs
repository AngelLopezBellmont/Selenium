using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



// Version 2014.10.20 Alopez
namespace myNameSpace_csCreateaAndWrite_Log
{
    using System.IO;

    public class csCreateaAndWrite_Log
    {

        //public string txt_Name;
        public static string my_path;
        public static string my_path_File;

        public csCreateaAndWrite_Log()
        {
            string txt_Name = "Log.txt";

            my_path = Directory.GetCurrentDirectory();

            my_path = if_Folder_Debug_or_Release_getParentFolder(my_path);


            my_path_File = my_path + @"\" + txt_Name;

            look_ifCreated_ifNot_Create_IT(my_path_File);
            //direct_Write_txt(my_path_File);
        }


        public csCreateaAndWrite_Log(string txt_Name)
        {
            txt_Name = hatNameTXTalready(txt_Name);                                 // if has .txt we let it, if not concatenate ".txt"

            //txt_Name = txt_Name + ".txt";
            //txt_Name = "Log.txt";
            my_path = Directory.GetCurrentDirectory();

            my_path = if_Folder_Debug_or_Release_getParentFolder(my_path);

            my_path_File = my_path + @"\" + txt_Name;

            look_ifCreated_ifNot_Create_IT(my_path_File);
            //direct_Write_txt(my_path_File);
        }


        public static string  hatNameTXTalready(string str_name)
        {
            // we check of the name comes already with .txt at the end or not. 
            //In case not we concatenate .txt to teh name at the end
            // if yes we let the name equal

            string last4_Character_name = str_name.Substring(str_name.Length - 4, 4);

            if (last4_Character_name ==".txt")
            {

                //str_name = str_name;
            }

            else
            {
                str_name = str_name + ".txt";

            }

            return str_name;

        }





        public static string if_Folder_Debug_or_Release_getParentFolder(string my_path)

        // If run one .exe and the project hat this Class, the log is created where the is .exe.
        // But if we run from Visual Studio Console in order to avoid to generate one Log in Debug folder (if we run in Debug) and other in Release Folder (if we run in release)
        // we let the Log in the parent Folder bin, at the same lever that Debug and Release Folder.

        {
            my_path = Directory.GetCurrentDirectory();

            string folderName = Path.GetFileName(my_path);

            if ((folderName == "Release") || (folderName == "Debug"))
            {
                DirectoryInfo my_Dolder_Parent = Directory.GetParent(my_path);
                my_path = my_Dolder_Parent.FullName;
            }


            return my_path;
        }





        public static void look_ifCreated_ifNot_Create_IT(string my_path_File)
        {

            if (!File.Exists(my_path_File))
            {
                File.Create(my_path_File).Dispose();
                TextWriter tw = new StreamWriter(my_path_File);
                tw.WriteLine("Log to check the process of the program:");
                tw.Close();
            }
        }




        public void direct_Write_txt(string textToWrtite)                           //no pude ser static pq sino no llama a los constructores y no recibe path etc.
        {
            TextWriter tw = new StreamWriter(my_path_File, true);
            tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff  - ") + textToWrtite);
            tw.Close();

        }
    }
}
