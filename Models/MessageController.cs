using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVMLibrary;

namespace TBMAutopilotDashboard.Models
{
   public class MessageController : Model, ICollection<Message>
   {
      #region Local Props
      public Message this[int index]
      {
         get => messages[index];
         set
         {
            messages[index] = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Messages));
         }
      }
      private AsyncObservableCollection<Message> messages = new AsyncObservableCollection<Message>();
      #endregion

      #region Constructors
      public MessageController(int maxSize)
      {
         Count = maxSize;
      }

      public int Count { get; private set; }
      public bool IsReadOnly => false;

      public void Add(Message item)
      {
         messages.Insert(0, item);
         if (messages.Count >= Count)
         {
            messages.RemoveAt(Count - 1);
         }
         OnPropertyChanged(nameof(Messages));
      }

      public void Clear()
      {
         messages.Clear();
         OnPropertyChanged(nameof(Messages));
      }

      public bool Contains(Message item)
      {
         return messages.Contains(item);
      }

      public void CopyTo(Message[] array, int arrayIndex)
      {
         messages.CopyTo(array, arrayIndex);
      }

      public IEnumerator<Message> GetEnumerator()
      {
         return messages.GetEnumerator();
      }

      public bool Remove(Message item)
      {
         if (messages.Remove(item))
         {
            OnPropertyChanged(nameof(Messages));
            return true;
         }
         return false;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
        #endregion

        #region Methods

        #endregion

        #region Full Props
        public AsyncObservableCollection<Message> Messages => messages;
        #endregion
    }
}
