using MVVMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO.Ports;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Runtime.InteropServices;
using ArduinoSerialLibrary;
using System.Windows.Controls;
using System.Windows;
using TBMAutopilotDashboard.SimUtils;
using TBMAutopilotDashboard.Models.SimConnectData;
using TBMAutopilotDashboard.Models;
using TBMAutopilotDashboard.Models.Enums;
using TBMAutopilotDashboard.Models.Enums.Constants;
using TBMAutopilotDashboard.Models.State;
using SettingsLibrary;
using System.Windows.Markup;

namespace TBMAutopilotDashboard
{
   public class MainViewModel : ViewModel, IBaseSimConnectWrapper
   {
      #region - Fields & Properties
      private SettingsModel _settings;
      private readonly InputDataMap _inputDataMap = InputDataMap.Instance;

      public const string _name = "Garmin_Autopilot_Client";
      /// <summary>
      /// User-defined win32 event?
      /// It's not clear what this variable is for.
      /// <para/>
      /// Just part of the encantation I guess...
      /// </summary>
      #region SimConnectWrapper
      public const int WM_USER_SIMCONNECT = 0x0402;
      /// <summary>
      /// Window Handle (Still not sure what do...)
      /// </summary>
      private IntPtr Wnd = IntPtr.Zero;
      #endregion

      /// <summary>
      /// SimConnect API Object
      /// </summary>
      private SimConnect SimConnect = null;
      private bool _connected;
      private bool _paused = true;
      private bool _getInputData = false;

      private MessageController _messages;
      private GarminPanelModel _panelStates;

      private PortStatus _selectedPort;
      private AsyncObservableCollection<PortStatus> _ports;

      private int _indIndex = 0;

      #region Command Bindings
      public Command ConnectCmd { get; private set; }
      public Command DisconnectCmd { get; private set; }
      public Command PauseCmd { get; private set; }
      public Command StartSerialCmd { get; private set; }
      public Command DebugStartSerialCmd { get; private set; }
      public Command StopSerialCmd { get; private set; }
      public Command RefreshPortsCmd { get; private set; }
      public Command TestCmd { get; private set; }
      public Command PanelTestCmd { get; private set; }
      #endregion

      #region Constants
      private readonly SIMCONNECT_SIMOBJECT_TYPE _simObjectType = SIMCONNECT_SIMOBJECT_TYPE.USER;
      public uint _timerInterval = 100;
      #endregion

      // This timer will probably be used as a sim startup delay timer.
      // Due to the posibility of multiple sim start and stop event when loading in,
      // it would be good to delay any automatic connection until after all the BS has stopped.
      //public Timer Timer { get; private set; }
      private Timer DeviceCheckTimer { get; set; }

      public GarminSerialController Serial { get; set; }
      private int _customBaudRate;
      #endregion

      #region - Constructors
      public MainViewModel()
      {
         _messages = new MessageController(50);
         _panelStates = new GarminPanelModel(_messages);

         ConnectCmd = new Command(Connect);
         DisconnectCmd = new Command(Disconnect);
         PauseCmd = new Command(Pause);
         PanelTestCmd = new Command(PanelTest);

         TestCmd = new Command(Test);

         //Timer = new Timer
         //{
         //   AutoReset = true,
         //   Interval = _timerInterval
         //};

         //Timer.Elapsed += OnTick;

         DeviceCheckTimer = new Timer
         {
            AutoReset = true,
            Interval = 1000,
            Enabled = true
         };
         DeviceCheckTimer.Elapsed += DeviceCheckTimer_Elapsed;

         StartSerialCmd = new Command(StartSerial);
         DebugStartSerialCmd = new Command(SendDebugSerial);
         StopSerialCmd = new Command(StopSerial);
         RefreshPortsCmd = new Command(RefreshPorts);

         Serial = new GarminSerialController(PanelStates, Messages);
         OpenPortsList = new AsyncObservableCollection<PortStatus>(SerialManager.GetOpenPorts());
      }

      #endregion

