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
using System.Windows.Threading;

namespace RobotHand_WPF_20290319.ViewModels
{
    public class CameraViewModel : BindableBase,IDisposable
    {
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();
     

        #region 串口相关属性

        private readonly ISerialPortService serialPort;

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
        private string btnConnectStateColor= "Red";
        public string BtnConnectStateColor
        {
            get { return btnConnectStateColor; }
            set { SetProperty(ref btnConnectStateColor, value); }
        }
        private string btnConnectState= "连接";
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
        #endregion
        public CameraViewModel(ISerialPortService serialPort)
        {
            this.serialPort = serialPort;

            StartCommand = new DelegateCommand(Start);
            InitCommand = new DelegateCommand(Init);
            ConnectCommmand = new DelegateCommand(Connect);

            LoadAvailablePorts();
        }

        private void LoadAvailablePorts()
        {
            AvailablePorts.Clear();
            var port = SerialPort.GetPortNames();
            foreach ( var portName in port)
            {
                AvailablePorts.Add(portName);
            }
            if (AvailablePorts.Any())
            {
                selectedPort=AvailablePorts.First();
            }
        }

        private void Connect()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    BtnConnectState = "连接";
                    BtnConnectStateColor = "Red";
                    AddLog("串口已断开");
                }
                else
                {
                    serialPort.Open(SelectedPort, SelectBandRate);
                    BtnConnectState = "断开";
                    BtnConnectStateColor = "Green";
                    AddLog("串口来连接成功");
                }
            }catch(Exception ex)
            {
                AddLog(ex.Message);
                LogHelper.WriteOrderLog(ex.ToString());
            }
        }
        private void Init()
        {
            throw new NotImplementedException();
        }
        private void Start()
        {
            throw new NotImplementedException();
        }

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand InitCommand { get; set; }
        public DelegateCommand ConnectCommmand { get; set; }

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
            throw new NotImplementedException();
        }
    }
}
