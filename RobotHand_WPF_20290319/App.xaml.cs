
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using RobotHand_WPF_20290319.Extensions;
using RobotHand_WPF_20290319.ViewModels;
using RobotHand_WPF_20290319.Views;
using RobotHand_WPF_20290319.Views.Dialogs;
using System.Windows;

namespace RobotHand_WPF_20290319
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager=Container.Resolve<IRegionManager>();
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
            var agg=Container.Resolve<IEventAggregator>();
            agg.SendMsg("打开成功机械臂注胶程序");
        }
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterForNavigation<IndexView,IndexViewModel>();
            containerRegistry.RegisterForNavigation<CameraView,CameraViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView,SettingsViewModel>();

            containerRegistry.RegisterDialog<MsgView, MsgViewModel>();

        }
    }

}
