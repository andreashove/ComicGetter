using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace imageGetter
{
    class ComicGetter
    {

        static void DownloadComics()
        {
            var todaysDate = DateTime.Today.ToString("yyyy-MM-dd");
            string[] comics = new string[] { "lunch", "dilbert" };
            var webpage = "https://www.tu.no/?module=TekComics&service=image&id=";
            var errorImageFilePath = "C:\\Temp\\Comics\\error\\error.png";
            var storageFolder = "C:\\Temp\\Comics\\" + todaysDate + "\\";

            var combinedImage = storageFolder + "combinedImage.jpg";
            if (File.Exists(combinedImage))
            {
                File.Delete(combinedImage);
            }

            if (!Directory.Exists(storageFolder))
            {
                Directory.CreateDirectory(storageFolder);
            }

            foreach (string comic in comics)
            {
                var pathDownloadedFile = storageFolder + comic + ".jfif";
                var downloadURL = webpage + comic + "&key=" + todaysDate;
                bool downloadFailed = false;

                for (int i = 0; i < 3; i++)
                {
                    downloadFailed = false;
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFile(downloadURL, pathDownloadedFile);
                        }
                    }
                    catch (Exception)
                    {
                        downloadFailed = true;
                        continue;
                    }
                    

                    FileInfo fi = new FileInfo(pathDownloadedFile);

                    if (fi.Length > 0)
                    {
                        break;
                    }

                    if (i == 2)
                    {
                        downloadFailed = true;
                    }

                }
                if (downloadFailed)
                {
                    File.Copy(errorImageFilePath, pathDownloadedFile);
                }

            }
            

            string combinedImageFilePath = CombineImagesVertically(storageFolder);
            Wallpaper.SetDesktopWallpaper(combinedImageFilePath);

        }

        private static string CombineImagesVertically(string folderPath)
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo d = new DirectoryInfo(folderPath);

            foreach (var file in d.GetFiles())
            {
                files.Add(file);
            }

            string combinedImage = folderPath + "combinedImage.jpg";

            List<int> imageHeights = new List<int>();
            List<int> imageWidths = new List<int>();

            int nIndex = 0;
            //int width = 0;
            int height = 0;
            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                imageWidths.Add(img.Width);
                height += img.Height;
                img.Dispose();
            }
            imageWidths.Sort();
            imageHeights.Sort();
            int width = imageWidths[imageWidths.Count - 1];


            System.Drawing.Bitmap img3 = new System.Drawing.Bitmap((int)width + 250, (int)height + 150);
            using (Graphics gfx = Graphics.FromImage(img3))
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
            }



            Graphics g = Graphics.FromImage(img3);
            g.Clear(Color.White);

            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                if (nIndex == 0)
                {
                    g.DrawImage(img, new Point(0, 0));
                    nIndex++;
                    height = img.Height;
                }
                else
                {
                    g.DrawImage(img, new Point(0, height));
                    height += img.Height;
                }
                img.Dispose();
            }
            g.Dispose();

            img3.Save(combinedImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            img3.Dispose();
            return combinedImage;
        }
        /*
        private static void CombineImagesHorizontally(FileInfo[] files)
        {
            //change the location to store the final image.
            string finalImage = "C:\\Temp\\FinalImage.jpg";
            List<int> imageHeights = new List<int>();
            int nIndex = 0;
            int width = 0;
            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                imageHeights.Add(img.Height);
                width += img.Width;
                img.Dispose();
            }
            imageHeights.Sort();
            int height = imageHeights[imageHeights.Count - 1];

            System.Drawing.Bitmap img3 = new System.Drawing.Bitmap((int)width, (int)height);
            Graphics g = Graphics.FromImage(img3);
            g.Clear(SystemColors.AppWorkspace);
            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                if (nIndex == 0)
                {
                    g.DrawImage(img, new Point(0, 0));
                    nIndex++;
                    width = img.Width;
                }
                else
                {
                    g.DrawImage(img, new Point(width, 0));
                    width += img.Width;
                }
                img.Dispose();
            }
            g.Dispose();
            img3.Save(finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            img3.Dispose();
            //imageLocation.Image = Image.FromFile(finalImage);
        }
        */
        private static void UpdateTaskInScheduler()
        {
            using (TaskService ts = new TaskService())
            {
                Task myTask = ts.GetTask("ComicGetter");
                TimeTrigger tt = new TimeTrigger();

                tt.StartBoundary = DateTime.Today.AddDays(1) + TimeSpan.FromHours(4.5);
                tt.Repetition.Interval = TimeSpan.FromMinutes(1);
                tt.Repetition.Duration = TimeSpan.Zero;

                myTask.Definition.Settings.StopIfGoingOnBatteries = false;
                myTask.Definition.Settings.DisallowStartIfOnBatteries = false;
                myTask.Definition.Settings.RunOnlyIfNetworkAvailable = true;

                if (myTask.Definition.Triggers.Count > 0)
                {
                    if (myTask.Definition.Triggers[0] is TimeTrigger)
                    {
                        myTask.Definition.Triggers.RemoveAt(0);
                        myTask.Definition.Triggers.Add(tt);
                    }
                    else
                    {
                        throw new SystemException("Could not find correct trigger to update");
                    }

                }

                myTask.RegisterChanges();
            }
        }



        static void Main(string[] args)
        {
            Initialize();
            DownloadComics();
            UpdateTaskInScheduler(); 

        }

        static void Initialize()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 1.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
        }

        public sealed class Wallpaper
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

            private static readonly int SPI_SETDESKWALLPAPER = 0x14;
            private static readonly int SPIF_UPDATEINIFILE = 0x01;
            private static readonly int SPIF_SENDWININICHANGE = 0x02;

            public static void SetDesktopWallpaper(string filename)
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filename, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }

        }


    }
}