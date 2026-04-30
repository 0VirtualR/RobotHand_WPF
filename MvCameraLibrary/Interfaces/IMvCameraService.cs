using MvCameraLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvCameraLibrary.Interfaces
{
    public interface IMvCameraService:IDisposable
    {
        bool IsOpened { get; }

        event EventHandler<CameraFrameEventArgs> FrameReceived;

        bool Initialize();
        void StartLive();
        void StopLive();
        void ShowSettingPage();
        bool Snap(string filePath);
        CameraStatistics GetStatistics();
    }
}
