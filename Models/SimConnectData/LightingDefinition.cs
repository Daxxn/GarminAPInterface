using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBMAutopilotDashboard.Models.SimConnectData
{
   public enum LightingDescriptor
   {
      LIGHTING_MAIN = 220
   }

   [StructLayout(LayoutKind.Sequential)]
   public struct LightingDefinition
   {
      public int PanelPot { get; set; } // Value is from 0 - 100.
      public int Panel { get; set; }
   }
}
