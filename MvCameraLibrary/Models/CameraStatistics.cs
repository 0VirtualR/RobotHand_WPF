using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvCameraLibrary.Models
{
    public sealed class CameraStatistics
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double DispFps { get; set; }
        public double CapFps { get; set; }

        public override string ToString()
        {
            return $"Resolution: {Width}*{Height} | DispFPS: {DispFps:0.0} | CapFPS: {CapFps:0.0}";
        }
    }
}
