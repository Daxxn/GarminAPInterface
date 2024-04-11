using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.Enums;
using TBMAutopilotDashboard.Models.SimConnectData;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminPanelModel : Model
   {
      #region Local Props
      private SimConnect _simConnect;
      private readonly MessageController _messages;
      private GarminButtons _buttons;
      private GarminIndicators _indicators = new GarminIndicators();
      private GarminEncoders _encoders = new GarminEncoders();
      private GarminLighting _lighting = new GarminLighting();

      private ushort _backlight = 0;
      private float _maxBacklightScale = 0.5f;
      private ushort _maxBacklight = 0;

      private ushort _brightness = 0;
      private byte _maxBrightness = byte.MaxValue;
      #endregion

      #region Constructors
      public GarminPanelModel(MessageController messages)
      {
         _messages = messages;
         _buttons = new GarminButtons(messages);
      }
      #endregion

      #region Methods
      public void SendButtonStates()
      {
         if (Buttons.XFR)
         {
            Indicators.UpdateXFRIndicators();
            Buttons.XFR = false;
         }
         Buttons.SendInputsToSim();
         Encoders.SendDataToSim();
      }

      public void RegisterSimData()
      {
         if (_simConnect is null) return;
         Buttons.InitSimInputs();
         Encoders.RegisterSimData();
         Indicators.RegisterSimData();
         Lighting.RegisterSimData();
         //_simConnect.RegisterDataDefineStruct<SimVarDataDefinition>(StructDefinition.SIMVAR_DATA_MSG);
      }

      public void ReceiveSimData(SIMCONNECT_RECV_SIMOBJECT_DATA data)
      {
         // After testing, combine indicator and lighting into one request.
         if (data.dwDefineID == (uint)StructDefinition.INDICATOR_MESSAGE)
         {
            if (data.dwData[0] is IndicatorDefinition indDef)
            {
               Indicators.ReadSimData(indDef);
               return;
            }
         }
         if (data.dwDefineID == (uint)StructDefinition.LIGHTING_MESSAGE)
         {
            if (data.dwData[0] is LightingDefinition lghtDef)
            {
               Lighting.ReadSimData(lghtDef);
               return;
            }
         }
         _messages.Add(new Message("Failed to read data from sim. Unknown structure.", Messagetype.ERROR));
      }

      //public byte[] SendIndBrightness()
      //{
      //   return new byte[]
      //   {
      //      (byte)(IndicatorBrightness & 0xFF),
      //      (byte)(IndicatorBrightness >> 8),
      //   };
      //}

      //public void SendIndBrightness(byte[] buffer, int offset = 0)
      //{
      //   buffer[offset] = (byte)(IndicatorBrightness & 0xFF);
      //   buffer[offset + 1] = (byte)(IndicatorBrightness >> 8);
      //}

      //public byte[] SendBacklight()
      //{
      //   var bl = ScaleBacklight();
      //   return new byte[]
      //   {
      //      (byte)(bl & 0xFF),
      //      (byte)(bl >> 8),
      //   };
      //}

      //public void SendBacklight(byte[] buffer, int offset = 0)
      //{
      //   var bl = ScaleBacklight();
      //   buffer[offset] = (byte)(bl & 0xFF);
      //   buffer[offset + 1] = (byte)(bl >> 8);
      //}

      //private ushort ScaleBacklight()
      //{
      //   return (ushort)((float)Backlight * _maxBacklightScale);
      //}

      public void ConnectModels(SimConnect simConnect)
      {
         _simConnect = simConnect;
         Buttons.ConnectModel(simConnect);
         Encoders.ConnectModel(simConnect);
         Indicators.ConnectModel(simConnect);
         Lighting.ConnectModel(simConnect);
      }
      #endregion

      #region Full Props
      public GarminButtons Buttons
      {
         get => _buttons;
         set
         {
            _buttons = value;
            OnPropertyChanged();
         }
      }

      public GarminIndicators Indicators
      {
         get => _indicators;
         set
         {
            _indicators = value;
            OnPropertyChanged();
         }
      }

      public GarminEncoders Encoders
      {
         get => _encoders;
         set
         {
            _encoders = value;
            OnPropertyChanged();
         }
      }

      public GarminLighting Lighting
      {
         get => _lighting;
         set
         {
            _lighting = value;
            OnPropertyChanged();
         }
      }

      //public ushort Backlight
      //{
      //   get => _backlight;
      //   set
      //   {
      //      _backlight = value;
      //      OnPropertyChanged();
      //   }
      //}

      //public ushort MaxBacklight
      //{
      //   get => _maxBacklight;
      //   set
      //   {
      //      _maxBacklight = value;
      //      _maxBacklightScale = (float)value / (float)UInt16.MaxValue;
      //      OnPropertyChanged();
      //   }
      //}

      //public ushort IndicatorBrightness
      //{
      //   get => _brightness;
      //   set
      //   {
      //      _brightness = value;
      //      OnPropertyChanged();
      //   }
      //}

      //public byte MaxIndicatorBrightness
      //{
      //   get => _maxBrightness;
      //   set
      //   {
      //      _maxBrightness = value;
      //      OnPropertyChanged();
      //   }
      //}
      #endregion
   }
}
