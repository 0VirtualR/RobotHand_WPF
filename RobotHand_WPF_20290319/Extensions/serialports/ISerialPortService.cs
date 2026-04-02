using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RobotHand_WPF_20290319.Extensions.SerialPorts
{
    public interface ISerialPortService
    {
        event Action<string> DataReceived;
        bool IsOpen { get; }
        bool Open(string portName="",int baudRate=115200);
        void Close();
        Task SendAsync(string data);
    }
}
