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
      RX_ENC_DIR = 0x03,
      RX_ENC_POS = 0x04,
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

   public enum GarminCommandLength : int
   {
      BUTTONS     = 3,
      ENCODERS    = 5,
      ENCODER_DIR = 2,
      STATUS      = 3,
      STREAM      = BUTTONS + ENCODERS
   }

   public enum GarminCmdError : byte
   {
      UNKNOWN_ERROR   = 0,
      UNKNOWN_COMMAND = 1,
      MISSING_ARGS    = 2,
   }

   public enum GroupEventID
   {
      ENCODERS = 200,
      LIGHTING = 400,
   }

   public enum EncoderEventID
   {
      HDG_INC   = 2001,
      HDG_DEC   = 2002,
      HDG_SET   = 2000,
      ALT_INC   = 2101,
      ALT_DEC   = 2102,
      ALT_SET   = 2100,
      WHEEL_INC = 2201,
      WHEEL_DEC = 2202,
      WHEEL_SET = 2200,
      CRS1_INC  = 2301,
      CRS1_DEC  = 2302,
      CRS1_SET  = 2300,
      CRS2_INC  = 2401,
      CRS2_DEC  = 2402,
      CRS2_SET  = 2400,
   }

   public enum LightingEventID
   {
      LGHT_SET,
      LGHT_INC,
      LGHT_DEC,
      LGHT_TOGGLE
   }

   public enum LightingIndex
   {
      NONE,
      BEACON,
      STROBE,
      NAV_POS,
      PANEL,
      LANDING,
      TAXI,
      RECOGNITION,
      WING,
      LOGO,
      CABIN,
      PEDESTAL,
      GLARESHIELD,
      AMBIENT
   }

   public enum EncoderState
   {
      INCREMENT = 2,
      DECREMENT = 0,
      STILL     = 1,
   }

   public enum Messagetype
   {
      INFO  = 0,
      ERROR = 1,
      WARN  = 2,
   }

   public enum SIMCONNECT_GROUP_PRIORITY : uint
   {
      HIGHEST  = 1,
      STANDARD = 1900000000,
      DEFAULT  = 2000000000,
      LOWEST   = 4000000000
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
      HDG,
      ALT,
      WHEEL,
      CRS1,
      CRS2,
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
      INDICATOR_MESSAGE = 0x42,
      LIGHTING_MESSAGE = 0x24,
      SIMVAR_DATA_MSG = 0x2442,
   };

   public enum RequestID
   {
      INPUT_EVENTS = 0x4542,
      INDICATOR = 0x1243,
      SYSTEM_STATE = 0x9563,
      LIGHTING = 0x4863,
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
            { GarminCommand.RX_ENC_POS, 5 },
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

         public static readonly Dictionary<string, int> ToIndex = new Dictionary<string, int>
         {
            { "HDG",      0  },
            { "APR",      1  },
            { "BC",       2  },
            { "NAV",      3  },
            { "FD",       4  },
            { "BANK",     5  },
            { "AP",       6  },
            { "XFR",      7  },
            { "YD",       8  },
            { "ALT",      9  },
            { "VS",       10 },
            { "VNV",      11 },
            { "FLC",      12 },
            { "SPD",      13 },
            { "CRS1_ENC", 14 },
            { "CRS2_ENC", 15 },
            { "HDG_ENC",  16 },
            { "ALT_ENC",  17 },
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

         public static readonly Dictionary<PanelButton, string> ColorFromEnum = new Dictionary<PanelButton, string>
         {
            { PanelButton.HDG,      "HDG_Color"      },
            { PanelButton.APR,      "APR_Color"      },
            { PanelButton.BC,       "BC_Color"       },
            { PanelButton.NAV,      "NAV_Color"      },
            { PanelButton.FD,       "FD_Color"       },
            { PanelButton.BANK,     "BANK_Color"     },
            { PanelButton.AP,       "AP_Color"       },
            { PanelButton.XFR,      "XFR_Color"      },
            { PanelButton.YD,       "YD_Color"       },
            { PanelButton.ALT,      "ALT_Color"      },
            { PanelButton.VS,       "VS_Color"       },
            { PanelButton.VNV,      "VNV_Color"      },
            { PanelButton.FLC,      "FLC_Color"      },
            { PanelButton.SPD,      "SPD_Color"      },
            { PanelButton.CRS1_ENC, "CRS1_ENC_Color" },
            { PanelButton.CRS2_ENC, "CRS2_ENC_Color" },
            { PanelButton.HDG_ENC,  "HDG_ENC_Color"  },
            { PanelButton.ALT_ENC,  "ALT_ENC_Color"  },
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

         public static readonly Dictionary<string, int> ToIndex = new Dictionary<string, int>
         {
            { "HDG",   5  },
            { "APR",   4  },
            { "BC",    2  },
            { "NAV",   3  },
            { "FD",    10 },
            { "BANK",  14 },
            { "AP",    13 },
            { "XFR_R", 9  },
            { "XFR_L", 11 },
            { "YD",    12 },
            { "ALT",   7  },
            { "VS",    6  },
            { "VNV",   8  },
            { "FLC",   0  },
            { "SPD",   1  },
            { "ERROR", 15 },
         };

         public static readonly Dictionary<int, string> ToName = new Dictionary<int, string>
         {
            { 5,  "HDG"   },
            { 4,  "APR"   },
            { 2,  "BC"    },
            { 3,  "NAV"   },
            { 10, "FD"    },
            { 14, "BANK"  },
            { 13, "AP"    },
            { 9,  "XFR_R" },
            { 11, "XFR_L" },
            { 12, "YD"    },
            { 7,  "ALT"   },
            { 6,  "VS"    },
            { 8,  "VNV"   },
            { 0,  "FLC"   },
            { 1,  "SPD"   },
            { 15, "ERROR" },
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

         public static readonly Dictionary<PanelEncoder, int> ToIndex = new Dictionary<PanelEncoder, int>
         {
            { PanelEncoder.HDG, (int)PanelEncoder.HDG },
            { PanelEncoder.ALT, (int)PanelEncoder.ALT },
            { PanelEncoder.CRS1, (int)PanelEncoder.CRS1 },
            { PanelEncoder.CRS2, (int)PanelEncoder.CRS2 },
            { PanelEncoder.WHEEL, (int)PanelEncoder.WHEEL },
         };

         public static readonly Dictionary<PanelEncoder, string> ToPropName = new Dictionary<PanelEncoder, string>
         {
            { PanelEncoder.HDG, "HDG_ENC" },
            { PanelEncoder.ALT, "ALT_ENC" },
            { PanelEncoder.CRS1, "CRS1_ENC" },
            { PanelEncoder.CRS2, "CRS2_ENC" },
            { PanelEncoder.WHEEL, "WHEEL_ENC" },
         };
      }
   }
}
