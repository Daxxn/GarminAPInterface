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
   public class GarminEncoder : Model
   {
      #region Local Props
      private SimConnect _simConnect;
      //private static readonly InputDataMap _inputDataMap = InputDataMap.Instance;
      private static GroupEventID GroupEventID = GroupEventID.ENCODERS;

      private EncoderState _state = EncoderState.STILL;
      private uint _currPos = 0;
      private int _posChange = 0;

      private string _incInputName = null;
      private string _decInputName = null;
      private string _setInputName = null;
      private EncoderEventID _incEventID;
      private EncoderEventID _decEventID;
      private EncoderEventID _setEventID;

      private bool _enabled = true;

      private PanelEncoder? _name = null;

      //private ulong _incHash;
      //private ulong _decHash;
      #endregion

      #region Constructors
      public GarminEncoder() { }
      #endregion

      #region Methods
      public void ConnectModel(SimConnect simConnect)
      {
         _simConnect = simConnect;
      }

      public void RegisterSimData()
      {
         _simConnect.MapClientEventToSimEvent(IncrementEventID, IncrementInputName);
         _simConnect.MapClientEventToSimEvent(DecrementEventID, DecrementInputName);
         _simConnect.AddClientEventToNotificationGroup(GroupEventID, IncrementEventID, false);
         _simConnect.AddClientEventToNotificationGroup(GroupEventID, DecrementEventID, false);
      }

      //public void RegisterInputs()
      //{
      //   _incHash = _inputDataMap[IncrementInputName];
      //   _decHash = _inputDataMap[DecrementInputName];
      //}

      public void SendInputsToSim(SimConnect simConnect)
      {
         if (State == EncoderState.INCREMENT)
         {
            simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, IncrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
            State = EncoderState.STILL;
         }
         else if (State == EncoderState.DECREMENT)
         {
            simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, DecrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
            State = EncoderState.STILL;
         }

         //uint newPos = (uint)(CurrentPosition + PositionChange);
         //simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, SetEventID, newPos, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
         //State = EncoderState.STILL;
         //PositionChange = 0;

         //if (CurrentPosition == SetPosition)
         //{
         //   State = EncoderState.STILL;
         //}
         //else if (CurrentPosition > SetPosition)
         //{
         //   State = EncoderState.DECREMENT;
         //   simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, DecrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
         //   CurrentPosition--;
         //}
         //else
         //{
         //   State = EncoderState.INCREMENT;
         //   simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, IncrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
         //   CurrentPosition++;
         //}
      }
      #endregion

      #region Full Props
      public PanelEncoder? Name
      {
         get => _name;
         set
         {
            _name = value;
            OnPropertyChanged();
         }
      }

      public EncoderState State
      {
         get => _state;
         set
         {
            _state = value;
            OnPropertyChanged();
         }
      }

      public string IncrementInputName
      {
         get => _incInputName;
         set
         {
            _incInputName = value;
            OnPropertyChanged();
         }
      }

      public string DecrementInputName
      {
         get => _decInputName;
         set
         {
            _decInputName = value;
            OnPropertyChanged();
         }
      }

      public string SetInputName
      {
         get => _setInputName;
         set
         {
            _setInputName = value;
            OnPropertyChanged();
         }
      }

      public EncoderEventID IncrementEventID
      {
         get => _incEventID;
         set
         {
            _incEventID = value;
            OnPropertyChanged();
         }
      }

      public EncoderEventID DecrementEventID
      {
         get => _decEventID;
         set
         {
            _decEventID = value;
            OnPropertyChanged();
         }
      }

      public EncoderEventID SetEventID
      {
         get => _setEventID;
         set
         {
            _setEventID = value;
            OnPropertyChanged();
         }
      }

      public bool Enabled
      {
         get => _enabled;
         set
         {
            _enabled = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT HEADING LOCK DIR", "DEGREES")]
      public uint CurrentPosition
      {
         get => _currPos;
         set
         {
            _currPos = value;
            OnPropertyChanged();
         }
      }

      public int PositionChange
      {
         get => _posChange;
         set
         {
            _posChange = value;
            OnPropertyChanged();
         }
      }

      public int Difference => (int)_currPos + _posChange;
      #endregion
   }
}
