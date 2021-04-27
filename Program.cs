using System;
using System.Threading.Tasks;

namespace SendPushTest
{
    class MainClass
    {


        public static void Main(string[] args)
        {
            var wf = workflow();
            wf.GetAwaiter().GetResult();
        }

        public static async Task workflow()
        {
            var rpn = new PushNotificator();
            //rpn.checkedUnavailableDevices();

            rpn.sendTestNotification();
            return;

        }
    }
}
