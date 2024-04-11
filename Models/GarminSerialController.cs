using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

using ArduinoSerialLibrary;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.Enums;
using TBMAutopilotDashboard.Models.Enums.Constants;
using TBMAutopilotDashboard.Models.Exceptions;
using TBMAutopilotDashboard.Models.State;

namespace TBMAutopilotDashboard.Models
{
   public class GarminSerialController : Model
   {
      #region Local Props
      private const int DefaultBaudRate = 115200;
      private readonly GarminPanelModel _panelStates;
      private readonly MessageController _messages;
      public delegate void ReceiveDataDelegate(byte[] data);
      private byte[] _rxBuffer = new byte[SerialCommands.ReceiveBufferSize];
      private byte[] _txBuffer = new byte[SerialCommands.TransmitBufferSize];
      private SerialPort serialPort;
      private Dictionary<GarminCommand, ReceiveDataDelegate> _receiveDelegates;
      private string _name = GarminController.GarminControllerName;
      private string _port = null;
      private bool _streamActive = false;
      public bool IsOpen => serialPort != null ? serialPort.IsOpen : false;
      #endregion

      #region Constructors
      public GarminSerialController(GarminPanelModel panelStates, MessageController messages)
      {
         //serialPort = new SerialPort(/*search for port info*/);
         //serialPort.DataReceived += SerialPort_DataReceived;
         _receiveDelegates = new Dictionary<GarminCommand, ReceiveDataDelegate>()
         {
            { GarminCommand.RX_STREAM, ReceiveStream },
            { GarminCommand.RX_HELLO, ReceivedHello  },
            { GarminCommand.RX_TX_ERROR, HandleError }
         };
         _panelStates = panelStates;
         _messages = messages;
      }
      #endregion

      #region Methods
      public void InitPort(string name)
      {
         if (serialPort?.IsOpen != true)
         {
            var names = SerialPort.GetPortNames();
            if (names.Contains(name))
            {
               serialPort = new SerialPort(name, DefaultBaudRate)
               {
                  DataBits = 8,
                  DtrEnable = false,
                  StopBits = StopBits.One,
                  Parity = Parity.None,
                  Handshake = Handshake.None
               };
               serialPort.DataReceived += SerialPort_DataReceived;
               Open();
               Name = name;
               OnPropertyChanged(nameof(BaudRate));
               OnPropertyChanged(nameof(DataBits));
               OnPropertyChanged(nameof(StopBit));
               OnPropertyChanged(nameof(ParityBit));
               OnPropertyChanged(nameof(HandshakeType));
            }
         }
      }

      public void Open()
      {
         if (serialPort?.IsOpen != true)
         {
            try
            {
               serialPort.Open();
               if (serialPort.IsOpen)
               {
                  SendCommand(GarminCommand.RX_HELLO);
               }
            }
            catch (Exception e)
            {
               _messages.Add(new Message(e.Message, Messagetype.ERROR));
            }
         }
      }

      public void Close()
      {
         if (serialPort?.IsOpen != true) return;
         serialPort.Close();
      }

      public void StartStream()
      {
         if (serialPort?.IsOpen != true) return;
         SendCommand(GarminCommand.TX_START_STREAM);
      }

      public void StopStream()
      {
         if (serialPort?.IsOpen != true) return;
         SendCommand(GarminCommand.TX_STOP_STREAM);
      }

      public void RegisterDelegate(ReceiveDataDelegate rxDelegate, GarminCommand cmd)
      {
         _receiveDelegates.Add(cmd, rxDelegate);
      }

      public bool RemoveDelegate(GarminCommand cmd)
      {
         return _receiveDelegates.Remove(cmd);
      }

      public void SendIndicators()
      {
         if (serialPort?.IsOpen != true) return;
         if (!_panelStates.Indicators.StateChanged) return;
         _txBuffer[0] = (byte)GarminCommand.TX_INDICATORS;
         //_panelStates.Indicators.SendIndicators().CopyTo(_txBuffer, 1);
         _panelStates.Indicators.SendIndicators(_txBuffer, 1);
         serialPort.Write(_txBuffer, 0, 3);
      }

