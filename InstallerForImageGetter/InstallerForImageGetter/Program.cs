using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.TaskScheduler;

namespace InstallerForImageGetter
{
    class Program
    {
        private static void CreateTaskInScheduler()
        {
            using (TaskService ts = new TaskService())
            {
                TimeTrigger tt = new TimeTrigger();

                tt.StartBoundary = DateTime.Now + TimeSpan.FromMinutes(0.05);
                tt.Repetition.Interval = TimeSpan.FromMinutes(1);
                tt.Repetition.Duration = TimeSpan.FromDays(1);
                tt.Id = DateTime.Now.ToString();
                      
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Updates your desktop background with todays Lunch comic strip from Dagbladet.no";
                // do not stop the task if computer switches to battery mode.
                td.Settings.StopIfGoingOnBatteries = false;

                // do not prevent the task from starting if laptop on battery mode.
                td.Settings.DisallowStartIfOnBatteries = false;

                // task will only start if computer is connected to the internet.
                td.Settings.RunOnlyIfNetworkAvailable = true;
                td.Triggers.Add(tt);                

                td.Actions.Add(new ExecAction("c:\\ComicGetter\\bin\\ComicGetter.exe", null));
                ts.RootFolder.RegisterTaskDefinition(@"ComicGetter", td);
            }
        }
        static void Main(string[] args)
        {
            string[] comics = { "Lunch", "Rocky" };
            Console.WriteLine("Installing ComicGetter v0.2");
            Console.WriteLine("This program downloads a new comic strip every \nday and sets it as your desktop background.");
            Console.WriteLine("");
            Console.WriteLine("** PLEASE CHOOSE ONE OF THE FOLLOWING COMICS **");
            Console.WriteLine("");
            Console.WriteLine("You have the following options:");
            for (int i = 0; i<comics.Length; i++)
            {
                Console.WriteLine(i + 1 + " - " + comics[i]);
            }
            Console.WriteLine("");
            Console.WriteLine("** PLEASE CHOOSE ONE OF THE FOLLOWING COMICS **");
            Console.WriteLine("");
            Int32 number = 1;
            bool correctInput = false;
            while (!correctInput)
            {
                Console.Write("Your choice (1-2): ");
                var choice = Console.ReadKey();
                Console.WriteLine("\n");
                
                if (Int32.TryParse(choice.KeyChar.ToString(), out number))
                {
                    if (number >= 1 && number <= comics.Length)
                        correctInput = true;
                    else
                        Console.WriteLine("Invalid input: Please enter a number between 1 and 2.");
                }
                else
                {
                    Console.WriteLine("Invalid input: Please enter a number between 1 and 2.");
                }
            }
            var userChoice = comics[number - 1];
            Console.WriteLine("You chose: " + userChoice + ". ");
            Console.WriteLine("Press Enter to continue installation, or close this window to abort.");
            Console.ReadLine();
            Console.WriteLine("Installing the ComicGetter...\n");
            Directory.CreateDirectory("c:\\ComicGetter");
            Console.WriteLine("\nDirectory created: C:\\ComicGetter");
            Directory.CreateDirectory("c:\\ComicGetter\\bin");
            Console.WriteLine("Directory created: C:\\ComicGetter\\bin");
            Directory.CreateDirectory("c:\\ComicGetter\\img");
            Console.WriteLine("Directory created: C:\\ComicGetter\\img");

            string configFile = Directory.GetCurrentDirectory() + "\\config.txt";
            /*
            if (File.Exists(configFile))
                File.Delete(configFile);

            File.CreateText(Directory.GetCurrentDirectory() + "\\config.txt");

            using (StreamWriter file = new StreamWriter(configFile))
            {
                file.WriteLine(userChoice);
            }
            string sourcePath = Directory.GetCurrentDirectory();
            string targetPath = "c:\\ComicGetter\\bin";
            string[] filesToCopy = { "ComicGetter.exe", "Microsoft.Win32.TaskScheduler.dll" , "Uninstall.exe" , "howtouninstall.txt", "README.txt", "config.txt" };

            string sourceFile;
            string targetFile;

            foreach (string str in filesToCopy)
            {
                sourceFile = Path.Combine(sourcePath, str);
                targetFile = Path.Combine(targetPath, str);
                try
                {
                    File.Copy(sourceFile, targetFile, true);
                }
                catch
                {
                    Console.WriteLine("\n********** INSTALLATION ERROR **********\n");
                    Console.WriteLine(" - Unable to copy files to directory.");
                    Console.WriteLine(" - Make sure to run this installer as administrator.");
                    Console.WriteLine(" - (Right Click --> Run as Administrator)");
                    Console.WriteLine(" - If the issue persists, come find me and hit me (gently) in the face");
                    Console.WriteLine("\n********** INSTALLATION ERROR **********");
                    Console.WriteLine("\nPress Enter to quit...");
                    Console.ReadLine();
                    return;
                }
            }
            /*string sourceFile = Path.Combine(sourcePath, fileName);
            string destFile = Path.Combine(targetPath, fileName);

            string sourceDll = Path.Combine(sourcePath, dllName);
            string destDll = Path.Combine(targetPath, dllName);

            string uninstallFile = Path.Combine(sourcePath, uninstallName);
            string uninstallDest = Path.Combine(targetPath, uninstallName);

            string howtoFile = Path.Combine(sourcePath, untxt);
            string howtoFileDest = Path.Combine(targetPath, untxt);

            string readmeFile = Path.Combine(sourcePath, readmeName);
            string readmeFileDest = Path.Combine(targetPath, readmeName);
            
            try
            {
                File.Copy(sourceFile, destFile, true);
                File.Copy(sourceDll, destDll, true);
                File.Copy(uninstallFile, uninstallDest, true);
                File.Copy(howtoFile, howtoFileDest, true);
                File.Copy(readmeFile, readmeFileDest, true);
            }

            catch
            {
                Console.WriteLine("\n********** INSTALLATION ERROR **********\n");
                Console.WriteLine(" - Unable to copy files to directory.");
                Console.WriteLine(" - Please run this installer as administrator.");
                Console.WriteLine(" - (Right Click --> Run as Administrator)");
                Console.WriteLine(" - If the issue persists, come find me and hit me (gently) in the face");
                Console.WriteLine("\n********** INSTALLATION ERROR **********");
                Console.WriteLine("\nPress Enter to quit...");
                Console.ReadLine();
                return;
            }
            */
            Console.WriteLine("File copied: Executable file to C:\\ComicGetter\\bin");
            Console.WriteLine("File copied: Uninstaller to C:\\ComicGetter\\bin");
            CreateTaskInScheduler();
            Console.WriteLine("Task scheduled to run once per day.");
            Console.WriteLine("\nInstall completed! New comic will appear soon.");
            Console.WriteLine("Press Enter to quit...");
            Console.ReadLine();
        }
    }
}
