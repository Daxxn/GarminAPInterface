using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBMAutopilotDashboard.Models.Enums
{
   public enum GarminTransmitCommand : byte
   {
      TX_INDICATORS = 6,
      TX_BACKLIGHT = 8,
      TX_IND_BRIGHT = 9,
      TX_OPTIONS = 0xF1,
      TX_MAX_IND_BRIGHT = 10,
   }

   public enum GarminReceiveCommand : byte
   {
      RX_BUTTONS = 0x02,
      RX_ENCODERS = 0x04,
      RX_STATUS = 0xF2,
   }

   public enum GarminCommand : byte
   {
      RX_BUTTONS = 0x02,
      RX_ENCODERS = 0x04,
      RX_STREAM = 0x05,
      RX_STATUS = 0xF2,
      TX_INDICATORS = 0x06,
      TX_BACKLIGHT = 0x08,
      TX_IND_BRIGHT = 0x09,
      TX_MAX_IND_BRIGHT = 0x0A,
      TX_START_STREAM = 0x11,
      TX_STOP_STREAM = 0x12,
      TX_OPTIONS = 0xF1,
      RX_TX_ERROR = 0xFF,
      RX_HELLO = 0xFE
   }

   public enum CarminCmdError : byte
   {
      UNKNOWN_ERROR = 0,
      UNKNOWN_COMMAND = 1,
      MISSING_ARGS = 2,
   }

   public enum EncoderGroupEventID
   {
      DEFAULT = 200
   }

   public enum EncoderEventID
   {
      HDG_INC   = 2000,
      HDG_DEC   = 2001,
      ALT_INC   = 2002,
      ALT_DEC   = 2003,
      WHEEL_INC = 2004,
      WHEEL_DEC = 2005,
      CRS1_INC  = 2006,
      CRS1_DEC  = 2007,
      CRS2_INC  = 2008,
      CRS2_DEC  = 2009
   }

   public enum EncoderState
   {
      INCREMENT = 1,
      DECREMENT = -1,
      STILL     = 0,
   }

   public enum Messagetype
   {
      INFO  = 0,
      ERROR = 1,
      WARN  = 2,
   }

   public enum SIMCONNECT_GROUP_PRIORITY : uint
   {
      HIGHEST = 1,
      STANDARD = 1900000000,
      DEFAULT = 2000000000,
      LOWEST = 4000000000
   }

   public enum PanelButton
   {
      HDG,
      APR,
      BC,
      NAV,
      FD,
      BANK,
      AP,
      XFR,
      YD,
      ALT,
      VS,
      VNV,
      FLC,
      SPD,
      CRS1_ENC,
      CRS2_ENC,
      HDG_ENC,
      ALT_ENC
   }

   public enum PanelIndicator
   {
      FLC   = 0,
      SPD   = 1,
      BC    = 2,
      NAV   = 3,
      APR   = 4,
      HDG   = 5,
      VS    = 6,
      ALT   = 7,
      VNV   = 8,
      XFR_R = 9,
      FD    = 10,
      XFR_L = 11,
      YD    = 12,
      AP    = 13,
      BANK  = 14,
      ERROR = 15,
   }

   public enum PanelEncoder
   {
      CRS1,
      CRS2,
      HDG,
      ALT,
      WHEEL
   }

   public enum SimTypes
   {
      BOOL,
      INT,
      UINT,
      BYTE,
      FLOAT,
      ENUM,
      STRUCT,
      STRING,
   }

   public enum SimSystemEvent : uint
   {
      EVERY_SEC = 0x1FFFFF00,
      EVERY_4_SEC = 0x1FFFFF01,
      EVERY_6HZ = 0x1FFFFF02,
      EVERY_FRAME = 0x1FFFFF03,
      AIRCRAFT_LOADED = 0x1FFFFF04,
      CRASHED = 0x1FFFFF05,
      CRASH_RESET = 0x1FFFFF06,
      FLIGHT_LOADED = 0x1FFFFF07,
      FLIGHT_SAVED = 0x1FFFFF08,
      FLIGHT_PLAN_ACTIVATED = 0x1FFFFF09,
      FLIGHT_PLAN_DEACTIVATED = 0x1FFFFF0A,
      OBJECT_ADDED = 0x1FFFFF0B,
      OBJECT_REMOVED = 0x1FFFFF0C,
      PAUSE = 0x1FFFFF0D,
      PAUSE_EX1 = 0x1FFFFF0E,
      PAUSED = 0x1FFFFF0F,
      PAUSE_FRAME = 0x1FFFFF10,
      POSITION_CHANGED = 0x1FFFFF11,
      SIM = 0x1FFFFF12,
      SIM_START = 0x1FFFFF13,
      SIM_STOP = 0x1FFFFF14,
      SOUND = 0x1FFFFF15,
      UNPAUSED = 0x1FFFFF16,
      VIEW_CHANGED = 0x1FFFFF17,
   }

   public enum StructDefinition
   {
      INDICATOR_MESSAGE = 0x42
   };

   public enum RequestID
   {
      INPUT_EVENTS = 0x4542,
      INDICATOR = 0x1243,
      SYSTEM_STATE = 0x9563,
   }

   namespace Constants
   {
      public static class GarminController
      {
         public const string GarminControllerName = "Garmin AP ComPort";
      }

      public static class SerialCommands
      {
         public const int ReceiveBufferSize = 56;
         public const int TransmitBufferSize = 56;
         public static readonly Dictionary<GarminCommand, int> CommandSize = new Dictionary<GarminCommand, int>
         {
            { GarminCommand.RX_BUTTONS, 3 },
            { GarminCommand.RX_ENCODERS, 5 },
            { GarminCommand.RX_STREAM, 8 },
            { GarminCommand.RX_STATUS, 3 },
            { GarminCommand.TX_INDICATORS, 2 },
            { GarminCommand.TX_BACKLIGHT, 2 },
            { GarminCommand.TX_IND_BRIGHT, 1 },
            { GarminCommand.TX_MAX_IND_BRIGHT, 1 },
            { GarminCommand.TX_START_STREAM, 0 },
            { GarminCommand.TX_STOP_STREAM, 0 },
            { GarminCommand.TX_OPTIONS, 1 },
            { GarminCommand.RX_TX_ERROR, 0 },
            { GarminCommand.RX_HELLO, 0 },
         };
      }

      public static class SimSysEventName
      {
         public const string oneSecond = "1sec";
         public const string fourSeconds = "4sec";
         public const string sixHz = "6Hz";
         public const string aircraftLoaded = "AircraftLoaded";
         public const string crashed = "Crashed";
         public const string crashReset = "CrashReset";
         public const string flightLoaded = "FlightLoaded";
         public const string flightSaved = "FlightSaved";
         public const string flightPlanActivated = "FlightPlanActivated";
         public const string flightPlanDeactivated = "FlightPlanDeactivated";
         public const string frame = "Frame";
         public const string objectAdded = "ObjectAdded";
         public const string objectRemoved = "ObjectRemoved";
         public const string pause = "Pause";
         public const string pauseEX1 = "Pause_EX1";
         public const string paused = "Paused";
         public const string pauseFrame = "PauseFrame";
         public const string positionChanged = "PositionChanged";
         public const string sim = "Sim";
         public const string simStart = "SimStart";
         public const string simStop = "SimStop";
         public const string sound = "Sound";
         public const string unpaused = "Unpaused";
         public const string ViewChanged = "View";
      }

      public static class PanelButtonNames
      {
         public const int ButtonCount = 18;
         public static readonly Dictionary<string, PanelButton> ToEnum = new Dictionary<string, PanelButton>
         {
            { "HDG",      PanelButton.HDG      },
            { "APR",      PanelButton.APR      },
            { "BC",       PanelButton.BC       },
            { "NAV",      PanelButton.NAV      },
            { "FD",       PanelButton.FD       },
            { "BANK",     PanelButton.BANK     },
            { "AP",       PanelButton.AP       },
            { "XFR",      PanelButton.XFR      },
            { "YD",       PanelButton.YD       },
            { "ALT",      PanelButton.ALT      },
            { "VS",       PanelButton.VS       },
            { "VNV",      PanelButton.VNV      },
            { "FLC",      PanelButton.FLC      },
            { "SPD",      PanelButton.SPD      },
            { "CRS1_ENC", PanelButton.CRS1_ENC },
            { "CRS2_ENC", PanelButton.CRS2_ENC },
            { "HDG_ENC",  PanelButton.HDG_ENC  },
            { "ALT_ENC",  PanelButton.ALT_ENC  },
         };

         public static readonly Dictionary<PanelButton, string> FromEnum = new Dictionary<PanelButton, string>
         {
            { PanelButton.HDG,      "HDG"      },
            { PanelButton.APR,      "APR"      },
            { PanelButton.BC,       "BC"       },
            { PanelButton.NAV,      "NAV"      },
            { PanelButton.FD,       "FD"       },
            { PanelButton.BANK,     "BANK"     },
            { PanelButton.AP,       "AP"       },
            { PanelButton.XFR,      "XFR"      },
            { PanelButton.YD,       "YD"       },
            { PanelButton.ALT,      "ALT"      },
            { PanelButton.VS,       "VS"       },
            { PanelButton.VNV,      "VNV"      },
            { PanelButton.FLC,      "FLC"      },
            { PanelButton.SPD,      "SPD"      },
            { PanelButton.CRS1_ENC, "CRS1_ENC" },
            { PanelButton.CRS2_ENC, "CRS2_ENC" },
            { PanelButton.HDG_ENC,  "HDG_ENC"  },
            { PanelButton.ALT_ENC,  "ALT_ENC"  },
         };

         public static readonly Dictionary<string, string> FromButton = new Dictionary<string, string>
         {
            { "HDG_BTN",      "HDG"      },
            { "APR_BTN",      "APR"      },
            { "BC_BTN",       "BC"       },
            { "NAV_BTN",      "NAV"      },
            { "FD_BTN",       "FD"       },
            { "BANK_BTN",     "BANK"     },
            { "AP_BTN",       "AP"       },
            { "XFR_BTN",      "XFR"      },
            { "YD_BTN",       "YD"       },
            { "ALT_BTN",      "ALT"      },
            { "VS_BTN",       "VS"       },
            { "VNV_BTN",      "VNV"      },
            { "FLC_BTN",      "FLC"      },
            { "SPD_BTN",      "SPD"      },
            { "CRS1_ENC_BTN", "CRS1_ENC" },
            { "CRS2_ENC_BTN", "CRS2_ENC" },
            { "HDG_ENC_BTN",  "HDG_ENC"  },
            { "ALT_ENC_BTN",  "ALT_ENC"  },
         };
      }

      public static class PanelIndicatorNames
      {
         public const int IndicatorCount = 16;
         public static readonly Dictionary<string, PanelIndicator> ToEnum = new Dictionary<string, PanelIndicator>
         {
            { "HDG",   PanelIndicator.HDG   },
            { "APR",   PanelIndicator.APR   },
            { "BC",    PanelIndicator.BC    },
            { "NAV",   PanelIndicator.NAV   },
            { "FD",    PanelIndicator.FD    },
            { "BANK",  PanelIndicator.BANK  },
            { "AP",    PanelIndicator.AP    },
            { "XFR_R", PanelIndicator.XFR_R },
            { "XFR_L", PanelIndicator.XFR_L },
            { "YD",    PanelIndicator.YD    },
            { "ALT",   PanelIndicator.ALT   },
            { "VS",    PanelIndicator.VS    },
            { "VNV",   PanelIndicator.VNV   },
            { "FLC",   PanelIndicator.FLC   },
            { "SPD",   PanelIndicator.SPD   },
            { "ERROR", PanelIndicator.ERROR },
         };

         public static readonly Dictionary<PanelIndicator, string> FromEnum = new Dictionary<PanelIndicator, string>
         {
            { PanelIndicator.HDG,   "HDG"   },
            { PanelIndicator.APR,   "APR"   },
            { PanelIndicator.BC,    "BC"    },
            { PanelIndicator.NAV,   "NAV"   },
            { PanelIndicator.FD,    "FD"    },
            { PanelIndicator.BANK,  "BANK"  },
            { PanelIndicator.AP,    "AP"    },
            { PanelIndicator.XFR_R, "XFR_R" },
            { PanelIndicator.XFR_L, "XFR_L" },
            { PanelIndicator.YD,    "YD"    },
            { PanelIndicator.ALT,   "ALT"   },
            { PanelIndicator.VS,    "VS"    },
            { PanelIndicator.VNV,   "VNV"   },
            { PanelIndicator.FLC,   "FLC"   },
            { PanelIndicator.SPD,   "SPD"   },
            { PanelIndicator.ERROR, "ERROR" },
         };
      }

      public static class PanelEncoderNames
      {
         public const int EncoderCount = 5;
         public static readonly Dictionary<string, PanelEncoder> ToEnum = new Dictionary<string, PanelEncoder>
         {
            { "HDG", PanelEncoder.HDG     },
            { "ALT", PanelEncoder.ALT     },
            { "CRS1", PanelEncoder.CRS1   },
            { "CRS2", PanelEncoder.CRS2   },
            { "WHEEL", PanelEncoder.WHEEL },
         };
      }
   }
}
