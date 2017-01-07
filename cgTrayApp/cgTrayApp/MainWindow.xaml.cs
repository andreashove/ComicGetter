using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System;
using Hardcodet.Wpf.TaskbarNotification;
using System.IO;

namespace cgTrayApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    


    public partial class MainWindow : Window
    {
        /*
        private TaskbarIcon tb;

        private void InitApplication()
        {
            //initialize NotifyIcon
            tb = (TaskbarIcon)FindResource("MyNotifyIcon");
        }*/
        public MainWindow()
        {
            
            InitializeComponent();
            //MoveCurrentImageToFavs();
            //Environment.Exit(0);
            
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MoveCurrentImageToFavs();
        }
        
        
        void MoveCurrentImageToFavs()
        {
            string sourcePath = "c:\\ComicGetter\\img";
            string targetPath = "c:\\ComicGetter\\favs";
            string fileToCopy = "comicstrip.jpg";

            // count # of images in favs folder
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(targetPath);
            int count = dir.GetFiles().Length;

            System.Windows.MessageBox.Show(count.ToString());
            int nextFavFileId = count+1;
            string newFileName = nextFavFileId.ToString();
            newFileName += ".gif";


            string sourceFile;
            string targetFile;
            //System.Windows.MessageBox.Show("clicked!");
            
            sourceFile = Path.Combine(sourcePath, fileToCopy);
            targetFile = Path.Combine(targetPath, newFileName);

            File.Copy(sourceFile, targetFile, true);
           
            
        }
        /*

        
private void Button_Click(object sender, RoutedEventArgs e)
{
   this.WindowState = System.Windows.WindowState.Minimized;
   this.nIcon.Icon = new Icon(@"../../st_le_.ico");
   this.nIcon.ShowBalloonTip(5000, "Hi", "This is a BallonTip from Windows Notification", ToolTipIcon.Info);
   this.nIcon.Visible = true;
}
*/
    }
}
