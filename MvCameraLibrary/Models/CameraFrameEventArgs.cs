using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MvCameraLibrary.Models
{
    public sealed class CameraFrameEventArgs : EventArgs
    {
        public BitmapSource Frame { get; }
        public int Width { get; }
        public int Height { get; }
        public bool IsGray { get; }

        public CameraFrameEventArgs(BitmapSource frame, int width, int height, bool isGray)
        {
            Frame = frame;
            Width = width;
            Height = height;
            IsGray = isGray;
        }
    }
}
