using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotHand_WPF_20290319.ViewModels
{
    public class MsgViewModel :BindableBase, IDialogAware
    {
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }    

        public MsgViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
            SaveCommand = new DelegateCommand(Save);
        }

        private void Save()
        {
            var returnParam = new DialogParameters { { "param1", "确认保存返回参数" }, { "param2", 2 } };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK,returnParam));
        }

        private void Cancel()
        {
            var returnParam = new DialogParameters { { "param1", "确认取消按钮的返回参数" } };
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel,returnParam));    
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }


        //是关闭这个弹窗的句柄，要把弹窗的结果、数据传回打开这个弹窗的界面，执行实际的关闭dialog操作
        public event Action<IDialogResult> RequestClose;


        public bool CanCloseDialog()
        {
            // "当×掉这个弹窗的时候，看是否有没有数据没有保存，看是否不满足关闭这个弹窗dialog的条件" 它是一个验证关卡，在关闭前被框架自动调用，用来判断是否允许关闭。
            return true;
        }

        public void OnDialogClosed()
        {
            //对话框关闭后调用，用于清理资源
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //对话框打开时调用，接收主界面传递的参数
            if (parameters.ContainsKey("testInputParam"))
            {
                Title = parameters.GetValue<string>("testInputParam");
            }
        }
    }
}
