using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

namespace TBMAutopilotDashboard.Models.AttributeModels
{
   [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
   sealed class InputEventAttribute : Attribute
   {
      private readonly string _inputEventName;

      public InputEventAttribute(string inputEventName)
      {
         if (string.IsNullOrEmpty(inputEventName))
            throw new ArgumentNullException(nameof(inputEventName), "Attribute name cannot be null.");
         _inputEventName = inputEventName;
      }

      public string InputEventName => _inputEventName;
   }
}
