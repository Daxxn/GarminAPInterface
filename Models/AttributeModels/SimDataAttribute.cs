using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

namespace TBMAutopilotDashboard.Models.AttributeModels
{
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   sealed class SimDataAttribute : Attribute
   {
      private readonly string _simDataName;
      private readonly string _simUnits;
      private readonly SIMCONNECT_DATATYPE _type;

      public SimDataAttribute(string simDataName)
      {
         _simDataName = simDataName;
         _simUnits = "BOOL";
         _type = SIMCONNECT_DATATYPE.INT32;
      }

      public SimDataAttribute(string simDataName, string units)
      {
         _simDataName = simDataName;
         _simUnits = units;
      }

      public SimDataAttribute(string simDataName, string units, SIMCONNECT_DATATYPE type)
      {
         _simDataName = simDataName;
         _simUnits = units;
         _type = type;
      }

      public string SimDataName => _simDataName;
      public string SimUnits => _simUnits;
      public SIMCONNECT_DATATYPE Type => _type;
   }
}
