using MVSDK;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using CameraHandle = System.Int32;

namespace RobotHand_WPF_20290319.Extensions.Camera
{
    public class CameraService : ICameraService
    {
        private IntPtr m_Grabber = IntPtr.Zero;
        private CameraHandle m_hCamera = 0;
        private tSdkCameraDevInfo m_DevInfo;
        private pfnCameraGrabberFrameCallback m_FrameCallback;
        private ColorPalette m_GrayPal;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public event Action<BitmapSource> FrameReceived;

        public CameraService()
        {
            m_FrameCallback = new pfnCameraGrabberFrameCallback(CameraGrabberFrameCallback);
        }

        public bool Initialize()
        {
            if (_isInitialized) return true;

            CameraSdkStatus status = 0;
            tSdkCameraDevInfo[] devList;
            MvApi.CameraEnumerateDevice(out devList);
            int numDev = devList?.Length ?? 0;
            if (numDev < 1)
                return false;

            if (numDev == 1)
                status = MvApi.CameraGrabber_Create(out m_Grabber, ref devList[0]);
            else
                status = MvApi.CameraGrabber_CreateFromDevicePage(out m_Grabber);

            if (status != 0) return false;

            MvApi.CameraGrabber_GetCameraDevInfo(m_Grabber, out m_DevInfo);
            MvApi.CameraGrabber_GetCameraHandle(m_Grabber, out m_hCamera);

            MvApi.CameraGrabber_SetRGBCallback(m_Grabber, m_FrameCallback, IntPtr.Zero);

            tSdkCameraCapbility cap;
            MvApi.CameraGetCapability(m_hCamera, out cap);
            if (cap.sIspCapacity.bMonoSensor != 0)
            {
                MvApi.CameraSetIspOutFormat(m_hCamera, (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
                using (var tmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                {
                    m_GrayPal = tmp.Palette;
                    for (int i = 0; i < m_GrayPal.Entries.Length; i++)
                        m_GrayPal.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);
                }
            }

            MvApi.CameraGrabber_StartLive(m_Grabber);
            _isInitialized = true;
            return true;
        }

        public void ShowPropertyPage(IntPtr ownerHandle)
        {
            if (m_hCamera != 0)
                MvApi.CameraCreateSettingPage(m_hCamera, ownerHandle, m_DevInfo.acFriendlyName, null, IntPtr.Zero, 0);
        }

        private void CameraGrabberFrameCallback(
            IntPtr Grabber,
            IntPtr pFrameBuffer,
            ref tSdkFrameHead pFrameHead,
            IntPtr Context)
        {
            MvApi.CameraFlipFrameBuffer(pFrameBuffer, ref pFrameHead, 1);

            int w = pFrameHead.iWidth;
            int h = pFrameHead.iHeight;
            bool gray = (pFrameHead.uiMediaType == (uint)MVSDK.emImageFormat.CAMERA_MEDIA_TYPE_MONO8);
            int stride = gray ? w : w * 3;

            using (Bitmap image = new Bitmap(w, h, stride,
                gray ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb,
                pFrameBuffer))
            {
                if (gray) image.Palette = m_GrayPal;

                IntPtr hBitmap = image.GetHbitmap();
                try
                {
                    BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    bitmapSource.Freeze();

                    FrameReceived?.Invoke(bitmapSource);
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public void Dispose()
        {
            if (m_Grabber != IntPtr.Zero)
            {
                MvApi.CameraGrabber_StopLive(m_Grabber);
                MvApi.CameraGrabber_Destroy(m_Grabber);
                m_Grabber = IntPtr.Zero;
            }
            m_hCamera = 0;
            _isInitialized = false;
        }
    }
}
