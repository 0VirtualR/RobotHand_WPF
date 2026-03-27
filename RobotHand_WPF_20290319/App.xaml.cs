
using Prism.DryIoc;
using Prism.Ioc;
using RobotHand_WPF_20290319.ViewModels;
using RobotHand_WPF_20290319.Views;
using System.Windows;

namespace RobotHand_WPF_20290319
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

        }
    }

}
