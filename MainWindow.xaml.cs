using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace TBMAutopilotDashboard
{
   interface IBaseSimConnectWrapper
   {
      int GetUserSimConnectWinEvent();
      void ReceiveSimConnectMessage();
      void SetWindowHandle(IntPtr _hWnd);
      void Disconnect();
   }
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private readonly int[] typicalBaudRates = new int[]
      {
         4800, 9600, 19200, 38400, 57600,
         115200, 230400, 460800, 921600
      };

      private MainViewModel VM { get; set; }
      public MainWindow()
      {
         VM = new MainViewModel();
         DataContext = VM;
         InitializeComponent();

         foreach (var typ in typicalBaudRates)
         {
            var tempMenu = new MenuItem()
            {
               Header = typ,
               DataContext = typ,
            };
            tempMenu.Click += VM.BaudRateMenuChanged;
            BaudRateMenu.Items.Add(tempMenu);
         }
      }

      protected HwndSource GetHWinSource()
      {
         return PresentationSource.FromVisual(this) as HwndSource;
      }

      private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
      {
         if (this.DataContext is IBaseSimConnectWrapper oBaseSimConnectWrapper)
         {
            try
            {
               if (iMsg == oBaseSimConnectWrapper.GetUserSimConnectWinEvent())
               {
                  oBaseSimConnectWrapper.ReceiveSimConnectMessage();
               }
            }
            catch
            {
               oBaseSimConnectWrapper.Disconnect();
            }
         }

         return IntPtr.Zero;
      }

      protected override void OnSourceInitialized(EventArgs e)
      {
         base.OnSourceInitialized(e);
         GetHWinSource().AddHook(WndProc);
         if (this.DataContext is IBaseSimConnectWrapper oBaseSimConnectWrapper)
         {
            oBaseSimConnectWrapper.SetWindowHandle(GetHWinSource().Handle);
         }
      }
   }
}
