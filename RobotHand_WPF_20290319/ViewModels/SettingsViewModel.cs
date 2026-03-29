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
    public class SettingsViewModel:BindableBase
    {
        private readonly IDialogService dialogService;
        public DelegateCommand OpenDialogCommand { get; set; }
        public SettingsViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            OpenDialogCommand = new DelegateCommand(OpenDialogFunc);
        }

        private async void OpenDialogFunc()
        {
            var param = new DialogParameters
            {
                {"testInputParam","这是测试主界面输入参数" }
            };
            dialogService.ShowDialog("MsgView", param, callback =>
            {
                if(callback.Result==ButtonResult.OK)
                {
                    string para = callback.Parameters.GetValue<string>("param1");
                }

                if(callback.Result==ButtonResult.Cancel)
                {
                    string para = callback.Parameters.GetValue<string>("param1");
                }
            });
        }

  
    }
}
