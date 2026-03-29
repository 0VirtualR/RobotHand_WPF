using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RobotHand_WPF_20290319.Extensions
{
    public class MsgModel
    {
        public string Filter {  get; set; }
        public string Msg {  get; set; }
    }
    public class MsgEvent : PubSubEvent<MsgModel>
    {

    }
  public static  class SnackbarExtension
    {
        public static void RegisterMsg(this IEventAggregator aggregator,Action<MsgModel> action,string filtrName = "Main")
        {
            aggregator.GetEvent<MsgEvent>().Subscribe(action, ThreadOption.PublisherThread, true, (m) =>
            {
                return m.Filter.Equals(filtrName);
            });
        }
        public static void SendMsg(this IEventAggregator aggregator,string Msg,string filterName = "Main")
        {
            aggregator.GetEvent<MsgEvent>().Publish(new MsgModel()
            {
                Msg = Msg,
                Filter = filterName
            });
        }
    }

}
