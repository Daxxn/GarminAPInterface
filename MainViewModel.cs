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
   public enum DataRequestID
   {
      ID_0 = 0, ID_1 = 1, ID_2 = 2, ID_3 = 3,
   };

   public class MainViewModel : ViewModel, IBaseSimConnectWrapper
   {
      #region - Fields & Properties
      private SettingsModel _settings;
      private readonly InputDataMap _inputDataMap = InputDataMap.Instance;

      public const string _name = "TBMAutopilot-Client";
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

      private GarminPanelModel _panelStates = new GarminPanelModel();

      private MessageController _messages = new MessageController(50);

      private PortStatus _selectedPort;
      private ObservableCollection<PortStatus> _ports;

      #region Command Bindings
      public Command ConnectCmd { get; private set; }
      public Command DisconnectCmd { get; private set; }
      public Command PauseCmd { get; private set; }
      public Command StartSerialCmd { get; private set; }
      public Command DebugStartSerialCmd { get; private set; }
      public Command StopSerialCmd { get; private set; }
      public Command RefreshPortsCmd { get; private set; }
      public Command GetInputEventsCMD { get; private set; }
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

      #region Test Buttons
      //public Dictionary<string, bool> _testButtons = new Dictionary<string, bool>
      //{
      //   { "HDG", false },
      //   { "APR", false },
      //   { "BC", false },
      //   { "NAV", false },
      //   { "FD", false },
      //   { "BANK", false },
      //   { "AP", false },
      //   { "YD", false },
      //   { "ALT", false },
      //   { "VS", false },
      //   { "FLC", false }
      //};
      //public Dictionary<uint, string> _testButtonIndex = new Dictionary<uint, string>
      //{
      //   { 0, "HDG" },
      //   { 1, "APR" },
      //   { 2, "BC" },
      //   { 3, "NAV" },
      //   { 4, "FD" },
      //   { 5, "BANK" },
      //   { 6, "AP" },
      //   { 7, "YD" },
      //   { 8, "ALT" },
      //   { 9, "VS" },
      //   { 10, "FLC" }
      //};
      #endregion

      public SerialManager<TBMData> Serial { get; set; }
      private int _customBaudRate;
      #endregion

      #region - Constructors
      public MainViewModel()
      {
         //VariableRequests = new ObservableCollection<Variable>(FileManager.ReadVariables());
         //foreach (var request in VariableRequests)
         //{
         //   request.IsPending = !RegisterToSimConnect(request); // Add a check for "SimConnect is null".
         //}

         ConnectCmd = new Command(Connect);
         DisconnectCmd = new Command(Disconnect);
         PauseCmd = new Command(Pause);
         PanelTestCmd = new Command(PanelTest);

         GetInputEventsCMD = new Command(GetInputEvents);

         //Timer = new Timer
         //{
         //   AutoReset = true,
         //   Interval = _timerInterval
         //};

         //Timer.Elapsed += OnTick;

         StartSerialCmd = new Command(o => StartSerial());
         DebugStartSerialCmd = new Command(o => SendDebugSerial());
         StopSerialCmd = new Command(o => StopSerial());
         RefreshPortsCmd = new Command(o => RefreshPorts());

         Serial = new SerialManager<TBMData>(TBMData.ConvertData, CustomBaudRate, TBMData.DataSize);
         OpenPortsList = new ObservableCollection<PortStatus>(SerialManager.GetOpenPorts());
      }
      #endregion

      #region - Methods
      private void Pause()
      {
         IsPaused = !IsPaused;
      }

      private void Resume()
      {
         IsPaused = false;
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

      private void GetInputEvents()
      {
         if (SimConnect is null) return;

         SimConnect.EnumerateInputEvents(DataRequestID.ID_0);
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
         Messages.Add(new Message("Disconnecting..."));

         //Timer.Stop();

         if (SimConnect != null)
         {
            SimConnect.Dispose();
            SimConnect = null;
         }

         Connected = false;

         SetRequestsPendingState(true);
      }

      private void Connect()
      {
         Messages.Add(new Message("Connecting..."));

         try
         {
            SimConnect = new SimConnect(_name, Wnd, WM_USER_SIMCONNECT, null, 0);
            SimConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(OnRecvOpen);
            SimConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnRecvQuit);
            SimConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnRecvException);
            SimConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnRecvSimobjectDataBytype);
            SimConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(OnRecvSimobjectData);
            SimConnect.OnRecvEnumerateInputEvents += new SimConnect.RecvEnumerateInputEventsEventHandler(OnRecvEnumerateInputEvents);
            SimConnect.OnRecvSystemState += new SimConnect.RecvSystemStateEventHandler(OnRecvSystemState);
            SimConnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(OnRecvEvent);

            SimConnect.SubscribeToSystemEvent(SimSystemEvent.SIM_START, SimSysEventName.simStart);
            SimConnect.SubscribeToSystemEvent(SimSystemEvent.SIM_STOP, SimSysEventName.simStop);
            SimConnect.SubscribeToSystemEvent(SimSystemEvent.EVERY_6HZ, SimSysEventName.sixHz);
         }
         catch (COMException ex)
         {
            Messages.Add(new Message(ex.Message, 1));
         }
      }

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

      private void OnRecvSystemState(SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data)
      {
         if (data.dwID == (uint)SimSystemEvent.SIM_START)
         {
            Resume();
         }
         else if (data.dwRequestID == (uint)SimSystemEvent.SIM_STOP)
         {
            Pause();
         }
         else if (data.dwRequestID == (uint)SimSystemEvent.EVERY_6HZ)
         {
            On6HzTick();
         }
      }

      #region SimConnect Events
      private void OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
      {
         Messages.Add(new Message("Open Connection"));

         Connected = true;

         PanelStates.RegisterSimData(SimConnect);

         if (_getInputData)
         {
            GetInputEvents();
         }
      }

      private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
      {
         Messages.Add(new Message("Closing Connection"));

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

      private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
      {
         SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
         Console.WriteLine("SimConnect_OnRecvException: " + eException.ToString());

         Messages.Add(new Message("SimConnect : " + eException.ToString(), 2));
      }

      private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
      {
         uint iDefinition = data.dwDefineID;
         uint iRequest = data.dwRequestID;
         uint iObject = data.dwObjectID;

         //var variable = VariableRequests.First((v) => (uint)v.Definition == iDefinition && (uint)v.Request == iRequest);

         //if (variable is null) return;

         //variable.Value = (double)data.dwData[0];
         //variable.IsPending = false;

         //UpdateTestButtons(iRequest, variable.Value);
      }

      private void OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
      {
         PanelStates.Indicators.ReadSimData(data);
      }
      #endregion

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
         //Serial.SendData(TBMData.BuildData(VariableRequests));
      }

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

      private void SetRequestsPendingState(bool state = false)
      {
         //foreach (Variable oSimvarRequest in VariableRequests)
         //{
         //   oSimvarRequest.IsPending = state;
         //}
      }

      //public void UpdateTestButtons(uint index, double value)
      //{
      //   TestButtons[_testButtonIndex[index]] = value != 0;

      //   if (index > 6)
      //   {
      //      TestButtons["FLC"] = !TestButtons["ALT"] && !TestButtons["VS"] && TestButtons["AP"];
      //   } else if (index == 5)
      //   {
      //      TestButtons["BANK"] = value > 3;
      //   }
      //   OnPropertyChanged(nameof(HDG));
      //   OnPropertyChanged(nameof(APR));
      //   OnPropertyChanged(nameof(BC));
      //   OnPropertyChanged(nameof(NAV));
      //   OnPropertyChanged(nameof(FD));
      //   OnPropertyChanged(nameof(BANK));
      //   OnPropertyChanged(nameof(AP));
      //   OnPropertyChanged(nameof(YD));
      //   OnPropertyChanged(nameof(ALT));
      //   OnPropertyChanged(nameof(VS));
      //   OnPropertyChanged(nameof(FLC));
      //}

      public void SendDebugSerial()
      {
         //Timer.Start();
         try
         {
            Messages.Add(new Message("Debug Serial Mode"));

            TBMData tempData = new TBMData()
            {
               ALTLock = 0,
               Approach = 1,
               Backcourse = 1,
               FlightDirector = 0,
               HDGLock = 0,
               Master = 1,
               MaxBank = 50,
               NAV1 = 0,
               VertHold = 1,
               YawDamper = 1
            };
            if (!Serial.IsPortOpen)
            {
               Serial.InitPort(SelectedPort, CustomBaudRate);
               Serial.Open();
            }
            Serial.SendData(tempData);
         }
         catch (Exception e)
         {
            Messages.Add(new Message(e.Message, 1));
            throw;
         }
      }

      public void StartSerial()
      {
         Messages.Add(new Message("Starting serial comms.."));
         try
         {
            if (!Serial.Status.Equals(SelectedPort))
            {
               Serial.InitPort(SelectedPort, CustomBaudRate);
            }
            Serial.Open();
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
            Messages.Add(new Message("Stopping serial comms.."));
            Serial.Close();
         }
         catch (Exception e)
         {
            Messages.Add(new Message(e.Message, 1));
         }
      }

      public void RefreshPorts()
      {
         SelectedPort = null;
         OpenPortsList = new ObservableCollection<PortStatus>(SerialManager.GetOpenPorts());
      }

      public void BaudRateMenuChanged(object sender, RoutedEventArgs e)
      {
         if (sender is MenuItem mi)
         {
            CustomBaudRate = (int)mi.DataContext;
         }
      }

      public void AddMessage(Message msg)
      {
         Messages.Add(msg);
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

      //public Variable SelectedVariable
      //{
      //   get { return _selectedVariable; }
      //   set
      //   {
      //      _selectedVariable = value;
      //      OnPropertyChanged();
      //   }
      //}

      //public ObservableCollection<Variable> VariableRequests
      //{
      //   get { return _variableRequests; }
      //   set
      //   {
      //      _variableRequests = value;
      //      OnPropertyChanged();
      //   }
      //}

      public MessageController Messages
      {
         get { return _messages; }
         set
         {
            _messages = value;
            OnPropertyChanged();
         }
      }

      private readonly SolidColorBrush _activeColor = new SolidColorBrush(Color.FromRgb(100, 255, 155));
      private readonly SolidColorBrush _inactiveColor = new SolidColorBrush(Color.FromRgb(255, 155, 100));

      //public Dictionary<string, bool> TestButtons
      //{
      //   get { return _testButtons; }
      //   set
      //   {
      //      _testButtons = value;
      //      OnPropertyChanged();
      //      OnPropertyChanged(nameof(HDG));
      //      OnPropertyChanged(nameof(APR));
      //      OnPropertyChanged(nameof(BC));
      //      OnPropertyChanged(nameof(NAV));
      //      OnPropertyChanged(nameof(FD));
      //      OnPropertyChanged(nameof(BANK));
      //      OnPropertyChanged(nameof(AP));
      //      OnPropertyChanged(nameof(YD));
      //      OnPropertyChanged(nameof(ALT));
      //      OnPropertyChanged(nameof(VS));
      //      OnPropertyChanged(nameof(FLC));
      //   }
      //}

      public PortStatus SelectedPort
      {
         get { return _selectedPort; }
         set
         {
            _selectedPort = value;
            OnPropertyChanged();
         }
      }

      public ObservableCollection<PortStatus> OpenPortsList
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

      #region Button Mockup
      //public SolidColorBrush HDG
      //{
      //   //get => TestButtons["HDG"] ? _activeColor : _inactiveColor;
      //   get => TestButtons["HDG"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush APR
      //{
      //   get => TestButtons["APR"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush BC
      //{
      //   get => TestButtons["BC"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush NAV
      //{
      //   get => TestButtons["NAV"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush FD
      //{
      //   get => TestButtons["FD"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush BANK
      //{
      //   get => TestButtons["BANK"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush AP
      //{
      //   get => TestButtons["AP"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush YD
      //{
      //   get => TestButtons["YD"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush ALT
      //{
      //   get => TestButtons["ALT"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush VS
      //{
      //   get => TestButtons["VS"] ? _activeColor : _inactiveColor;
      //}

      //public SolidColorBrush FLC
      //{
      //   get => TestButtons["FLC"] ? _activeColor : _inactiveColor;
      //}
      #endregion
      #endregion
   }
}
