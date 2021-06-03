using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBMAutopilotDashboard
{
   public class TBMData
   {
      #region - Fields & Properties
      public static readonly int DataSize = 10;
      public byte HDGLock { get; set; }
      public byte Approach { get; set; }
      public byte Backcourse { get; set; }
      public byte NAV1 { get; set; }
      public byte FlightDirector { get; set; }
      public double MaxBank { get; set; }
      public byte Master { get; set; }
      public byte YawDamper { get; set; }
      public byte ALTLock { get; set; }
      public byte VertHold { get; set; }
      #endregion

      #region - Constructors
      public TBMData() { }
      #endregion

      #region - Methods
      public static byte[] ConvertData(TBMData data)
      {
         return new byte[]
         {
            data.HDGLock,
            data.Approach,
            data.Backcourse,
            data.NAV1,
            data.FlightDirector,
            (byte)(data.MaxBank == 0 ? 0 : 1),
            data.Master,
            data.YawDamper,
            data.ALTLock,
            data.VertHold
         };
      }

      public static TBMData BuildData(IEnumerable<Variable> vars)
      {
         var data = new TBMData();
         foreach (var var in vars)
         {
            switch (var.Name)
            {
               case "AUTOPILOT HEADING LOCK":
                  data.HDGLock = (byte)var.Value;
                  break;
               case "AUTOPILOT APPROACH HOLD":
                  data.Approach = (byte)var.Value;
                  break;
               case "AUTOPILOT BACKCOURSE HOLD":
                  data.Backcourse = (byte)var.Value;
                  break;
               case "AUTOPILOT NAV1 LOCK":
                  data.NAV1 = (byte)var.Value;
                  break;
               case "AUTOPILOT FLIGHT DIRECTOR ACTIVE":
                  data.FlightDirector = (byte)var.Value;
                  break;
               case "AUTOPILOT MAX BANK":
                  data.MaxBank = (double)var.Value;
                  break;
               case "AUTOPILOT MASTER":
                  data.Master = (byte)var.Value;
                  break;
               case "AUTOPILOT YAW DAMPER":
                  data.YawDamper = (byte)var.Value;
                  break;
               case "AUTOPILOT ALTITUDE LOCK":
                  data.ALTLock = (byte)var.Value;
                  break;
               case "AUTOPILOT VERTICAL HOLD":
                  data.VertHold = (byte)var.Value;
                  break;
               default:
                  break;
            }
         }
         return data;
      }
      #endregion

      #region - Full Properties

      #endregion
   }
}
