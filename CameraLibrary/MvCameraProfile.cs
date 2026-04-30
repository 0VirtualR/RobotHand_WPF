using CameraLibrary.Interface;
using CameraLibrary.Service;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraLibrary
{
    public class MvCameraProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册相机服务为单例
            containerRegistry.RegisterSingleton<ICameraService, MvCameraService>();
        }
    }
}
