using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

namespace TBMAutopilotDashboard.Models.AttributeModels
{
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   sealed class SimEventAttribute : Attribute
   {
      private readonly string _simEventName;
      private readonly SIMCONNECT_DATATYPE _type;
      public SimEventAttribute(string simEventName)
      {
         if (string.IsNullOrEmpty(simEventName))
            throw new ArgumentNullException(nameof(simEventName), "Attribute name cannot be null.");
         _simEventName = simEventName;
         _type = SIMCONNECT_DATATYPE.INT32;
      }
      public SimEventAttribute(string simEventName, SIMCONNECT_DATATYPE type)
      {
         if (string.IsNullOrEmpty(simEventName))
            throw new ArgumentNullException(nameof(simEventName), "Attribute name cannot be null.");
         _simEventName = simEventName;
         _type = type;
      }

      public string SimEventName => _simEventName;

      public SIMCONNECT_DATATYPE Type => _type;
   }
}
