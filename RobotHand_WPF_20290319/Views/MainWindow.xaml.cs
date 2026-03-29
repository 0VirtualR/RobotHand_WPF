using Prism.Events;
using RobotHand_WPF_20290319.Extensions;
using RobotHand_WPF_20290319.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotHand_WPF_20290319.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEventAggregator eventAggregator;

        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            eventAggregator.RegisterMsg(arg =>
            {
                Snackbar.MessageQueue.Enqueue(arg.Msg);
            });
            MenuBar.SelectionChanged += (s, e) =>
            {
                drawerHost.IsRightDrawerOpen = false;
            };

            #region 主界面中的基础按钮
            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };
            btnClose.Click += async (s, e) =>
            {
                //var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                //if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close();
            };
            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };

            ColorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                    this.WindowState = WindowState.Maximized;
                else
                    this.WindowState = WindowState.Normal;
            };
            this.eventAggregator = eventAggregator;
            #endregion
        }
    }
}