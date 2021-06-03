using MVVMLibraryFW;
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

namespace TBMAutopilotDashboard
{
   public class MainViewModel : ViewModel, IBaseSimConnectWrapper
   {
      #region - Fields & Properties
      public const string _name = "TBMAutopilot-Client";
      /// <summary>
      /// User-defined win32 event?
      /// </summary>
      #region SimConnectWrapper
      public const int WM_USER_SIMCONNECT = 0x0402;
      /// <summary>
      /// Window Handle (Still not sure what do...)
      /// </summary>
      private IntPtr Wnd = new IntPtr(0);
      #endregion

      /// SimConnect object
      private SimConnect SimConnect = null;
      private bool _connected;
      private bool _paused = true;

      private ObservableCollection<Variable> _variableRequests = new ObservableCollection<Variable>();
      private ObservableCollection<Message> _messages = new ObservableCollection<Message>();

      private Variable _selectedVariable;

      private PortStatus _selectedPort;
      private ObservableCollection<PortStatus> _ports;


      #region Command Bindings
      public BaseCommand ConnectCmd { get; private set; }
      public BaseCommand DisconnectCmd { get; private set; }
      public BaseCommand PauseCmd { get; private set; }
      public BaseCommand StartSerialCmd { get; private set; }
      public BaseCommand DebugStartSerialCmd { get; private set; }
      public BaseCommand StopSerialCmd { get; private set; }
      public BaseCommand RefreshPortsCmd { get; private set; }
      #endregion

      #region Constants
      private readonly SIMCONNECT_SIMOBJECT_TYPE _simObjectType = SIMCONNECT_SIMOBJECT_TYPE.USER;
      public uint _timerInterval = 100;
      #endregion

      public Timer Timer { get; private set; }

      #region Test Buttons
      public Dictionary<string, bool> _testButtons = new Dictionary<string, bool>
      {
         { "HDG", false },
         { "APR", false },
         { "BC", false },
         { "NAV", false },
         { "FD", false },
         { "BANK", false },
         { "AP", false },
         { "YD", false },
         { "ALT", false },
         { "VS", false },
         { "FLC", false }
      };
      public Dictionary<uint, string> _testButtonIndex = new Dictionary<uint, string>
      {
         { 0, "HDG" },
         { 1, "APR" },
         { 2, "BC" },
         { 3, "NAV" },
         { 4, "FD" },
         { 5, "BANK" },
         { 6, "AP" },
         { 7, "YD" },
         { 8, "ALT" },
         { 9, "VS" },
         { 10, "FLC" }
      };

      public SerialManager<TBMData> Serial { get; set; }
      private int _customBaudRate;
      #endregion

      #endregion

      #region - Constructors
      public MainViewModel()
      {
         VariableRequests = new ObservableCollection<Variable>(FileManager.ReadVariables());
         foreach (var request in VariableRequests)
         {
            request.IsPending = !RegisterToSimConnect(request);
         }

         ConnectCmd = new BaseCommand((o) => Connect());
         DisconnectCmd = new BaseCommand((o) => Disconnect());
         PauseCmd = new BaseCommand((o) => Pause());

         Timer = new Timer
         {
            AutoReset = true,
            Interval = _timerInterval
         };

         Timer.Elapsed += OnTick;

         StartSerialCmd = new BaseCommand(o => StartSerial());
         DebugStartSerialCmd = new BaseCommand(o => SendDebugSerial());
         StopSerialCmd = new BaseCommand(o => StopSerial());
         RefreshPortsCmd = new BaseCommand(o => RefreshPorts());

         Serial = new SerialManager<TBMData>(TBMData.ConvertData, CustomBaudRate, TBMData.DataSize);
         OpenPortsList = new ObservableCollection<PortStatus>(SerialManager.GetOpenPorts());
      }
      #endregion

      #region - Methods
      public void Pause()
      {
         IsPaused = !IsPaused;
      }

      public void Resume()
      {
         Timer.Start();
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

         Timer.Stop();

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
         }
         catch (COMException ex)
         {
            Messages.Add(new Message(ex.Message, 1));
         }
      }

      #region SimConnect Events
      private void OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
      {
         Messages.Add(new Message("Open Connection"));

         Connected = true;

         foreach (Variable oSimvarRequest in VariableRequests)
         {
            if (oSimvarRequest.IsPending)
            {
               oSimvarRequest.IsPending = !RegisterToSimConnect(oSimvarRequest);
            }
         }

         Timer.Start();
      }

      private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
      {
         Messages.Add(new Message("Closing Connection"));

         Disconnect();
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

         var variable = VariableRequests.First((v) => (uint)v.Definition == iDefinition && (uint)v.Request == iRequest);

         if (variable is null) return;

         variable.Value = (double)data.dwData[0];
         variable.IsPending = false;

         UpdateTestButtons(iRequest, variable.Value);
      }
      #endregion

