using System;
using Microsoft.Win32.TaskScheduler;
using System.IO;

namespace uninstallerForImageGetter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("!!! PLEASE READ THIS !!!");
            Console.WriteLine("Delete the ComicGetter directory after this uninstaller is finished.");
            Console.WriteLine("!!! PLEASE READ THIS !!!");
            Console.Write("\nPress Enter to continue or close this window to abort.");
            Console.ReadLine();
            using (TaskService ts = new TaskService())
            {
                try
                {
                    ts.RootFolder.DeleteTask("ComicGetter");

                }
                catch (FileNotFoundException fnfe)
                {
                    Console.WriteLine("ERROR: Could not uninstall the daily updating task. Probably the task is already deleted.");
                }

            }
            Console.WriteLine("Uninstall completed!. Press Enter to quit.");
            Console.ReadLine();
        }
    }
}
