using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBMAutopilotDashboard.SimUtils
{
   public interface IBaseSimConnectWrapper
   {
      int GetUserSimConnectWinEvent();
      void ReceiveSimConnectMessage();
      void SetWindowHandle(IntPtr _hWnd);
      void Disconnect();
   }
}
