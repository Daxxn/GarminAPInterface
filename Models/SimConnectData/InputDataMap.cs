using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using JsonReaderLibrary;

using Microsoft.FlightSimulator.SimConnect;

using Newtonsoft.Json.Linq;

namespace TBMAutopilotDashboard.Models.SimConnectData
{
   internal struct InputModel
   {
      public string Name;
      public ulong Hash;
   }

   public class InputDataMap : IEnumerable
   {
      private static InputDataMap _instance = new InputDataMap();
      public ulong this[string key] { get => InputDir[key]; }
      #region Local Props
      private Dictionary<string, ulong> InputDir { get; set; } = new Dictionary<string, ulong>();
      public ICollection<string> Keys => InputDir.Keys;
      public ICollection<ulong> Values => InputDir.Values;
      public int Count => InputDir.Count;
      public bool IsReadOnly => true;
      #endregion

      #region Constructors
      private InputDataMap() { }
      #endregion

      #region Methods
      public bool OnStartup()
      {
         if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "InputData.json"))) return true;
         var data = JsonReader.OpenJsonFile<List<InputModel>>(Path.Combine(Environment.CurrentDirectory, "InputData.json"));
         foreach (var dataEntry in data)
         {
            InputDir.Add(dataEntry.Name, dataEntry.Hash);
         }
         return false;
      }

      public void OnRecvEnumeratedInputEvents(SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data)
      {
         List<InputModel> inputs = new List<InputModel>();
         foreach (SIMCONNECT_INPUT_EVENT_DESCRIPTOR evnt in data.rgData)
         {
            inputs.Add(new InputModel()
            {
               Name = evnt.Name,
               Hash = evnt.Hash,
            });
         }
         JsonReader.SaveJsonFile(Path.Combine(Environment.CurrentDirectory, "InputData.json"), inputs);
      }

      public bool Contains(KeyValuePair<string, ulong> item)
      {
         return InputDir.Contains(item);
      }

      public bool ContainsKey(string key)
      {
         return InputDir.ContainsKey(key);
      }

      public IEnumerator<KeyValuePair<string, ulong>> GetEnumerator()
      {
         return InputDir.GetEnumerator();
      }

      public bool TryGetValue(string key, out ulong value)
      {
         return InputDir.TryGetValue(key, out value);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
      #endregion

      #region Full Props
      public static InputDataMap Instance => _instance;
      #endregion
   }
}
