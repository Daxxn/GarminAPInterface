using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.AttributeModels;
using TBMAutopilotDashboard.Models.Enums;
using TBMAutopilotDashboard.Models.Enums.Constants;
using TBMAutopilotDashboard.Models.SimConnectData;

using static TBMAutopilotDashboard.Models.GarminSerialController;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminIndicators : Model
   {
      #region Local Props
      private bool xfrState = false;
      private bool _stateChanged = false;
      public bool StateChanged => _stateChanged;
      public bool this[PanelIndicator btn]
      {
         get => States[btn];
         set
         {
            if (States[btn] != value)
            {
               _stateChanged = true;
            }
            States[btn] = value;
            OnPropertyChanged(PanelIndicatorNames.FromEnum[btn]);
         }
      }
      public bool this[string btn]
      {
         get => States[PanelIndicatorNames.ToEnum[btn]];
         set
         {
            if (States[PanelIndicatorNames.ToEnum[btn]] != value)
            {
               _stateChanged = true;
            }
            States[PanelIndicatorNames.ToEnum[btn]] = value;
            OnPropertyChanged(btn);
         }
      }
      private Dictionary<PanelIndicator, bool> States { get; set; } = new Dictionary<PanelIndicator, bool>();
      private PropertyInfo[] _indicatorProps;
      #endregion

      #region Constructors
      public GarminIndicators()
      {
         foreach (PanelIndicator pi in Enum.GetValues(typeof(PanelIndicator)))
         {
            States.Add(pi, false);
         }

         _indicatorProps = typeof(IndicatorDefinition).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      }
      #endregion

      #region Methods
      public void RegisterSimData(SimConnect simConnect)
      {
         var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
         foreach (var prop in props)
         {
            var dataAttr = prop.GetCustomAttribute<SimDataAttribute>();
            if (dataAttr != null)
            {
               simConnect.AddToDataDefinition(
                  StructDefinition.INDICATOR_MESSAGE,
                  dataAttr.SimDataName,
                  dataAttr.SimUnits,
                  dataAttr.Type,
                  0.0f,
                  SimConnect.SIMCONNECT_UNUSED
               );
            }
         }
         simConnect.RegisterDataDefineStruct<IndicatorDefinition>(StructDefinition.INDICATOR_MESSAGE);
      }

      public void ReadSimData(SIMCONNECT_RECV_SIMOBJECT_DATA data)
      {
         // Cast the data to IndicatorDefinition and parse the values...
         if (data.dwData[0] is IndicatorDefinition indDef)
         {
            foreach (var prop in _indicatorProps)
            {
               this[prop.Name] = Convert.ToBoolean(prop.GetValue(indDef));
            }
         }
      }

      public void UpdateXFRIndicators()
      {
         xfrState = !xfrState;
         if (xfrState)
         {
            XFR_L = true;
            XFR_R = false;
         }
         else
         {
            XFR_L = false;
            XFR_R = true;
         }
      }

      public byte[] SendIndicators()
      {
         ushort temp = 0;
         for (int i = 0; i < PanelIndicatorNames.IndicatorCount; i++)
         {
            var t = (Convert.ToUInt16(States[(PanelIndicator)i])) << i;
            temp |= (ushort)t;
         }
         byte[] buffer = new byte[] { (byte)(temp & 0xFF), (byte)(temp >> 8) };
         _stateChanged = false;
         return buffer;
      }

      public void ClearIndicators()
      {
         foreach (PanelIndicator ind in Enum.GetValues(typeof(PanelIndicator)))
         {
            States[ind] = false;
         }
      }
      #endregion

      #region Full Props

      [SimData("AUTOPILOT HEADING LOCK")]
      public bool HDG
      {
         get => States[PanelIndicator.HDG];
         set
         {
            States[PanelIndicator.HDG] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT APPROACH HOLD")]
      public bool APR
      {
         get => States[PanelIndicator.APR];
         set
         {
            States[PanelIndicator.APR] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT BANK HOLD")]
      public bool BC
      {
         get => States[PanelIndicator.BC];
         set
         {
            States[PanelIndicator.BC] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT NAV1 LOCK")]
      public bool NAV
      {
         get => States[PanelIndicator.NAV];
         set
         {
            States[PanelIndicator.NAV] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT FLIGHT DIRECTOR ACTIVE")]
      public bool FD
      {
         get => States[PanelIndicator.FD];
         set
         {
            States[PanelIndicator.FD] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT MAX BANK")]
      public bool BANK
      {
         get => States[PanelIndicator.BANK];
         set
         {
            States[PanelIndicator.BANK] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT MASTER")]
      public bool AP
      {
         get => States[PanelIndicator.AP];
         set
         {
            States[PanelIndicator.AP] = value;
            OnPropertyChanged();
         }
      }

      public bool XFR_R
      {
         get => States[PanelIndicator.XFR_R];
         set
         {
            States[PanelIndicator.XFR_R] = value;
            OnPropertyChanged();
         }
      }

      public bool XFR_L
      {
         get => States[PanelIndicator.XFR_L];
         set
         {
            States[PanelIndicator.XFR_L] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT YAW DAMPER")]
      public bool YD
      {
         get => States[PanelIndicator.YD];
         set
         {
            States[PanelIndicator.YD] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT ALTITUDE LOCK")]
      public bool ALT
      {
         get => States[PanelIndicator.ALT];
         set
         {
            States[PanelIndicator.ALT] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT VERTICAL HOLD")]
      public bool VS
      {
         get => States[PanelIndicator.VS];
         set
         {
            States[PanelIndicator.VS] = value;
            OnPropertyChanged();
         }
      }

      public bool VNV
      {
         get => States[PanelIndicator.VNV];
         set
         {
            States[PanelIndicator.VNV] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT FLIGHT LEVEL CHANGE")]
      public bool FLC
      {
         get => States[PanelIndicator.FLC];
         set
         {
            States[PanelIndicator.FLC] = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT MANAGED SPEED IN MACH")]
      public bool SPD
      {
         get => States[PanelIndicator.SPD];
         set
         {
            States[PanelIndicator.SPD] = value;
            OnPropertyChanged();
         }
      }

      public bool ERROR
      {
         get => States[PanelIndicator.ERROR];
         set
         {
            States[PanelIndicator.ERROR] = value;
            OnPropertyChanged();
         }
      }
      #endregion
   }
}