      #region - Methods
      private void Pause()
      {
         IsPaused = true;
         Messages.Add(new Message("Pause", Messagetype.WARN));
         Serial.StopStream();
      }

      private void Resume()
      {
         IsPaused = false;
         Messages.Add(new Message("Resume", Messagetype.WARN));
         Serial.StartStream();
      }

      private void PanelTest(object param)
      {
         if (param is string str)
         {
            PanelStates.Buttons[str] = true;
         }
      }

      public void PanelButtonPush(string name)
      {
         PanelStates.Buttons[name] = true;
      }

      public void PanelButtonRelease(string name)
      {
         PanelStates.Buttons[name] = false;
      }

      private void Test()
      {
         if (_indIndex < 16)
         {
            PanelStates.Indicators[(PanelIndicator)_indIndex] = true;
            _indIndex++;
         }
         else
         {
            PanelStates.Indicators.ClearIndicators();
            _indIndex = 0;
         }
      }

      #region Background Setup
      public int GetUserSimConnectWinEvent()
      {
         return WM_USER_SIMCONNECT;
      }

      public void ReceiveSimConnectMessage()
      {
         SimConnect?.ReceiveMessage();
      }

      public void SetWindowHandle(IntPtr _hWnd)
      {
         Wnd = _hWnd;
      }
      #endregion

      public void Disconnect()
      {
         Messages.Add(new Message("Disconnecting from Simulator..."));

         //Timer.Stop();

         if (SimConnect != null)
         {
            SimConnect.Dispose();
            SimConnect = null;
         }

         Connected = false;
         IsPaused = true;

         // OLD
         //SetRequestsPendingState(true);
      }

      private void Connect()
      {
         Messages.Add(new Message("Connecting to Simulator..."));

         try
         {
            SimConnect = new SimConnect(_name, Wnd, WM_USER_SIMCONNECT, null, 0);
            SimConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(OnRecvOpen);
            SimConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnRecvQuit);
            SimConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnRecvException);
            SimConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnRecvSimobjectDataBytype);
            SimConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(OnRecvSimobjectData);
            SimConnect.OnRecvEnumerateInputEvents += new SimConnect.RecvEnumerateInputEventsEventHandler(OnRecvEnumerateInputEvents);
            SimConnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(OnRecvEvent);
            SimConnect.OnRecvSystemState += new SimConnect.RecvSystemStateEventHandler(SimConnect_OnRecvSystemState);

