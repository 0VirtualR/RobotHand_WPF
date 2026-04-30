using CameraLibrary.Interface;
using Prism.Commands;
using Prism.Mvvm;
using RobotHand_WPF_20290319.Extensions.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RobotHand_WPF_20290319.ViewModels
{
    public class MvCameraViewModel : BindableBase
    {

        private BitmapSource _imageSource;
        public BitmapSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private string _statusText;
        private readonly CameraLibrary.Interface.ICameraService _cameraService;

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public DelegateCommand InitializeCommand { get; }
        public DelegateCommand StartCommand { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand SettingCommand { get; }
        public DelegateCommand SnapCommand { get; }

        public MvCameraViewModel(CameraLibrary.Interface.ICameraService cameraService)
        {
            _cameraService = cameraService;
            _cameraService.FrameReceived += CameraService_FrameReceived;

            InitializeCommand = new DelegateCommand(Initialize);
            StartCommand = new DelegateCommand(() => _cameraService.StartLive());
            StopCommand = new DelegateCommand(() => _cameraService.StopLive());
            SettingCommand = new DelegateCommand(() => _cameraService.ShowSettingPage());
            SnapCommand = new DelegateCommand(Snap);
        }

        private void Initialize()
        {
            if (_cameraService.Initialize())
            {
                StatusText = "相机初始化成功";
                _cameraService.StartLive();
            }
            else
            {
                StatusText = "未找到相机或打开失败";
            }
        }

        private void Snap()
        {
            string fileName = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"{DateTime.Now:yyyyMMdd_HHmmss}.bmp");

            bool ok = _cameraService.Snap(fileName);
            StatusText = ok ? $"已保存：{fileName}" : "Snap failed";
        }

        private void CameraService_FrameReceived(object sender, CameraFrameEventArgs e)
        {
            // 回到 UI 线程更新绑定属性
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ImageSource = e.Frame;
                StatusText = $"Resolution: {e.Width}*{e.Height}";
            }));
        }
    }
}