      private void OnTick(object sender, ElapsedEventArgs e)
      {
         foreach (Variable variableRequest in VariableRequests)
         {
            if (!variableRequest.IsPending)
            {
               SimConnect?.RequestDataOnSimObjectType(variableRequest.Request, variableRequest.Definition, 0, _simObjectType);
               variableRequest.IsPending = true;
            }
         }

         Serial.SendData(TBMData.BuildData(VariableRequests));
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
         foreach (Variable oSimvarRequest in VariableRequests)
         {
            oSimvarRequest.IsPending = state;
         }
      }

      public void UpdateTestButtons(uint index, double value)
      {
         TestButtons[_testButtonIndex[index]] = value == 0 ? false : true;

         if (index > 6)
         {
            TestButtons["FLC"] = !TestButtons["ALT"] && !TestButtons["VS"] && TestButtons["AP"];
         } else if (index == 5)
         {
            TestButtons["BANK"] = value > 3 ? true : false;
         }
         OnPropertyChanged(nameof(HDG));
         OnPropertyChanged(nameof(APR));
         OnPropertyChanged(nameof(BC));
         OnPropertyChanged(nameof(NAV));
         OnPropertyChanged(nameof(FD));
         OnPropertyChanged(nameof(BANK));
         OnPropertyChanged(nameof(AP));
         OnPropertyChanged(nameof(YD));
         OnPropertyChanged(nameof(ALT));
         OnPropertyChanged(nameof(VS));
         OnPropertyChanged(nameof(FLC));
      }

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
      #endregion

      #region - Full Properties 
      public bool Connected
      {
         get { return _connected; }
         set
         {
            _connected = value;
            IsPaused = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ConnectedColor));
         }
      }
      public bool IsPaused
      {
         get { return _paused; }
         set
         {
            _paused = value;
            Timer.Enabled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ConnectedColor));
         }
      }

      public SolidColorBrush ConnectedColor
      {
         get
         {
            if (Connected)
            {
               if (!IsPaused)
               {
                  return new SolidColorBrush(Color.FromRgb(255, 255, 100));
               }
               else
               {
                  return new SolidColorBrush(Color.FromRgb(100, 255, 100));
               }
            }
            else
            {
               return new SolidColorBrush(Color.FromRgb(255, 100, 100));
            }
         }
      }

      public Variable SelectedVariable
      {
         get { return _selectedVariable; }
         set
         {
            _selectedVariable = value;
            OnPropertyChanged();
         }
      }

      public ObservableCollection<Variable> VariableRequests
      {
         get { return _variableRequests; }
         set
         {
            _variableRequests = value;
            OnPropertyChanged();
         }
      }

      public ObservableCollection<Message> Messages
      {
         get { return _messages; }
         set
         {
            _messages = value;
            if (_messages.Count > 10)
            {
               _messages.RemoveAt(0);
            }
            OnPropertyChanged();
         }
      }


      private readonly SolidColorBrush _activeColor = new SolidColorBrush(Color.FromRgb(100, 255, 155));
      private readonly SolidColorBrush _inactiveColor = new SolidColorBrush(Color.FromRgb(255, 155, 100));

      public Dictionary<string, bool> TestButtons
      {
         get { return _testButtons; }
         set
         {
            _testButtons = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HDG));
            OnPropertyChanged(nameof(APR));
            OnPropertyChanged(nameof(BC));
            OnPropertyChanged(nameof(NAV));
            OnPropertyChanged(nameof(FD));
            OnPropertyChanged(nameof(BANK));
            OnPropertyChanged(nameof(AP));
            OnPropertyChanged(nameof(YD));
            OnPropertyChanged(nameof(ALT));
            OnPropertyChanged(nameof(VS));
            OnPropertyChanged(nameof(FLC));
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

      #region Button Mockup
      public SolidColorBrush HDG
      {
         get => TestButtons["HDG"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush APR
      {
         get => TestButtons["APR"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush BC
      {
         get => TestButtons["BC"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush NAV
      {
         get => TestButtons["NAV"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush FD
      {
         get => TestButtons["FD"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush BANK
      {
         get => TestButtons["BANK"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush AP
      {
         get => TestButtons["AP"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush YD
      {
         get => TestButtons["YD"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush ALT
      {
         get => TestButtons["ALT"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush VS
      {
         get => TestButtons["VS"] ? _activeColor : _inactiveColor;
      }

      public SolidColorBrush FLC
      {
         get => TestButtons["FLC"] ? _activeColor : _inactiveColor;
      }
      #endregion
      #endregion
   }
}