            SimConnect.SubscribeToSystemEvent(SimSystemEvent.SIM_START, SimSysEventName.simStart);
            SimConnect.SubscribeToSystemEvent(SimSystemEvent.SIM_STOP, SimSysEventName.simStop);
            SimConnect.SubscribeToSystemEvent(SimSystemEvent.EVERY_6HZ, SimSysEventName.sixHz);
         }
         catch (COMException ex)
         {
            Messages.Add(new Message(ex.Message, 1));
         }
      }

      /// <summary>
      /// Called when the simulator fires any kind of system event.
      /// <para/>
      /// Usually stuff like, statring/stopping a flight, pausing, quitting, things like that.
      /// <para/>
      /// <seealso href="https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Events_And_Data/SimConnect_SubscribeToSystemEvent.htm">Look here</seealso> for a list of events
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="data"></param>
      private void OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data)
      {
         if (data.uEventID == (uint)SimSystemEvent.SIM_START)
         {
            Resume();
         }
         else if (data.uEventID == (uint)SimSystemEvent.SIM_STOP)
         {
            Pause();
         }
         else if (data.uEventID == (uint)SimSystemEvent.EVERY_6HZ)
         {
            On6HzTick();
         }
      }

      private void SimConnect_OnRecvSystemState(SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data)
      {
         if (data.dwInteger == 1)
         {
            Resume();
         }
         else if (data.dwInteger == 0)
         {
            Pause();
         }
      }

      #region SimConnect Events
      private void OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
      {
         Messages.Add(new Message("Opening Simulator Connection"));

         Connected = true;

         PanelStates.RegisterSimData(SimConnect);

         if (_getInputData)
         {
            Test();
         }

         SimConnect.RequestSystemState(RequestID.SYSTEM_STATE, "Sim");
      }

      private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
      {
         Messages.Add(new Message("Closing Simulator Connection"));

         Disconnect();
      }

      private void OnRecvEnumerateInputEvents(SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data)
      {
         try
         {
            _inputDataMap.OnRecvEnumeratedInputEvents(data);
            _getInputData = false;
         }
         catch (Exception e)
         {
            MessageBox.Show($"Unable to read the list of input events.\n\n{e.Message}", "Error");
         }
      }

      /// <summary>
      /// Called when an error occurs with the <see cref="SimConnect"/> server.
      /// </summary>
      /// <param name="sender">Ignored</param>
      /// <param name="data">Error message and other info about the error.</param>
      private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
      {
         SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
         Console.WriteLine("SimConnect_OnRecvException: " + eException.ToString());

         Messages.Add(new Message("SimConnect : " + eException.ToString(), Messagetype.ERROR));
      }

      /// <summary>
      /// DEPRECIATED
      /// <para/>
      /// Not used anymore. Instead use <see cref="OnRecvSimobjectData(SimConnect, SIMCONNECT_RECV_SIMOBJECT_DATA)"/>
      /// <para/>
      /// <see href="https://forums.flightsimulator.com/t/event-ids-indexing/523851/8">This guy</see> asid something about this being more efficient than <c>OnRecvSimobjectData()</c>
      /// <para/>
      /// Not sure. Keep around just in case performance becomes and issue.
      /// <para/>
      /// <see href="https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Events_And_Data/SimConnect_RequestDataOnSimObjectType.htm">look here</see>
      /// for more info on this event.
      /// </summary>
      private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
      {
         //uint iDefinition = data.dwDefineID;
         //uint iRequest = data.dwRequestID;
         //uint iObject = data.dwObjectID;
         //var values = data.dwData;
         //var index = data.dwentrynumber;
      }

      /// <summary>
      /// Called when the requested data is returned from <see cref="SimConnect"/>
      /// </summary>
      /// <param name="sender">Redundant. Use <see cref="SimConnect"/></param>
      /// <param name="data">Structure containing the data from the SimConnect server. <see href="https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Structures_And_Enumerations/SIMCONNECT_RECV_SIMOBJECT_DATA.htm">Look here</see> for more.</param>
      private void OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
      {
         PanelStates.Indicators.ReadSimData(data);
      }
      #endregion

      /// <summary>
      /// initializes the input event hash dictionary and loads the user's settings.
      /// </summary>
      public void OnStartup()
      {
         _getInputData = _inputDataMap.OnStartup();
         //_settings = SettingsManager.OnStartup<SettingsModel>(nameof(TBMAutopilotDashboard));
      }

      /// <summary>
      /// After all the data handling is figured out, this should probably be removed and replaced with a single call that sets the SIMCONNECT_PERIOD to ?SIM_FRAME?
      /// <para/>
      /// OR
      /// <para/>
      /// The system event "6Hz" could be used instead. It gives a decent refresh rate without drowning everything in requests.
      /// </summary>
      /// <param name="_">NOT USED</param>
      /// <param name="__">NOT USED</param>
      private void OnTick(object _, ElapsedEventArgs __)
      {
         // Send button states...
         PanelStates.SendButtonStates(SimConnect);

         // Send indicator data request...
         SimConnect?.RequestDataOnSimObject(RequestID.INDICATOR, StructDefinition.INDICATOR_MESSAGE, (uint)_simObjectType, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

         // Handle all the serial comms with the controller...
         //Serial.SendData(TBMData.BuildData(VariableRequests));
      }

      private void On6HzTick()
      {
         if (IsPaused || !Connected) return;
         // Send button states...
         PanelStates.SendButtonStates(SimConnect);

         // Send indicator data request...
         SimConnect?.RequestDataOnSimObject(RequestID.INDICATOR, StructDefinition.INDICATOR_MESSAGE, (uint)_simObjectType, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

         // Handle all the serial comms with the controller...
         Serial.SendIndicators();
      }

      /// <summary>
      /// DEPRECIATED
      /// <para>
      /// This gets data one variable at a time. Its not as useful as the new way.
      /// </para>
      /// Also, adding to the data definition is handled in the <see cref="GarminIndicators"/> class.
      /// </summary>
      /// <param name="_simvarRequest">Old simvar definition. Using more specific classes now. Like, <see cref="GarminIndicators"/></param>
      /// <returns></returns>
      private bool RegisterToSimConnect(Variable _simvarRequest)
      {
         if (SimConnect != null)
         {
            SimConnect.AddToDataDefinition(
               _simvarRequest.Definition,
               _simvarRequest.Name,
               _simvarRequest.Units,
               SIMCONNECT_DATATYPE.FLOAT64,
               0.0f,
               SimConnect.SIMCONNECT_UNUSED
            );
            SimConnect.RegisterDataDefineStruct<double>(_simvarRequest.Definition);

            return true;
         }
         else
         {
            return false;
         }
      }

      public void SendDebugSerial()
      {
         if (IsPaused)
         {
            Serial.StartStream();
            IsPaused = false;
         }
         else
         {
            Serial.StopStream();
            IsPaused = true;
         }
      }

      public void StartSerial()
      {
         try
         {
            if (SelectedPort is null) return;
            Messages.Add(new Message("Starting comms with controller.."));
            Serial.InitPort(SelectedPort.Name);
         }
         catch (Exception e)
         {
            Messages.Add(new Message(e.Message, 1));
         }
      }

      public void StopSerial()
      {
         try
         {
            Messages.Add(new Message("Stopping comms with controller.."));
            Serial.StopStream();
            Serial.Close();
            IsPaused = true;
         }
         catch (Exception e)
         {
            Messages.Add(new Message(e.Message, 1));
         }
      }

      public void RefreshPorts()
      {
         var tempList = SerialManager.GetOpenPorts();
         if (OpenPortsList.Any((p) => !tempList.Contains(p)))
         {
            OpenPortsList = new AsyncObservableCollection<PortStatus>(tempList);
         }
      }

      private void DeviceCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
      {
         RefreshPorts();
      }

      public void AddMessage(Message msg)
      {
         Messages.Add(msg);
      }

      public void SliderValueChanged(string name)
      {
         if (!Serial.IsOpen) return;
         switch (name)
         {
            case "BacklghtSlider":
               Serial.SendBacklight();
               break;
            case "MaxBacklghtSlider":
               Serial.SendBacklight();
               break;
            case "IndBrghtSlider":
               Serial.SendIndBrightness();
               break;
            case "MaxIndBrghtSlider":
               Serial.SendMaxIndBrightness();
               break;
            default:
               break;
         }
      }
      #endregion

      #region - Full Properties 
      public bool Connected
      {
         get { return _connected; }
         set
         {
            _connected = value;
            if (!_connected)
            {
               IsPaused = value;
            }
            OnPropertyChanged();
         }
      }
      public bool IsPaused
      {
         get { return _paused; }
         set
         {
            _paused = value;
            OnPropertyChanged();
         }
      }

      public MessageController Messages
      {
         get { return _messages; }
         set
         {
            _messages = value;
            OnPropertyChanged();
         }
      }

      public PortStatus SelectedPort
      {
         get { return _selectedPort; }
         set
         {
            _selectedPort = value;
            OnPropertyChanged();
         }
      }

      public AsyncObservableCollection<PortStatus> OpenPortsList
      {
         get { return _ports; }
         set
         {
            _ports = value;
            OnPropertyChanged();
         }
      }

      public int CustomBaudRate
      {
         get { return _customBaudRate; }
         set
         {
            _customBaudRate = value;
            OnPropertyChanged();
         }
      }

      public GarminPanelModel PanelStates
      {
         get => _panelStates;
         set
         {
            _panelStates = value;
            OnPropertyChanged();
         }
      }

      public SettingsModel Settings
      {
         get => _settings;
         set
         {
            _settings = value;
            OnPropertyChanged();
         }
      }
      #endregion
   }
}
