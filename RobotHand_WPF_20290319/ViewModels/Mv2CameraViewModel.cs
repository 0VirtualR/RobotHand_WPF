using MvCameraLibrary.Interfaces;
using MvCameraLibrary.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RobotHand_WPF_20290319.ViewModels
{
    public class Mv2CameraViewModel:BindableBase,IDisposable
    {
        private readonly DispatcherTimer _statTimer;
        private readonly IMvCameraService cameraService;
        private BitmapSource _imageSource;
        public BitmapSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private string _statText;
        public string StatText
        {
            get => _statText;
            set => SetProperty(ref _statText, value);
        }

        public DelegateCommand InitializeCommand { get; }
        public DelegateCommand StartCommand { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand SettingCommand { get; }
        public DelegateCommand SnapCommand { get; }

        public Mv2CameraViewModel(IMvCameraService cameraService)
        {
            cameraService = cameraService;
            cameraService.FrameReceived += CameraService_FrameReceived;

            InitializeCommand = new DelegateCommand(Initialize);
            StartCommand = new DelegateCommand(() => cameraService.StartLive());
            StopCommand = new DelegateCommand(() => cameraService.StopLive());
            SettingCommand = new DelegateCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    cameraService.ShowSettingPage();
                });
            });
            SnapCommand = new DelegateCommand(Snap);

            _statTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _statTimer.Tick += StatTimer_Tick;
            _statTimer.Start();
            this.cameraService = cameraService;
        }

        private void Initialize()
        {
            if (cameraService.Initialize())
            {
                StatusText = "相机初始化成功";
                cameraService.StartLive();
            }
            else
            {
                StatusText = "未扫描到相机或打开失败";
            }
        }

        private void Snap()
        {
            string fileName = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"{DateTime.Now:yyyyMMdd_HHmmss_fff}.bmp");

            bool ok = cameraService.Snap(fileName);
            StatusText = ok ? $"已保存：{fileName}" : "Snap failed";
        }

        private void CameraService_FrameReceived(object sender, CameraFrameEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ImageSource = e.Frame;
                StatusText = $"Resolution: {e.Width}*{e.Height}";
            }));
        }

        private void StatTimer_Tick(object sender, EventArgs e)
        {
            if (!cameraService.IsOpened)
            {
                StatText = string.Empty;
                return;
            }

            var stat = cameraService.GetStatistics();
            StatText = stat.ToString();
        }

        public void Dispose()
        {
            _statTimer.Stop();
            _statTimer.Tick -= StatTimer_Tick;
            cameraService.FrameReceived -= CameraService_FrameReceived;
            cameraService.StopLive();
        }
    }
}
