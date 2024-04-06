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
   public class GarminEncoder : Model
   {
      #region Local Props
      //private static readonly InputDataMap _inputDataMap = InputDataMap.Instance;
      private static EncoderGroupEventID GroupEventID = EncoderGroupEventID.DEFAULT;

      private EncoderState _state = EncoderState.STILL;
      private byte _currPos = 0;
      private byte _setPos = 0;

      private string _incInputName = null;
      private string _decInputName = null;
      private EncoderEventID _incEventID;
      private EncoderEventID _decEventID;

      private bool _enabled = true;

      //private ulong _incHash;
      //private ulong _decHash;
      #endregion

      #region Constructors
      public GarminEncoder() { }
      #endregion

      #region Methods
      public void RegisterSimData(SimConnect simConnect)
      {
         simConnect.MapClientEventToSimEvent(IncrementEventID, IncrementInputName);
         simConnect.MapClientEventToSimEvent(DecrementEventID, DecrementInputName);
         simConnect.AddClientEventToNotificationGroup(GroupEventID, IncrementEventID, false);
         simConnect.AddClientEventToNotificationGroup(GroupEventID, DecrementEventID, false);
      }
      //public void RegisterInputs()
      //{
      //   _incHash = _inputDataMap[IncrementInputName];
      //   _decHash = _inputDataMap[DecrementInputName];
      //}

      public void SendInputsToSim(SimConnect simConnect)
      {
         //if (_incHash == 0 || _decHash == 0) return;
         //simConnect.SetInputEvent(_incHash, State == EncoderState.INCREMENT);
         //simConnect.SetInputEvent(_decHash, State == EncoderState.DECREMENT);

         //if (State == EncoderState.INCREMENT)
         //{
         //   simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, IncrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
         //}
         //else if (State == EncoderState.DECREMENT)
         //{
         //   simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, DecrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
         //}

         if (CurrentPosition == SetPosition)
         {
            State = EncoderState.STILL;
         }
         else if (CurrentPosition > SetPosition)
         {
            State = EncoderState.DECREMENT;
            simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, DecrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
            CurrentPosition--;
         }
         else
         {
            State = EncoderState.INCREMENT;
            simConnect.TransmitClientEvent((uint)SIMCONNECT_SIMOBJECT_TYPE.USER, IncrementEventID, 0, GroupEventID, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
            CurrentPosition++;
         }
      }
      #endregion

      #region Full Props
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

      public bool Enabled
      {
         get => _enabled;
         set
         {
            _enabled = value;
            OnPropertyChanged();
         }
      }

      public byte SetPosition
      {
         get => _setPos;
         set
         {
            _setPos = value;
            OnPropertyChanged();
         }
      }

      public byte CurrentPosition
      {
         get => _currPos;
         set
         {
            _currPos = value;
            OnPropertyChanged();
         }
      }

      public int Difference => _currPos - _setPos;
      #endregion
   }
}
