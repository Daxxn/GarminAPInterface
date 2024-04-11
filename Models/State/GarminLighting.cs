using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.AttributeModels;
using TBMAutopilotDashboard.Models.Enums;
using TBMAutopilotDashboard.Models.SimConnectData;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminLighting : Model
   {
      #region Local Props
      private SimConnect _simConnect;
      private ushort _backlight = 0;
      private double _backlightPerc = 0;
      private ushort _indicatorBright = 0;
      private ushort _maxBacklight = 0;
      private float _maxBacklightScale = 0.5f;
      private byte _maxBrightness = byte.MaxValue;
      private bool _backlightEn = false;

      private double _prevBacklightPerc = 0;
      private bool _prevBacklightEn = false;

      private uint _lightStates = 0;

      private LightingIndex _lightIndex = LightingIndex.PANEL;
      //private string _potSimVar = "LIGHT POTENTIOMETER:4";
      //private string _potSimVar = "LIGHTING_Panel_Light";
      private string _potSimVar = "LIGHT PANEL POWER SETTING"; // This is the ONLY SimVar that works... and aparently its the pot...
      private string _panelOnSimVar = "LIGHT PANEL ON";
      private string _potSimEvent = "LIGHT_POTENTIOMETER_SET";

      private bool _stateChanged = false;
      public bool StateChanged => _stateChanged;
      #endregion

      #region Constructors
      public GarminLighting() { }
      #endregion

      #region Methods
      public void ConnectModel(SimConnect simConnect)
      {
         _simConnect = simConnect;
      }

      public void RegisterSimData()
      {
         _simConnect.AddToDataDefinition(
            StructDefinition.LIGHTING_MESSAGE,
            _potSimVar,
            "percentage",
            SIMCONNECT_DATATYPE.INT32,
            0.0f,
            SimConnect.SIMCONNECT_UNUSED
         );
         _simConnect.AddToDataDefinition(
            StructDefinition.LIGHTING_MESSAGE,
            _panelOnSimVar,
            "BOOL",
            SIMCONNECT_DATATYPE.INT32,
            0.0f,
            SimConnect.SIMCONNECT_UNUSED
         );
         //_simConnect.AddToDataDefinition(
         //   StructDefinition.LIGHTING_MESSAGE,
         //   "LIGHT STATES",
         //   "MASK",
         //   SIMCONNECT_DATATYPE.INT32,
         //   0.0f,
         //   SimConnect.SIMCONNECT_UNUSED
         //);
         _simConnect.RegisterDataDefineStruct<LightingDefinition>(StructDefinition.LIGHTING_MESSAGE);

         _simConnect.MapClientEventToSimEvent(LightingEventID.LGHT_SET, _potSimEvent);
         _simConnect.AddClientEventToNotificationGroup(GroupEventID.LIGHTING, LightingEventID.LGHT_SET, false);
      }

      public void ReadSimData(LightingDefinition data)
      {
         BacklightPerc = data.PanelPot * 0.01;
         BacklightEnable = data.Panel != 0;
         //_lightStates = data.LightingStates;
         Backlight = (ushort)Math.Floor((BacklightPerc * _maxBacklightScale) * ushort.MaxValue);
         if (BacklightPerc != _prevBacklightPerc || BacklightEnable != _prevBacklightEn)
         {
            _stateChanged = true;
         }
      }

      //public void ReadSimData(double data)
      //{
      //   BacklightPerc = data * 0.01;
      //   //_lightStates = data.LightingStates;
      //   Backlight = (ushort)Math.Floor((BacklightPerc * _maxBacklightScale) * ushort.MaxValue);
      //}

      public void SendSimEvent()
      {
         _simConnect.TransmitClientEvent_EX1(
            (uint)SIMCONNECT_SIMOBJECT_TYPE.USER,
            LightingEventID.LGHT_SET,
            GroupEventID.LIGHTING,
            SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY,
            (uint)_lightIndex, /* The index of the pot */
            (uint)(BacklightPerc * uint.MaxValue),
            /* Unused parameters */
            0, 0, 0
         );
      }

      public void SendToDevice(byte[] buffer, int offset = 1)
      {
         if (!BacklightEnable)
         {
            buffer[offset] = 0;
            buffer[offset + 1] = 0;
         }
         else
         {
            var bl = ScaleBacklight();
            buffer[offset] = (byte)(bl & 0xFF);
            buffer[offset + 1] = (byte)(bl >> 8);
         }
         _prevBacklightPerc = BacklightPerc;
         _prevBacklightEn = BacklightEnable;
         _stateChanged = false;
      }

      private ushort ScaleBacklight()
      {
         return (ushort)((float)Backlight * _maxBacklightScale);
      }

      public void SendIndBrightness(byte[] buffer, int offset = 1)
      {
         buffer[offset] = (byte)(IndicatorBrightness & 0xFF);
         buffer[offset + 1] = (byte)(IndicatorBrightness >> 8);
      }

      public void SendMaxIndBrightness(byte[] buffer, int offset = 1)
      {
         buffer[offset] = MaxIndicatorBrightness;
      }
      #endregion

      #region Full Props
      public ushort Backlight // Later this should be replaced.
      {
         get => _backlight;
         set
         {
            _backlight = value;
            OnPropertyChanged();
         }
      }

      public double BacklightPerc
      {
         get => _backlightPerc;
         set
         {
            _backlightPerc = value;
            OnPropertyChanged();
         }
      }

      public ushort MaxBacklight
      {
         get => _maxBacklight;
         set
         {
            _maxBacklight = value;
            _maxBacklightScale = (float)value / (float)UInt16.MaxValue;
            OnPropertyChanged();
         }
      }

      public ushort IndicatorBrightness
      {
         get => _indicatorBright;
         set
         {
            _indicatorBright = value;
            OnPropertyChanged();
         }
      }

      public byte MaxIndicatorBrightness
      {
         get => _maxBrightness;
         set
         {
            _maxBrightness = value;
            OnPropertyChanged();
         }
      }

      public bool BacklightEnable
      {
         get => _backlightEn;
         set
         {
            _backlightEn = value;
            OnPropertyChanged();
         }
      }
      #endregion
   }
}
