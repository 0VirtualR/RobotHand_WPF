using ImTools;
using MVSDK;
using Prism.Commands;
using Prism.Mvvm;
using RobotHand_WPF_20290319.Extensions.SerialPorts;
using RobotHand_WPF_20290319.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using CameraHandle = System.Int32;
using Prism.Regions;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using RobotHand_WPF_20290319.Extensions.Camera;
using System.Windows.Media;
using System.Windows.Shapes;
using RobotHand_20260313.Extensions;

namespace RobotHand_WPF_20290319.ViewModels
{
    public class CameraViewModel : BindableBase, IDisposable
    {
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();
        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand InitCommand { get; set; }
        public DelegateCommand ConnectCommmand { get; set; }

        public CameraViewModel(ISerialPortService serialPortService,ICameraService cameraService)
        {
            this.serialPortService = serialPortService;
            this.cameraService = cameraService;
            cameraService.FrameReceived += OnFrameReceived;

            StartCommand = new DelegateCommand(Start);
            InitCommand = new DelegateCommand(Init);
            ConnectCommmand = new DelegateCommand(Connect);

            LoadAvailablePorts();
            cameraService.Initialize();
        }


        private void Init()
        {
            //打开初始化窗口 进行十个点的定位
          
        }
        private void Start()
        {
            if (IsStartWork == false)
            {
                if (!serialPortService.IsOpen)
                {
                    AddLog("串口没有打开！");
                    return;
                }
              
                IsStartWork = true;

                BtnStateMsg = "停止程序";
            }
            else
            {
                IsStartWork = false;
                BtnStateMsg = "开始程序";
                if (serialPortService.IsOpen)
                    CLoseRobotPort();
            }

        }
        private bool isStartWork;
        public bool IsStartWork
        {
            get { return isStartWork; }
            set { SetProperty(ref isStartWork, value); }
        }
        //开始按钮的字符串提示消息
        private string btnStateMsg;
        public string BtnStateMsg
        {
            get { return btnStateMsg; }
            set { SetProperty(ref btnStateMsg, value); }
        }

        #region 视频相关联属性
        private BitmapSource currentFrame;
        public BitmapSource CurrentFrame
        {
            get { return currentFrame; }
            set { SetProperty(ref currentFrame, value); }
        }

        private void OnFrameReceived(BitmapSource source)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentFrame = source;
            });
        }
        #endregion

        #region 串口相关

        private readonly ISerialPortService serialPortService;
        private readonly ICameraService cameraService;
        private string selectedPort;
        public string SelectedPort
        {
            get { return selectedPort; }
            set { SetProperty(ref selectedPort, value); AddLog($"选择串口: {value}"); }
        }
        private int selectBandRate;
        public int SelectBandRate
        {
            get { return selectBandRate; }
            set { SetProperty(ref selectBandRate, value); AddLog($"选择波特率: {value}"); }
        }
        private string btnConnectStateColor = "Red";
        public string BtnConnectStateColor
        {
            get { return btnConnectStateColor; }
            set { SetProperty(ref btnConnectStateColor, value); }
        }
        private string btnConnectState = "连接";
        public string BtnConnectState
        {
            get { return btnConnectState; }
            set { SetProperty(ref btnConnectState, value); }
        }
        public ObservableCollection<string> AvailablePorts { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> BandRates { get; } = new ObservableCollection<int>()
        {
            115200,9600
        };
        public async void ControlMoveFunc(string data)
        {
            try
            {
                // 命令类型	数据长度	数据内容
                //命令类型 20前进 21 后退
                // 数据长度 帧数据内容的长度    01
                //数据内容 是哪个轴移动，00 x轴 01 y轴 02 z轴

                //string data = "200100";
                string crc = UsingModel.CalculateCrc(data);

                string cmd = "FFE0" + data + crc + "FFE1";


                await serialPortService.SendAsync(cmd);

                if (data.Substring(0, 2) == "24")
                {
                    cmd = "向前——" + cmd;
                }
                else if (data.Substring(0, 2) == "25")
                {
                    cmd = "向后——" + cmd;
                }
                else if (data.Substring(0, 2) == "22")
                {
                    cmd = "停止——" + cmd;
                }

                if (data.Substring(4, 2) == "00")
                {
                    cmd = "X轴" + cmd;
                }
                else if (data.Substring(4, 2) == "01")
                {
                    cmd = "Y轴" + cmd;
                }
                else if (data.Substring(4, 2) == "02")
                {
                    cmd = "Z轴" + cmd;
                }

                AddLog("发送：" + cmd);
            }
            catch (Exception ex)
            {
                LogHelper.WriteOrderLog(ex.ToString());
            }
        }
        private void CLoseRobotPort()
        {

            ControlMoveFunc("220100");
            ControlMoveFunc("220101");
            ControlMoveFunc("220102");
            serialPortService.Close();
            BtnConnectState = "连接";
            BtnConnectStateColor = "Red";

            AddLog("串口已断开");
        }
        private void Connect()
        {

            try
            {
                if (serialPortService.IsOpen)
                {
                    CLoseRobotPort();
                }
                else
                {
                    if(string.IsNullOrWhiteSpace(SelectedPort)|| SelectBandRate <1)
                    {
                        AddLog("SelectedPort和SelectBandRate不能为空");
                        return;
                    }
                    serialPortService.Open(SelectedPort, SelectBandRate);
                    BtnConnectState = "断开";
                    BtnConnectStateColor = "Green";
                    AddLog("串口来连接成功");
                }
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                LogHelper.WriteOrderLog(ex.ToString());
            }
        }
        private void LoadAvailablePorts()
        {
            AvailablePorts.Clear();
            var port = SerialPort.GetPortNames();
            foreach (var portName in port)
            {
                AvailablePorts.Add(portName);
            }
            if (AvailablePorts.Any())
            {
                selectedPort = AvailablePorts.First();
            }
        }

        #endregion



        public void AddLog(string str)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logs.Insert(0, $"[{DateTime.Now:HH:mm:ss}]{str}");  // 插入到最前面
                if (Logs.Count > 300)
                {
                    Logs.RemoveAt(300);  // 移除最后一条（最旧的）
                }
            });
        }
        public void Dispose()
        {
            cameraService.FrameReceived -= OnFrameReceived;
            cameraService.Dispose();
            currentFrame = null;

            //串口
            if (serialPortService.IsOpen)
            {
                CLoseRobotPort();
            }
        }
    }
}
