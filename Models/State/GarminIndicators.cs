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
      private SimConnect _simConnect;
      private bool xfrState = false;
      private bool _stateChanged = false;
      public bool StateChanged => _stateChanged;
      public bool this[PanelIndicator btn]
      {
         get => Convert.ToBoolean((_stateData >> (int)btn) & 1);
         set
         {
            if (value)
            {
               _stateData |= (ushort)(1 << (int)btn);
            }
            else
            {
               _stateData &= (ushort)(~(1 << (int)btn));
            }
            _stateChanged = true;
            OnPropertyChanged(PanelIndicatorNames.FromEnum[btn]);
         }
      }

      public bool this[string btn]
      {
         get => this[PanelIndicatorNames.ToEnum[btn]];
         set
         {
            //if (this[PanelIndicatorNames.ToEnum[btn]] != value)
            //{
            //   _stateChanged = true;
            //}
            this[PanelIndicatorNames.ToEnum[btn]] = value;
            //_stateChanged = true;
            //OnPropertyChanged(btn);
         }
      }

      public bool this[int btn]
      {
         get => Convert.ToBoolean(_stateData & (ushort)(1 << btn));
         set
         {
            if (value)
            {
               _stateData |= (ushort)(1 << (int)btn);
            }
            else
            {
               _stateData &= (ushort)(~(1 << (int)btn));
            }
            _stateChanged = true;
            OnPropertyChanged(PanelIndicatorNames.ToName[btn]);
         }
      }

      //private Dictionary<bool> States { get; set; } = new Dictionary<PanelIndicator, bool>();
      private PropertyInfo[] _indicatorProps;
      private ushort _stateData = 0;
      public ushort States
      {
         get => _stateData;
         set
         {
            _stateData = value;
            _stateChanged = true;
            for (int i = 0; i < 16; i++)
            {
               OnPropertyChanged(PanelIndicatorNames.ToName[i]);
            }
         }
      }
      #endregion

      #region Constructors
      public GarminIndicators()
      {
         //foreach (PanelIndicator pi in Enum.GetValues(typeof(PanelIndicator)))
         //{
         //   States.Add(pi, false);
         //}
         _indicatorProps = typeof(IndicatorDefinition).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      }
      #endregion

      #region Methods
      public void ConnectModel(SimConnect simConnect)
      {
         _simConnect = simConnect;
      }

      public void RegisterSimData()
      {
         var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
         foreach (var prop in props)
         {
            var dataAttr = prop.GetCustomAttribute<SimDataAttribute>();
            if (dataAttr != null)
            {
               _simConnect.AddToDataDefinition(
                  StructDefinition.INDICATOR_MESSAGE,
                  dataAttr.SimDataName,
                  dataAttr.SimUnits,
                  dataAttr.Type,
                  0.0f,
                  SimConnect.SIMCONNECT_UNUSED
               );
            }
         }
         _simConnect.RegisterDataDefineStruct<IndicatorDefinition>(StructDefinition.INDICATOR_MESSAGE);
      }

      public void ReadSimData(IndicatorDefinition indData)
      {
         foreach (var prop in _indicatorProps)
         {
            this[prop.Name] = Convert.ToBoolean(prop.GetValue(indData));
            //if (prop.Name != "LightingPot")
            //{
            //   this[prop.Name] = Convert.ToBoolean(prop.GetValue(indData));
            //}
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
            var t = (Convert.ToUInt16(this[i])) << i;
            temp |= (ushort)t;
         }
         byte[] buffer = new byte[] { (byte)(temp >> 8), (byte)(temp & 0xFF) };
         _stateChanged = false;
         return buffer;
      }

      public void SendIndicators(byte[] buffer, int offset = 1)
      {
         ushort temp = 0;
         for (int i = 0; i < PanelIndicatorNames.IndicatorCount; i++)
         {
            var t = (Convert.ToUInt16(this[i])) << i;
            temp |= (ushort)t;
         }
         //byte[] buffer = new byte[] { (byte)(temp >> 8), (byte)(temp & 0xFF) };
         buffer[offset] = (byte)(temp >> 8);
         buffer[offset + 1] = (byte)(temp & 0xFF);
         _stateChanged = false;
      }

      public void ClearIndicators()
      {
         States = 0;
         //foreach (PanelIndicator ind in Enum.GetValues(typeof(PanelIndicator)))
         //{
         //   States[ind] = false;
         //}
      }

      public void SetIndicators()
      {
         States = 0xFFFF;
      }
      #endregion

      #region Full Props

      [SimData("AUTOPILOT HEADING LOCK")]
      public bool HDG
      {
         get => this[PanelIndicator.HDG];
         set
         {
            this[PanelIndicator.HDG] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT APPROACH HOLD")]
      public bool APR
      {
         get => this[PanelIndicator.APR];
         set
         {
            this[PanelIndicator.APR] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT BACKCOURSE HOLD")]
      public bool BC
      {
         get => this[PanelIndicator.BC];
         set
         {
            this[PanelIndicator.BC] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT NAV1 LOCK")]
      public bool NAV
      {
         get => this[PanelIndicator.NAV];
         set
         {
            this[PanelIndicator.NAV] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT FLIGHT DIRECTOR ACTIVE")]
      public bool FD
      {
         get => this[PanelIndicator.FD];
         set
         {
            this[PanelIndicator.FD] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT BANK HOLD")]
      public bool BANK
      {
         get => this[PanelIndicator.BANK];
         set
         {
            this[PanelIndicator.BANK] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT MASTER")]
      public bool AP
      {
         get => this[PanelIndicator.AP];
         set
         {
            this[PanelIndicator.AP] = value;
            //OnPropertyChanged();
         }
      }

      public bool XFR_R
      {
         get => this[PanelIndicator.XFR_R];
         set
         {
            this[PanelIndicator.XFR_R] = value;
            //OnPropertyChanged();
         }
      }

      public bool XFR_L
      {
         get => this[PanelIndicator.XFR_L];
         set
         {
            this[PanelIndicator.XFR_L] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT YAW DAMPER")]
      public bool YD
      {
         get => this[PanelIndicator.YD];
         set
         {
            this[PanelIndicator.YD] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT ALTITUDE LOCK")]
      public bool ALT
      {
         get => this[PanelIndicator.ALT];
         set
         {
            this[PanelIndicator.ALT] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT VERTICAL HOLD")]
      public bool VS
      {
         get => this[PanelIndicator.VS];
         set
         {
            this[PanelIndicator.VS] = value;
            //OnPropertyChanged();
         }
      }

      public bool VNV
      {
         get => this[PanelIndicator.VNV];
         set
         {
            this[PanelIndicator.VNV] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT FLIGHT LEVEL CHANGE")]
      public bool FLC
      {
         get => this[PanelIndicator.FLC];
         set
         {
            this[PanelIndicator.FLC] = value;
            //OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT MANAGED SPEED IN MACH")]
      public bool SPD
      {
         get => this[PanelIndicator.SPD];
         set
         {
            this[PanelIndicator.SPD] = value;
            //OnPropertyChanged();
         }
      }

      public bool ERROR
      {
         get => this[PanelIndicator.ERROR];
         set
         {
            this[PanelIndicator.ERROR] = value;
            //OnPropertyChanged();
         }
      }
      #endregion
   }
}
