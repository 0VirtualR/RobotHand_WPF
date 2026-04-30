using MvCameraLibrary.Interfaces;
using MvCameraLibrary.Services;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvCameraLibrary
{
    public class Mv2CameraProfile : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
           
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMvCameraService, MvCameraService>();
        }
    }
}