      public void SendBacklight()
      {
         if (serialPort?.IsOpen != true) return;
         if (!_panelStates.Lighting.StateChanged) return;
         _txBuffer[0] = (byte)GarminCommand.TX_BACKLIGHT;
         //_panelStates.SendBacklight().CopyTo(_txBuffer, 1);
         //_panelStates.SendBacklight(_txBuffer, 1);
         _panelStates.Lighting.SendToDevice(_txBuffer);
         serialPort.Write(_txBuffer, 0, 3);
      }

      public void SendIndBrightness()
      {
         if (serialPort?.IsOpen != true) return;
         _txBuffer[0] = (byte)GarminCommand.TX_IND_BRIGHT;
         //_panelStates.SendIndBrightness().CopyTo(_txBuffer, 1);
         _panelStates.Lighting.SendIndBrightness(_txBuffer);
         serialPort.Write(_txBuffer, 0, 3);
      }

      public void SendMaxIndBrightness()
      {
         if (serialPort?.IsOpen != true) return;
         _txBuffer[0] = (byte)GarminCommand.TX_MAX_IND_BRIGHT;
         _panelStates.Lighting.SendMaxIndBrightness(_txBuffer);
         serialPort.Write(_txBuffer, 0, 2);
      }

      public void SendSettings()
      {

      }

      public void Pollbuttons()
      {
         if (serialPort?.IsOpen != true) return;
         SendCommand(GarminCommand.RX_BUTTONS);
      }

      public void PollEncoders()
      {
         if (serialPort?.IsOpen != true) return;
         SendCommand(GarminCommand.RX_ENC_POS);
      }

      public void PollSettings()
      {

      }

      private void SendCommand(GarminCommand cmd)
      {
         _txBuffer[0] = (byte)cmd;
         serialPort.Write(_txBuffer.ToArray(), 0, 1);
      }

      private void ReceiveStream(byte[] buffer)
      {
         var bufferList = buffer.ToList();
         _panelStates.Buttons.ReceiveData(bufferList.Take(SerialCommands.CommandSize[GarminCommand.RX_BUTTONS]).ToArray());
         _panelStates.Encoders.ReceiveData(bufferList.GetRange(SerialCommands.CommandSize[GarminCommand.RX_BUTTONS], SerialCommands.CommandSize[GarminCommand.RX_ENC_POS]).ToArray());
      }

      private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
      {
         serialPort.Read(_rxBuffer, 0, 1);
         if (Enum.IsDefined(typeof(GarminCommand), _rxBuffer[0]))
         {
            GarminCommand cmd = (GarminCommand)_rxBuffer[0];
            if (_receiveDelegates.ContainsKey(cmd))
            {
               int len = SerialCommands.CommandSize[cmd];
               if (len == 0)
               {
                  _receiveDelegates[cmd](null);
               }
               else
               {
                  serialPort.Read(_rxBuffer, 0, len);
                  _receiveDelegates[cmd](_rxBuffer.ToList().GetRange(0, len).ToArray());
               }
            }
         }
      }

      private void ReceivedHello(byte[] _)
      {
         if (_rxBuffer.Length > 0)
         {
            if (_rxBuffer[0] == (byte)GarminCommand.RX_HELLO)
            {
               _messages.Add(new Message("HELLO!!", Messagetype.INFO));
               return;
            }
         }
         throw new GarminSerialException("Connection Failure. Hello message not received.");
      }

      private void HandleError(byte[] _)
      {
         _messages.Add(new Message($"Controller error: {_rxBuffer[1]}", Messagetype.ERROR));
      }
      #endregion

      #region Full Props
      public string Name
      {
         get => _name;
         set
         {
            _name = value;
            OnPropertyChanged();
         }
      }

      public string Port
      {
         get => _port;
         set
         {
            _port = value;
            OnPropertyChanged();
         }
      }

      public bool StreamActive
      {
         get => _streamActive;
         set
         {
            _streamActive = value;
            OnPropertyChanged();
         }
      }

      public int? BaudRate => serialPort?.BaudRate;
      public int? DataBits => serialPort?.DataBits;
      public StopBits? StopBit => serialPort?.StopBits;
      public Parity? ParityBit => serialPort?.Parity;
      public Handshake? HandshakeType => serialPort?.Handshake;
      #endregion
   }
}
