
using MVSDK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CameraLibrary.Interface
{
    public class CameraFrameEventArgs : EventArgs
    {
        public BitmapSource Frame { get; }
        public int Width { get; }
        public int Height { get; }

        public CameraFrameEventArgs(BitmapSource frame, int width, int height)
        {
            Frame = frame;
            Width = width;
            Height = height;
        }
    }
    public interface ICameraService:IDisposable
    {
        bool IsOpened { get; }

        event EventHandler<CameraFrameEventArgs> FrameReceived;

        bool Initialize();
        void StartLive();
        void StopLive();
        void ShowSettingPage();
        bool Snap(string filePath);
    }
}