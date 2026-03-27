using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotHand_WPF_20290319.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {

        }
        #region 属性
        private bool isOpenMainLeftForm;
        public bool IsOpenMainLeftForm
        {
            get { return isOpenMainLeftForm; }
            set { SetProperty(ref isOpenMainLeftForm, value); }
        }
        #endregion
    }
}
