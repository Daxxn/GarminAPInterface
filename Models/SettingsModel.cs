using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVMLibrary;

using SettingsLibrary.Models;

namespace TBMAutopilotDashboard.Models
{
   public class SettingsModel : Model, ISettingsModel
   {
      public string SavePath { get; set; }
      public string LastSavePath { get; set; }

      private bool _autoConnectOnOpen = false;
      private string _msfsProcessName = "FlightSimulator.exe";

      public bool AutoConnectOnOpen
      {
         get => _autoConnectOnOpen;
         set
         {
            _autoConnectOnOpen = value;
            OnPropertyChanged();
         }
      }

      public string MsfsProcessName
      {
         get => _msfsProcessName;
         set
         {
            _msfsProcessName = value;
            OnPropertyChanged();
         }
      }
   }
}
