using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CameraLibrary.Interface;
using MVSDK;
using CameraHandle = System.Int32;

namespace CameraLibrary.Service
{
    public class MvCameraService : ICameraService, IDisposable
    {
        private IntPtr _grabber = IntPtr.Zero;
        private CameraHandle _hCamera = 0;
        private tSdkCameraDevInfo _devInfo;
        private ColorPalette _grayPal;
        private pfnCameraGrabberFrameCallback _frameCallback;

        public bool IsOpened => _grabber != IntPtr.Zero;

        public event EventHandler<CameraFrameEventArgs> FrameReceived;

        public MvCameraService()
        {
            _frameCallback = new pfnCameraGrabberFrameCallback(CameraGrabberFrameCallback);
        }
      
        public bool Initialize()
        {
            CameraSdkStatus status = 0;

            tSdkCameraDevInfo[] devList;
            MvApi.CameraEnumerateDevice(out devList);

            int numDev = devList != null ? devList.Length : 0;
            if (numDev < 1)
                return false;

            if (numDev == 1)
            {
                status = MvApi.CameraGrabber_Create(out _grabber, ref devList[0]);
            }
            else
            {
                status = MvApi.CameraGrabber_CreateFromDevicePage(out _grabber);
            }

            if (status != 0)
                return false;

            MvApi.CameraGrabber_GetCameraDevInfo(_grabber, out _devInfo);
            MvApi.CameraGrabber_GetCameraHandle(_grabber, out _hCamera);

            MvApi.CameraGrabber_SetRGBCallback(_grabber, _frameCallback, IntPtr.Zero);

            // 黑白相机转灰度
            tSdkCameraCapbility cap;
            MvApi.CameraGetCapability(_hCamera, out cap);
            if (cap.sIspCapacity.bMonoSensor != 0)
            {
                MvApi.CameraSetIspOutFormat(_hCamera, (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);

                Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
                _grayPal = bmp.Palette;
                for (int i = 0; i < _grayPal.Entries.Length; i++)
                    _grayPal.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
            }

            return true;
        }

        public void StartLive()
        {
            if (_grabber != IntPtr.Zero)
                MvApi.CameraGrabber_StartLive(_grabber);
        }

        public void StopLive()
        {
            if (_grabber != IntPtr.Zero)
                MvApi.CameraGrabber_StopLive(_grabber);
        }
        public void ShowSettingPage()
        {
            if (_grabber != IntPtr.Zero)
                MvApi.CameraShowSettingPage(_hCamera, 1);
        }
      

        public bool Snap(string filePath)
        {
            if (_grabber == IntPtr.Zero)
                return false;

            IntPtr image;
            if (MvApi.CameraGrabber_SaveImage(_grabber, out image, 2000) == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
            {
                try
                {
                    MvApi.CameraImage_SaveAsBmp(image, filePath);
                    return true;
                }
                finally
                {
                    MvApi.CameraImage_Destroy(image);
                }
            }

            return false;
        }

        private void CameraGrabberFrameCallback(
            IntPtr grabber,
            IntPtr pFrameBuffer,
            ref tSdkFrameHead pFrameHead,
            IntPtr context)
        {
            // SDK 数据是从下往上，需要翻转
            MvApi.CameraFlipFrameBuffer(pFrameBuffer, ref pFrameHead, 1);

            int w = pFrameHead.iWidth;
            int h = pFrameHead.iHeight;
            bool gray = (pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
            int stride = gray ? w : w * 3;
            int size = stride * h;

            byte[] buffer = new byte[size];
            Marshal.Copy(pFrameBuffer, buffer, 0, size);

            BitmapSource bitmapSource = BitmapSource.Create(
                w, h,
                96, 96,
                gray ? System.Windows.Media.PixelFormats.Gray8 : System.Windows.Media.PixelFormats.Bgr24,
                null,
                buffer,
                stride);

            bitmapSource.Freeze();

            if (gray && _grayPal != null)
            {
                // BitmapSource 不需要手动设置调色板
                // 这里保留逻辑说明：如果你用 GDI Bitmap，就需要 palette
            }

            FrameReceived?.Invoke(this, new CameraFrameEventArgs(bitmapSource, w, h));
        }

        public void Dispose()
        {
            if (_grabber != IntPtr.Zero)
            {
                MvApi.CameraGrabber_Destroy(_grabber);
                _grabber = IntPtr.Zero;
            }
        }
    
}
}