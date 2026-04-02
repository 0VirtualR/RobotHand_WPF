using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RobotHand_WPF_20290319.Extensions.Camera
{
    public interface ICameraService:IDisposable
    {
        bool IsInitialized { get; }
        event Action<BitmapSource> FrameReceived;
        bool Initialize();
        void ShowPropertyPage(IntPtr ownerHandle);
    }
}
