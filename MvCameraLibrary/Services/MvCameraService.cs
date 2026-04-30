
using MvCameraLibrary.Interfaces;
using MvCameraLibrary.Models;
using MVSDK;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraHandle = System.Int32;
using MvApi = MVSDK.MvApi;

namespace MvCameraLibrary.Services
{
    internal class MvCameraService:IMvCameraService
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
            _frameCallback = CameraGrabberFrameCallback;
        }
        public bool Initialize(IntPtr ownerHandle)
        {
            if (IsOpened)
                return true;

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

            // 黑白相机：设置输出 MONO8
            tSdkCameraCapbility cap;
            MvApi.CameraGetCapability(_hCamera, out cap);

            if (cap.sIspCapacity.bMonoSensor != 0)
            {
                MvApi.CameraSetIspOutFormat(_hCamera, (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
                    1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                _grayPal = bmp.Palette;
                for (int i = 0; i < _grayPal.Entries.Length; i++)
                {
                    _grayPal.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
                }
            }

            MvApi.CameraCreateSettingPage(
         _hCamera,
         ownerHandle,
         _devInfo.acFriendlyName,
         null,
         IntPtr.Zero,
         0);

            return true;
        }
        public bool Initialize()
        {
            if (IsOpened)
                return true;

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

            // 黑白相机：设置输出 MONO8
            tSdkCameraCapbility cap;
            MvApi.CameraGetCapability(_hCamera, out cap);

            if (cap.sIspCapacity.bMonoSensor != 0)
            {
                MvApi.CameraSetIspOutFormat(_hCamera, (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
                    1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                _grayPal = bmp.Palette;
                for (int i = 0; i < _grayPal.Entries.Length; i++)
                {
                    _grayPal.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
                }
            }

            return true;
        }

        public void StartLive()
        {
            if (IsOpened)
                MvApi.CameraGrabber_StartLive(_grabber);
        }

        public void StopLive()
        {
            if (IsOpened)
                MvApi.CameraGrabber_StopLive(_grabber);
        }

        public void ShowSettingPage()
        {
            if (IsOpened)
                MvApi.CameraShowSettingPage(_hCamera, 1);
        }

        public bool Snap(string filePath)
        {
            if (!IsOpened)
                return false;

            IntPtr image;
            var result = MvApi.CameraGrabber_SaveImage(_grabber, out image, 2000);

            if (result == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
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

        public CameraStatistics GetStatistics()
        {
            if (!IsOpened)
                return new CameraStatistics();

            tSdkGrabberStat stat;
            MvApi.CameraGrabber_GetStat(_grabber, out stat);

            return new CameraStatistics
            {
                Width = stat.Width,
                Height = stat.Height,
                DispFps = stat.DispFps,
                CapFps = stat.CapFps
            };
        }

        private void CameraGrabberFrameCallback(
            IntPtr grabber,
            IntPtr pFrameBuffer,
            ref tSdkFrameHead pFrameHead,
            IntPtr context)
        {
            // SDK 默认是倒着的，先翻转
            MvApi.CameraFlipFrameBuffer(pFrameBuffer, ref pFrameHead, 1);

            int w = pFrameHead.iWidth;
            int h = pFrameHead.iHeight;
            bool gray = pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8;

            int stride = gray ? w : w * 3;
            int size = stride * h;

            byte[] buffer = new byte[size];
            Marshal.Copy(pFrameBuffer, buffer, 0, size);

            BitmapSource bitmapSource = BitmapSource.Create(
                w, h,
                96, 96,
                gray ? PixelFormats.Gray8 : PixelFormats.Bgr24,
                null,
                buffer,
                stride);

            bitmapSource.Freeze();

            FrameReceived?.Invoke(this, new CameraFrameEventArgs(bitmapSource, w, h, gray));
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
