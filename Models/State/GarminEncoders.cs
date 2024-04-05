using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.Enums.Constants;
using TBMAutopilotDashboard.Models.Enums;
using Microsoft.FlightSimulator.SimConnect;
using TBMAutopilotDashboard.Models.AttributeModels;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminEncoders : Model
   {
      #region Local Props
      //public int this[PanelEncoder btn] { get => States[btn]; set => States[btn] = value; }
      //public int this[string btn] { get => States[PanelEncodernames.ToEnum[btn]]; set => States[PanelEncodernames.ToEnum[btn]] = value; }
      //private Dictionary<PanelEncoder, int> States { get; set; } = new Dictionary<PanelEncoder, int>();
      private GarminEncoder _hdg_Encoder;
      private GarminEncoder _alt_Encoder;
      private GarminEncoder _wheel_encoder;
      private GarminEncoder _crs1_encoder;
      private GarminEncoder _crs2_encoder;

      private Dictionary<PanelEncoder, GarminEncoder> _encoders = new Dictionary<PanelEncoder, GarminEncoder>();
      #endregion

      #region Constructors
      public GarminEncoders()
      {
         //foreach (PanelEncoder pe in Enum.GetValues(typeof(PanelEncoder)))
         //{
         //   States.Add(pe, 0);
         //}

         // These are the SimEvent style.
         // Meaning it will need to be registered to a notification group and mapped to a client event ID.
         HDG_ENC = new GarminEncoder()
         {
            IncrementInputName = "HEADING_BUG_INC",
            DecrementInputName = "HEADING_BUG_DEC",
            IncrementEventID = EncoderEventID.HDG_INC,
            DecrementEventID = EncoderEventID.HDG_DEC,
         };
         ALT_ENC = new GarminEncoder()
         {
            IncrementInputName = "AP_ALT_VAR_INC",
            DecrementInputName = "AP_ALT_VAR_DEC",
            IncrementEventID = EncoderEventID.ALT_INC,
            DecrementEventID = EncoderEventID.ALT_DEC,
         };
         WHEEL_ENC = new GarminEncoder()
         {
            IncrementInputName = "AP_VS_VAR_INC",
            DecrementInputName = "AP_VS_VAR_DEC",
            IncrementEventID = EncoderEventID.WHEEL_INC,
            DecrementEventID = EncoderEventID.WHEEL_DEC,
         };
         CRS1_ENC = new GarminEncoder()
         {
            IncrementInputName = "",
            DecrementInputName = "",
            IncrementEventID = EncoderEventID.CRS1_INC,
            DecrementEventID = EncoderEventID.CRS1_DEC,
         };
         CRS2_ENC = new GarminEncoder()
         {
            IncrementInputName = "",
            DecrementInputName = "",
            IncrementEventID = EncoderEventID.CRS2_INC,
            DecrementEventID = EncoderEventID.CRS2_DEC,
         };

         _encoders[PanelEncoder.HDG] = new GarminEncoder()
         {
            IncrementInputName = "HEADING_BUG_INC",
            DecrementInputName = "HEADING_BUG_DEC",
            IncrementEventID = EncoderEventID.HDG_INC,
            DecrementEventID = EncoderEventID.HDG_DEC,
         };
         _encoders[PanelEncoder.ALT] = new GarminEncoder()
         {
            IncrementInputName = "AP_ALT_VAR_INC",
            DecrementInputName = "AP_ALT_VAR_DEC",
            IncrementEventID = EncoderEventID.ALT_INC,
            DecrementEventID = EncoderEventID.ALT_DEC,
         };
         _encoders[PanelEncoder.WHEEL] = new GarminEncoder()
         {
            IncrementInputName = "AP_VS_VAR_INC",
            DecrementInputName = "AP_VS_VAR_DEC",
            IncrementEventID = EncoderEventID.WHEEL_INC,
            DecrementEventID = EncoderEventID.WHEEL_DEC,
         };
         // These arent able to be mapped... might as well find something else to map them to.
         _encoders[PanelEncoder.CRS1] = new GarminEncoder()
         {
            IncrementInputName = "",
            DecrementInputName = "",
            IncrementEventID = EncoderEventID.CRS1_INC,
            DecrementEventID = EncoderEventID.CRS1_DEC,
            Enabled = false
         };
         _encoders[PanelEncoder.CRS2] = new GarminEncoder()
         {
            IncrementInputName = "",
            DecrementInputName = "",
            IncrementEventID = EncoderEventID.CRS2_INC,
            DecrementEventID = EncoderEventID.CRS2_DEC,
            Enabled = false
         };
      }
      #endregion

      #region Methods
      public void RegisterSimData(SimConnect simConnect)
      {
         foreach (var enc in _encoders.Values)
         {
            if (enc.Enabled)
            {
               enc.RegisterSimData(simConnect);
            }
         }
         simConnect.SetNotificationGroupPriority(EncoderGroupEventID.DEFAULT, (uint)SIMCONNECT_GROUP_PRIORITY.DEFAULT);
      }

      public void SendDataToSim(SimConnect simConnect)
      {
         foreach (var enc in _encoders.Values)
         {
            if (enc.Enabled)
            {
               enc.SendInputsToSim(simConnect);
            }
         }
      }
      #endregion

      #region Full Props
      public GarminEncoder HDG_ENC
      {
         get => _hdg_Encoder;
         set
         {
            _hdg_Encoder = value;
            OnPropertyChanged();
         }
      }

      public GarminEncoder ALT_ENC
      {
         get => _alt_Encoder;
         set
         {
            _alt_Encoder = value;
            OnPropertyChanged();
         }
      }

      public GarminEncoder WHEEL_ENC
      {
         get => _wheel_encoder;
         set
         {
            _wheel_encoder = value;
            OnPropertyChanged();
         }
      }

      public GarminEncoder CRS1_ENC
      {
         get => _crs1_encoder;
         set
         {
            _crs1_encoder = value;
            OnPropertyChanged();
         }
      }

      public GarminEncoder CRS2_ENC
      {
         get => _crs2_encoder;
         set
         {
            _crs2_encoder = value;
            OnPropertyChanged();
         }
      }

      //[SimEvent("AP_HEADING_BUG_SET")]
      //public int HDG
      //{
      //   get => States[PanelEncoder.HDG];
      //   set
      //   {
      //      States[PanelEncoder.HDG] = value;
      //      OnPropertyChanged();
      //   }
      //}

      //[SimEvent("AP_ALT_VAR_SET_ENGLISH")]
      //public int ALT
      //{
      //   get => States[PanelEncoder.ALT];
      //   set
      //   {
      //      States[PanelEncoder.ALT] = value;
      //      OnPropertyChanged();
      //   }
      //}

      //[SimEvent("AP_N1_REF_SET")]
      //public int CRS1
      //{
      //   get => States[PanelEncoder.CRS1];
      //   set
      //   {
      //      States[PanelEncoder.CRS1] = value;
      //      OnPropertyChanged();
      //   }
      //}

      //public int CRS2
      //{
      //   get => States[PanelEncoder.CRS2];
      //   set
      //   {
      //      States[PanelEncoder.CRS2] = value;
      //      OnPropertyChanged();
      //   }
      //}

      //[SimEvent("AP_VS_VAR_SET_ENGLISH")]
      //public int WHEEL
      //{
      //   get => States[PanelEncoder.WHEEL];
      //   set
      //   {
      //      States[PanelEncoder.WHEEL] = value;
      //      OnPropertyChanged();
      //   }
      //}
      #endregion
   }
}
