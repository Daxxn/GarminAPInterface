using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Microsoft.FlightSimulator.SimConnect;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.AttributeModels;
using TBMAutopilotDashboard.Models.Enums;
using TBMAutopilotDashboard.Models.Enums.Constants;
using TBMAutopilotDashboard.Models.SimConnectData;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminButtons : Model
   {
      #region Local Props
      private SimConnect _simConnect;
      private readonly MessageController _messages;
      private static readonly SolidColorBrush pressedColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
      private static readonly SolidColorBrush releasedColor = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
      private static readonly InputDataMap _inputDataMap = InputDataMap.Instance;

      public GarminButtonModel this[int btn]
      {
         get => States[btn];
         set
         {
            States[btn] = value;
            OnPropertyChanged(nameof(States));
         }
      }

      public bool this[PanelButton btn]
      {
         get => States[(int)btn].State;
         set
         {
            States[(int)btn].State = value;
            OnPropertyChanged(PanelButtonNames.FromEnum[btn]);
            OnPropertyChanged(PanelButtonNames.ColorFromEnum[btn]);
         }
      }
      public bool this[string btn]
      {
         get => States[PanelButtonNames.ToIndex[btn]].State;
         set
         {
            States[PanelButtonNames.ToIndex[btn]].State = value;
            OnPropertyChanged(btn);
            OnPropertyChanged($"{btn}_Color");
         }
      }
      private AsyncObservableCollection<GarminButtonModel> _states = new AsyncObservableCollection<GarminButtonModel>();
      //private Dictionary<PanelButton, ulong> Hashes { get; set; } = new Dictionary<PanelButton, ulong>();
      private bool _stateChanged = false;
      private bool _stateLock = false;
      #endregion

      #region Constructors
      public GarminButtons(MessageController messages)
      {
         _messages = messages;
         foreach (PanelButton pb in Enum.GetValues(typeof(PanelButton)))
         {
            States.Add(new GarminButtonModel(pb));
            //Hashes.Add(pb, 0);
         }
      }
      #endregion

      #region Methods
      public void ConnectModel(SimConnect simConnect)
      {
         _simConnect = simConnect;
      }

      public void RegisterSimData()
      {
         var props = GetType().GetProperties(BindingFlags.Public);
         foreach (var prop in props)
         {
            var attrs = prop.GetCustomAttributes();
            foreach (var attr in attrs)
            {
               if (attr is SimDataAttribute dataAttr)
               {
                  _simConnect.AddToDataDefinition(
                     PanelButtonNames.ToEnum[prop.Name],
                     dataAttr.SimDataName,
                     dataAttr.SimUnits,
                     dataAttr.Type,
                     0.0f,
                     SimConnect.SIMCONNECT_UNUSED
                  );
               }
            }
         }
      }

      public void InitSimInputs()
      {
         // Need to decide if its worth getting the hashes from the server
         // EVERY time the app is started. The hashes dont seem to change
         // but it would make things easier. If not very efficient...
         var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
         foreach (var prop in props)
         {
            var attr = prop.GetCustomAttribute<InputEventAttribute>();
            if (attr != null)
            {
               States[PanelButtonNames.ToIndex[prop.Name]].Hash = _inputDataMap[attr.InputEventName];
               //Hashes[PanelButtonNames.ToEnum[prop.Name]] = _inputDataMap[attr.InputEventName];
            }
         }

         // May be a bad idea...
         //SendInputsToSim(simConnect);
      }

      public void SendInputsToSim()
      {
         if (_stateLock) return;
         if (!_stateChanged) return;
         try
         {
            List<PanelButton> updatedButtons = new List<PanelButton>();
            foreach (var btn in States)
            {
               if (btn.Hash != 0 && btn.State == true)
               {
                  _simConnect.SetInputEvent(btn.Hash, btn.State);
                  btn.UpdateState();
                  //updatedButtons.Add(btn.Key);
                  //OnPropertyChanged(PanelButtonNames.FromEnum[btn.Key]);
               }
            }
            _stateChanged = false;
            //foreach (var btn in updatedButtons)
            //{
            //   //States[btn] = false;
            //   OnPropertyChanged($"{PanelButtonNames.FromEnum[btn]}_Color");
            //   _stateChanged = false;
            //}
         }
         catch (InvalidOperationException)
         {
            _messages.Add(new Message("Button collision!", Messagetype.ERROR));
         }
      }

      public void ReceiveData(byte[] buffer)
      {
         _stateLock = true;
         uint temp = buffer[0];
         temp |= (uint)(buffer[1] << 8);
         temp |= (uint)(buffer[2] << 16);
         for (int i = 0; i < PanelButtonNames.ButtonCount; i++)
         {
            bool tempBtn = Convert.ToBoolean(temp & (1 << i));
            if (this[(PanelButton)i] != tempBtn)
            {
               _stateChanged = true;
            }
            this[(PanelButton)i] = tempBtn;
         }
         _stateLock = false;
      }
      #endregion

      #region Full Props
      public AsyncObservableCollection<GarminButtonModel> States
      {
         get => _states;
         set
         {
            _states = value;
            OnPropertyChanged();
         }
      }

      [SimData("AUTOPILOT HEADING LOCK")]
      [SimEvent("AP_HDG_HOLD")]
      [InputEvent("AUTOPILOT_Heading_Mode")]
      public bool HDG
      {
         get => this[PanelButton.HDG];
         set
         {
            this[PanelButton.HDG] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(HDG_Color));
         }
      }

      [SimData("AUTOPILOT APPROACH HOLD")]
      [SimEvent("AP_APR_HOLD")]
      [InputEvent("AUTOPILOT_Approach_Button")]
      public bool APR
      {
         get => this[PanelButton.APR];
         set
         {
            this[PanelButton.APR] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(APR_Color));
         }
      }

      [SimData("AUTOPILOT BACKCOURSE HOLD")]
      [SimEvent("AP_BC_HOLD")]
      [InputEvent("AUTOPILOT_Backcourse_Button")]
      public bool BC
      {
         get => this[PanelButton.BC];
         set
         {
            this[PanelButton.BC] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(BC_Color));
         }
      }

      /// <summary>
      /// Giving NAV1 a try. It may not work.
      /// </summary>
      [SimData("AUTOPILOT NAV1 LOCK")]
      [SimEvent("AP_NAV1_HOLD")]
      [InputEvent("AUTOPILOT_NAV_Mode")]
      public bool NAV
      {
         get => this[PanelButton.NAV];
         set
         {
            this[PanelButton.NAV] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(NAV_Color));
         }
      }

      [SimData("AUTOPILOT FLIGHT DIRECTOR ACTIVE")]
      [SimEvent("TOGGLE_FLIGHT_DIRECTOR")]
      [InputEvent("AUTOPILOT_FD_1_Mode")]
      public bool FD
      {
         get => this[PanelButton.FD];
         set
         {
            this[PanelButton.FD] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(FD_Color));
         }
      }

      [SimData("AUTOPILOT BANK HOLD")]
      [SimEvent("AP_BANK_HOLD")]
      [InputEvent("AUTOPILOT_Bank_Button")]
      public bool BANK
      {
         get => this[PanelButton.BANK];
         set
         {
            this[PanelButton.BANK] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(BANK_Color));
         }
      }

      [SimData("AUTOPILOT MASTER")]
      [SimEvent("AP_MASTER")]
      [InputEvent("AUTOPILOT_AP_1")]
      public bool AP
      {
         get => this[PanelButton.AP];
         set
         {
            this[PanelButton.AP] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(AP_Color));
         }
      }

      /// <summary>
      /// This is basically a vanity button.
      /// There isnt any references to it in the SimConnect API.
      /// It may be a function that will just toggles the lights.
      /// </summary>
      [InputEvent("AUTOPILOT_Transfer_Mode")]
      public bool XFR
      {
         get => this[PanelButton.XFR];
         set
         {
            this[PanelButton.XFR] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(XFR_Color));
         }
      }

      [SimData("AUTOPILOT YAW DAMPER")]
      [SimEvent("YAW_DAMPER_TOGGLE")]
      [InputEvent("AUTOPILOT_YD_Button")]
      public bool YD
      {
         get => this[PanelButton.YD];
         set
         {
            this[PanelButton.YD] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(YD_Color));
         }
      }

      [SimData("AUTOPILOT ALTITUDE LOCK")]
      [SimEvent("AP_ALT_HOLD")]
      [InputEvent("AUTOPILOT_Altitude_Button")]
      public bool ALT
      {
         get => this[PanelButton.ALT];
         set
         {
            this[PanelButton.ALT] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(ALT_Color));
         }
      }

      [SimData("AUTOPILOT VERTICAL HOLD")]
      [SimEvent("AP_PANEL_VS_HOLD")]
      [InputEvent("AUTOPILOT_VS_Mode")]
      public bool VS
      {
         get => this[PanelButton.VS];
         set
         {
            this[PanelButton.VS] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(VS_Color));
         }
      }

      /// <summary>
      /// This one is a bit weird. Because it wasnt well-supported in the past,
      /// it doesnt seem to have a command in the SimConnect API.
      /// </summary>
      [InputEvent("AUTOPILOT_VNAV_Mode")]
      public bool VNV
      {
         get => this[PanelButton.VNV];
         set
         {
            this[PanelButton.VNV] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(VNV_Color));
         }
      }

      [SimData("AUTOPILOT FLIGHT LEVEL CHANGE")]
      [SimEvent("FLIGHT_LEVEL_CHANGE")]
      [InputEvent("AUTOPILOT_FLC_Button")]
      public bool FLC
      {
         get => this[PanelButton.FLC];
         set
         {
            this[PanelButton.FLC] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(FLC_Color));
         }
      }

      [SimData("AUTOPILOT MANAGED SPEED IN MACH")]
      [SimEvent("AP_MANAGED_SPEED_IN_MACH_TOGGLE")]
      [InputEvent("AUTOPILOT_SpeedToggle_Mode")]
      public bool SPD
      {
         get => this[PanelButton.SPD];
         set
         {
            this[PanelButton.SPD] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(SPD_Color));
         }
      }

      /// <summary>
      /// Supposed to "Re-center CDI"
      /// <para/> Not sure what that means right now.
      /// </summary>
      [InputEvent("AUTOPILOT_Course_1_Sync")]
      public bool CRS1_ENC
      {
         get => this[PanelButton.CRS1_ENC];
         set
         {
            this[PanelButton.CRS1_ENC] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(CRS1_ENC_Color));
         }
      }

      /// <summary>
      /// Supposed to "Re-center CDI"
      /// <para/> Not sure what that means right now.
      /// </summary>
      [InputEvent("AUTOPILOT_Course_2_Sync")]
      public bool CRS2_ENC
      {
         get => this[PanelButton.CRS2_ENC];
         set
         {
            this[PanelButton.CRS2_ENC] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(CRS2_ENC_Color));
         }
      }

      [InputEvent("AUTOPILOT_Heading_Sync")]
      public bool HDG_ENC
      {
         get => this[PanelButton.HDG_ENC];
         set
         {
            this[PanelButton.HDG_ENC] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(HDG_ENC_Color));
         }
      }

      /// <summary>
      /// Aparently this doesnt have an associated function...
      /// </summary>
      public bool ALT_ENC
      {
         get => this[PanelButton.ALT_ENC];
         set
         {
            this[PanelButton.ALT_ENC] = value;
            //OnPropertyChanged();
            //OnPropertyChanged(nameof(ALT_ENC_Color));
         }
      }

      public bool StateChanged => _stateChanged;

      public SolidColorBrush HDG_Color => HDG ? pressedColor : releasedColor;
      public SolidColorBrush APR_Color => APR ? pressedColor : releasedColor;
      public SolidColorBrush BC_Color => BC ? pressedColor : releasedColor;
      public SolidColorBrush NAV_Color => NAV ? pressedColor : releasedColor;
      public SolidColorBrush FD_Color => FD ? pressedColor : releasedColor;
      public SolidColorBrush BANK_Color => BANK ? pressedColor : releasedColor;
      public SolidColorBrush AP_Color => AP ? pressedColor : releasedColor;
      public SolidColorBrush XFR_Color => XFR ? pressedColor : releasedColor;
      public SolidColorBrush YD_Color => YD ? pressedColor : releasedColor;
      public SolidColorBrush ALT_Color => ALT ? pressedColor : releasedColor;
      public SolidColorBrush VS_Color => VS ? pressedColor : releasedColor;
      public SolidColorBrush VNV_Color => VNV ? pressedColor : releasedColor;
      public SolidColorBrush FLC_Color => FLC ? pressedColor : releasedColor;
      public SolidColorBrush SPD_Color => SPD ? pressedColor : releasedColor;
      public SolidColorBrush CRS1_ENC_Color => CRS1_ENC ? pressedColor : releasedColor;
      public SolidColorBrush CRS2_ENC_Color => CRS2_ENC ? pressedColor : releasedColor;
      public SolidColorBrush HDG_ENC_Color => HDG_ENC ? pressedColor : releasedColor;
      public SolidColorBrush ALT_ENC_Color => ALT_ENC ? pressedColor : releasedColor;
      #endregion
   }
}
