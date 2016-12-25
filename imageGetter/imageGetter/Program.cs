using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace imageGetter
{
    class Program
    {
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
        private static void UpdateTaskInScheduler()
        {
            using (TaskService ts = new TaskService())
            {
                // get an instance of an existing task
                Task myTask = ts.GetTask("ComicGetter");

                TimeTrigger tt = new TimeTrigger();

                // postpone the task till next morning at 04:30.
                tt.StartBoundary = DateTime.Today.AddDays(1) + TimeSpan.FromHours(4.5);
                tt.Repetition.Interval = TimeSpan.FromMinutes(1);
                tt.Repetition.Duration = TimeSpan.Zero;
                
                // do not stop the task if computer switches to battery mode.
                myTask.Definition.Settings.StopIfGoingOnBatteries = false;
                myTask.Definition.Settings.DisallowStartIfOnBatteries = false;
                myTask.Definition.Settings.RunOnlyIfNetworkAvailable = true;

                // task will run whether user is logged in or not.
                // THIS IS ONLY SUPPORTED UP TO WINDOWS XP. LATER WINDOWS REQUIRE 
                // PASSWORD VERIFICATION IN ORDER TO RUN IT. 
                //              myTask.Definition.Settings.RunOnlyIfLoggedOn = false;

                // check to ensure you have a trigger ... 
                if (myTask.Definition.Triggers.Count > 0)
                {
                    // ... and it is the one want
                    if (myTask.Definition.Triggers[0] is TimeTrigger)
                    {
                        myTask.Definition.Triggers.RemoveAt(0);
                        myTask.Definition.Triggers.Add(tt);
                    }
                }

                // Register the changes (Note: if there is a password associated with the task, 
                // you will need to register using the TaskFolder.RegisterTaskDefinition method.
                myTask.RegisterChanges();
            }
            
        }
        static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            string comicName = File.ReadAllText(currentDirectory+"\\config.txt");
            var todayUnixTimestamp = DateTimeToUnixTimestamp(DateTime.Now.Date);
            string saveloc = "c:\\ComicGetter\\img\\comicstrip.gif";

            if (File.Exists(saveloc))
               File.Delete(saveloc);
             
            string webpage = "http://www.dagbladet.no/tegneserie/pondusarkiv/serveconfig.php?date="+todayUnixTimestamp+"&strip="+comicName;
            
            DirectoryInfo di = new DirectoryInfo("c:\\ComicGetter\\img");
            FileInfo[] fi = di.GetFiles();

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            

            int retriesLeft = 3;
            while (retriesLeft > 0)
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(webpage, saveloc);
                }

                // only one file in the img directory at any time, 
                // if the length of the downloaded files is 0 that means 
                // the download failed and an empty jpeg was created.
                // only setting background if the size is bigger than 0.
                fi = di.GetFiles();
                if (fi[0].Length != 0)
                {
                    key.SetValue(@"WallpaperStyle", 1.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                    Wallpaper.SetDesktopWallpaper(saveloc);
                    UpdateTaskInScheduler();                
                    return;
                }
                else
                {
                    retriesLeft--;
                }
            }

            // no retries left, download has not succeeded.
            UpdateTaskInScheduler(); // redo tomorrow
        }
    }
    
    public sealed class Wallpaper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction,int uParam, string lpvParam, int fuWinIni);

        private static readonly int SPI_SETDESKWALLPAPER = 0x14;
        private static readonly int SPIF_UPDATEINIFILE = 0x01;
        private static readonly int SPIF_SENDWININICHANGE = 0x02;

        public static void SetDesktopWallpaper(string filename)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filename, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
       
    }
}