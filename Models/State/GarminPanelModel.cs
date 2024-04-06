using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.Enums;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminPanelModel : Model
   {
      #region Local Props
      private readonly MessageController _messages;
      private GarminButtons _buttons;
      private GarminIndicators _indicators = new GarminIndicators();
      private GarminEncoders _encoders = new GarminEncoders();

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
      public void SendButtonStates(SimConnect simConnect)
      {
         if (Buttons.XFR)
         {
            Indicators.UpdateXFRIndicators();
            Buttons.XFR = false;
         }
         Buttons.SendInputsToSim(simConnect);
         Encoders.SendDataToSim(simConnect);
      }

      public void RegisterSimData(SimConnect simConnect)
      {
         if (simConnect is null) return;
         Buttons.InitSimInputs(simConnect);
         Indicators.RegisterSimData(simConnect);
         Encoders.RegisterSimData(simConnect);
      }

      public byte[] SendIndBrightness()
      {
         return new byte[]
         {
            (byte)(IndicatorBrightness & 0xFF),
            (byte)(IndicatorBrightness >> 8),
         };
      }

      public byte[] SendBacklight()
      {
         var bl = ScaleBacklight();
         return new byte[]
         {
            (byte)(bl & 0xFF),
            (byte)(bl >> 8),
         };
      }

      private ushort ScaleBacklight()
      {
         return (ushort)((float)Backlight * _maxBacklightScale);
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

      public ushort Backlight
      {
         get => _backlight;
         set
         {
            _backlight = value;
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
         get => _brightness;
         set
         {
            _brightness = value;
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
      #endregion
   }
}
