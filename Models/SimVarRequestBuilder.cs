using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBMAutopilotDashboard.Models.SimConnectData;

namespace TBMAutopilotDashboard.Models
{
   public static class SimVarRequestBuilder
   {
      public static string BuildRequest(SimVar v, bool addGTSign = false) => $"({(addGTSign ? ">" : "")}A:{(v.UseTopVar ? $"1:" : "")}{v.Name}{(v.Index > 1 ? $":{v.Index}" : "")}, {v.Units})";
      public static string BuildRequest(SimVar a, SimVar b)
      {
         if (a is null) return null;
         return b is null ? BuildRequest(a) : $"2 {BuildRequest(a, true)} {BuildRequest(b)}";
      }

      public static string BuildEnvironmentVariableRequest(EnvVarModel envVar)
      {
         return $"(>E:{envVar.Name})";
      }
   }
}
