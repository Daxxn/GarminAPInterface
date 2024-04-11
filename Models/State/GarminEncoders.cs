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
      private SimConnect _simConnect;
      //public int this[PanelEncoder btn] { get => States[btn]; set => States[btn] = value; }
      //public int this[string btn] { get => States[PanelEncoderNames.ToEnum[btn]]; set => States[PanelEncoderNames.ToEnum[btn]] = value; }
      //private Dictionary<PanelEncoder, int> States { get; set; } = new Dictionary<PanelEncoder, int>();
      private GarminEncoder _hdg_Encoder;
      private GarminEncoder _alt_Encoder;
      private GarminEncoder _wheel_encoder;
      private GarminEncoder _crs1_encoder;
      private GarminEncoder _crs2_encoder;

      public GarminEncoder this[PanelEncoder enc]
      {
         get => _encoders[(int)enc];
         set
         {
            _encoders[(int)enc] = value;
            OnPropertyChanged(PanelEncoderNames.ToPropName[enc]);
         }
      }

      private AsyncObservableCollection<GarminEncoder> _encoders = new AsyncObservableCollection<GarminEncoder>();
      public AsyncObservableCollection<GarminEncoder> Encoders => _encoders;
      #endregion

      #region Constructors
      public GarminEncoders()
      {
         foreach (var _ in PanelEncoderNames.ToIndex)
         {
            _encoders.Add(new GarminEncoder());
         }

         //foreach (PanelEncoder pe in Enum.GetValues(typeof(PanelEncoder)))
         //{
         //   States.Add(pe, 0);
         //}

         // These are the SimEvent style.
         // Meaning it will need to be registered to a notification group and mapped to a client event ID.
         HDG_ENC = new GarminEncoder()
         {
            Name = PanelEncoder.HDG,
            IncrementInputName = "HEADING_BUG_INC",
            DecrementInputName = "HEADING_BUG_DEC",
            SetInputName = "HEADING_BUG_SET",
            IncrementEventID = EncoderEventID.HDG_INC,
            DecrementEventID = EncoderEventID.HDG_DEC,
            SetEventID = EncoderEventID.HDG_SET,
         };
         ALT_ENC = new GarminEncoder()
         {
            Name = PanelEncoder.ALT,
            IncrementInputName = "AP_ALT_VAR_INC",
            DecrementInputName = "AP_ALT_VAR_DEC",
            SetInputName = "AP_ALT_VAR_SET_ENGLISH",
            IncrementEventID = EncoderEventID.ALT_INC,
            DecrementEventID = EncoderEventID.ALT_DEC,
            SetEventID = EncoderEventID.ALT_SET,
         };
         WHEEL_ENC = new GarminEncoder()
         {
            Name = PanelEncoder.WHEEL,
            IncrementInputName = "AP_VS_VAR_INC",
            DecrementInputName = "AP_VS_VAR_DEC",
            SetInputName = "AP_VS_VAR_SET_ENGLISH",
            IncrementEventID = EncoderEventID.WHEEL_INC,
            DecrementEventID = EncoderEventID.WHEEL_DEC,
            SetEventID = EncoderEventID.WHEEL_SET,
            Enabled = false,
         };
         CRS1_ENC = new GarminEncoder()
         {
            Name = PanelEncoder.CRS1,
            IncrementInputName = "",
            DecrementInputName = "",
            IncrementEventID = EncoderEventID.CRS1_INC,
            DecrementEventID = EncoderEventID.CRS1_DEC,
            Enabled = false,
         };
         CRS2_ENC = new GarminEncoder()
         {
            Name = PanelEncoder.CRS2,
            IncrementInputName = "",
            DecrementInputName = "",
            IncrementEventID = EncoderEventID.CRS2_INC,
            DecrementEventID = EncoderEventID.CRS2_DEC,
            Enabled = false,
         };
      }
      #endregion

      #region Methods
      public void ConnectModel(SimConnect simConnect)
      {
         _simConnect = simConnect;
         foreach (var enc in _encoders)
         {
            enc.ConnectModel(simConnect);
         }
      }

      public void RegisterSimData()
      {
         foreach (var enc in _encoders)
         {
            if (enc.Enabled)
            {
               enc.RegisterSimData();
            }
         }
         _simConnect.SetNotificationGroupPriority(GroupEventID.ENCODERS, (uint)SIMCONNECT_GROUP_PRIORITY.DEFAULT);
      }

      public void SendDataToSim()
      {
         foreach (var enc in _encoders)
         {
            if (enc.Enabled)
            {
               enc.SendInputsToSim(_simConnect);
            }
         }
      }

      public void ReceiveData(byte[] buffer)
      {
         HDG_ENC.State   = (EncoderState)(buffer[0] & 0b00000011);
         ALT_ENC.State   = (EncoderState)((buffer[0] & 0b00001100) >> 2);
         WHEEL_ENC.State = (EncoderState)((buffer[0] & 0b00110000) >> 4);
         CRS1_ENC.State  = (EncoderState)((buffer[0] & 0b11000000) >> 6);
         CRS2_ENC.State  = (EncoderState)(buffer[1] & 0b11);
         SendDataToSim();

         //for (int i = 0; i < PanelEncoderNames.EncoderCount; i++)
         //{
         //   this[(PanelEncoder)i].PositionChange = (sbyte)buffer[i];
         //   //this[(PanelEncoder)i].SendPosition();
         //}
      }
      #endregion

      #region Full Props
      public GarminEncoder HDG_ENC
      {
         get => this[PanelEncoder.HDG];
         set => this[PanelEncoder.HDG] = value;
      }

      public GarminEncoder ALT_ENC
      {
         get => this[PanelEncoder.ALT];
         set => this[PanelEncoder.ALT] = value;
      }

      public GarminEncoder WHEEL_ENC
      {
         get => this[PanelEncoder.WHEEL];
         set => this[PanelEncoder.WHEEL] = value;
      }

      public GarminEncoder CRS1_ENC
      {
         get => this[PanelEncoder.CRS1];
         set => this[PanelEncoder.CRS1] = value;
      }

      public GarminEncoder CRS2_ENC
      {
         get => this[PanelEncoder.CRS2];
         set => this[PanelEncoder.CRS2] = value;
      }
      #endregion
   }
}
