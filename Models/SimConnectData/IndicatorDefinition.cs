using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBMAutopilotDashboard.Models.SimConnectData
{
   public enum IndicatorDescriptor
   {
      INDICATOR_MAIN = 110
   }

   [StructLayout(LayoutKind.Sequential)]
   public struct IndicatorDefinition
   {
      public int HDG  { get; set; }
      public int APR  { get; set; }
      public int BC   { get; set; }
      public int NAV  { get; set; }
      public int FD   { get; set; }
      public int BANK { get; set; }
      public int AP   { get; set; }
      public int YD   { get; set; }
      public int ALT  { get; set; }
      public int VS   { get; set; }
      public int FLC  { get; set; }
      public int SPD  { get; set; }
      //public double PanelPot { get; set; } // Value is from 0 - 100.
      //public int Panel { get; set; }
      //public int VNV { get; set; }
      //public int XFR_R { get; set; }
      //public int XFR_L { get; set; }
      //public int ERROR { get; set; }
   }
}
