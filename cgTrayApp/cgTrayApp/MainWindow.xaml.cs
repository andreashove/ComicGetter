
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System;
using Hardcodet.Wpf.TaskbarNotification;

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
