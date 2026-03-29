using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using RobotHand_WPF_20290319.Extensions;
using RobotHand_WPF_20290319.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotHand_WPF_20290319.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(IRegionManager regionManager,IEventAggregator aggregator)
        {
            this.regionManager = regionManager;
            this.aggregator = aggregator;
            MenuBars =new ObservableCollection<MenuBar>();
            CreateMenuBar();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
        }

        private void Navigate(MenuBar bar)
        {
            try
            {
                regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(bar.NameSpace.Trim());
                aggregator.SendMsg(bar.NameSpace.Trim() + "页面加载成功");

            }
          catch(Exception ex)
            {

            }
        }
        #region 字段
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator aggregator;
        #endregion

        #region 属性
        private bool isOpenMainDrawer;
        public bool IsOpenMainDrawer
        {
            get { return isOpenMainDrawer; }
            set { SetProperty(ref isOpenMainDrawer, value); }
        }
        private bool isOpenMainLeftForm;
        public bool IsOpenMainLeftForm
        {
            get { return isOpenMainLeftForm; }
            set { SetProperty(ref isOpenMainLeftForm, value); }
        }
        private ObservableCollection<MenuBar> menuBars;
        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { SetProperty(ref menuBars, value); }
        }
        #endregion
        #region 命令
        public DelegateCommand<MenuBar> NavigateCommand { get; set; }
        #endregion

        #region 函数
        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookOutline", Title = "初始化界面", NameSpace = "CameraView" });
       
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
        }
        #endregion
    }
}
