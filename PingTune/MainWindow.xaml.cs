using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFSpark;
using System.Security.Principal;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;

namespace PingTune
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, EthernetAdapter> _adaptors;
        public EthernetAdapter _selectedAdaptor;
        GlobalSystemState _sys_state;
        int optMtu = PingTune.MTUOptimizer.getOptimalMTU();

        private IToolTipLibrary _tt;

        protected List<KeyValuePair<string, string>> GetAdapterList()
        {
            var ret = new List<KeyValuePair<string,string>>();
            var not_connected = new List<KeyValuePair<string,string>>();
            foreach (var eth in _adaptors.Values)
            {              
                if (eth.Connected == true)
                {
                    string desc = eth.Description + " (" + eth.Address +")";

                    ret.Add(new KeyValuePair<string, string>(eth.Id, desc) );
                }
                else
                {
                    string desc = eth.Description + " (disconnected)";
                    not_connected.Add(new KeyValuePair<string, string>(eth.Id, desc));
                }
            }
            ret.AddRange(not_connected); // Make sure connected adapters are in the list before unconnected.
            return ret;
        }


        IToolTipLibrary getToolTipsForLanguage( string lang )
        {
            switch (lang)
            {
                case "unittest": // stub language for unit testing.
                    return new ToolTipLibraryEnglish();
                default: // process ISO3 codes for real languages.
                    return new ToolTipLibraryEnglish();
            }
        }

        private void genericButtonPress( object sender, RoutedEventArgs e)
        {
            var origin_name = ((Button)sender).Name;
            var event_type = e.RoutedEvent.Name;
            string msg = "unhandled event: " + event_type + " origin: " + origin_name;
            var s = _selectedAdaptor; // for sake of brevity.
            var st = _sys_state;

            switch (origin_name)
            {
                case "MtuOptButton":
                    s.MTU = optMtu;
                    MtuValueBox.Text = s.MTU.ToString();
                    break;
                case "buttonSaveExit":
                    st.Commit();

                    foreach (var a in _adaptors.Values)
                        a.Commit();
                    MessageBox.Show("Done writing changes to the registry.\nThese will take effect after the next reboot.");
                    this.Close();
                    break;
                case "buttonCancelQuit":
                    this.Close();
                    break; // vaguely superfluous!
                default:
                    MessageBox.Show(msg);
                    break;
            }
            return;
        }

        // fuck XAML anyway :-)
        void getToolTip( object sender, RoutedEventArgs e)
        {
            var origin_name = ((FrameworkElement)sender).Name;
            ((FrameworkElement)sender).ToolTip = _tt.getToolTip( origin_name );

        }

        private void genericToggle( object sender, RoutedEventArgs e)
        {
            var event_type = e.RoutedEvent.Name;
            var origin_name = ((ToggleSwitch)sender).Name;
            string msg = "unhandled event: " + event_type + " origin: " + origin_name;

            // two aliases for the sake of brevity below
            var s = _selectedAdaptor; 
            var st = _sys_state;
            bool t; // dummy variable to trick the ternary operator.

            switch (origin_name)
            {   // Toggle NIC level flow control off/on
                case "toggleFlowControl":
                    t = (event_type == "Checked") ? s.FlowControl.setOptimized() : s.FlowControl.setDefault();
                    break;
                case "toggleInterruptModeration":
                    t = (event_type == "Checked") ? s.InterruptModeration.setOptimized() : s.InterruptModeration.setDefault();
                    break;
                case "toggleNagle":
                    t = (event_type == "Checked") ? s.Nagling.setOptimized() : s.Nagling.setDefault();
                    break;
                case "toggleAckDelay":
                    t = (event_type == "Checked") ? s.TcpDelayedAckTicks.setOptimized() : s.TcpDelayedAckTicks.setDefault();
                    break;
                case "toggleMaxAckFreq":
                    t = (event_type == "Checked") ? s.TcpAckFrequency.setOptimized() : s.TcpAckFrequency.setDefault();
                    break;
                case "toggleMaxTxBuf":
                    t = (event_type == "Checked") ? s.TxBuffers.setOptimized() : s.TxBuffers.setDefault();
                    break;
                case "toggleMaxRxBuf":
                    t = (event_type == "Checked") ? s.RxBuffers.setOptimized() : s.RxBuffers.setDefault();
                    break;
                case "toggleTcpOffload":
                    t = (event_type == "Checked") ? s.TcpChecksumOffload.setOptimized() : s.TcpChecksumOffload.setDefault();
                    break;
                case "toggleUdpOffload":
                    t = (event_type == "Checked") ? s.UdpChecksumOffload.setOptimized() : s.UdpChecksumOffload.setDefault();
                    break;
                case "toggleHeadDataSplit":
                    t = (event_type == "Checked") ? s.HeaderDataSplit.setOptimized() : s.HeaderDataSplit.setDefault();
                    break;          
                case "toggleRssDisable":
                    t = (event_type == "Checked") ? s.RSS.setOptimized() : s.RSS.setDefault();
                    break;
                    // TODOs
                case "toggleThrottling":
                    t = (event_type == "Checked") ? st.netThrottling.setOptimized() : st.netThrottling.setDefault();
                    break;
                case "toggleMaxForePriority":
                    t = (event_type == "Checked") ? st.backgroundReservedCpuPct.setOptimized() : st.backgroundReservedCpuPct.setDefault();
                    break;
                    // END TODO
                case "toggleLargeSendOffload":
                    t = (event_type == "Checked") ? s.LargeSendOffload.setOptimized() : s.LargeSendOffload.setDefault();
                    break;
                default:
                    MessageBox.Show(msg);
                    break;
            }
        }

        private void onSelectAdapter(object sender, SelectionChangedEventArgs e)
        {
            _selectedAdaptor = _adaptors[AdapterListBox.SelectedValue.ToString()];
            MtuValueBox.Text = _selectedAdaptor.MTU.ToString();
            toggleFlowControl.IsChecked = _selectedAdaptor.FlowControl.isOptimized();
            toggleMaxAckFreq.IsChecked = _selectedAdaptor.TcpAckFrequency.isOptimized();
            toggleMaxRxBuf.IsChecked = _selectedAdaptor.RxBuffers.isOptimized();
            toggleMaxTxBuf.IsChecked = _selectedAdaptor.TxBuffers.isOptimized();
            toggleNagle.IsChecked = _selectedAdaptor.Nagling.isOptimized();
            toggleInterruptModeration.IsChecked = _selectedAdaptor.InterruptModeration.isOptimized();
            toggleUdpOffload.IsChecked = _selectedAdaptor.UdpChecksumOffload.isOptimized();
            toggleTcpOffload.IsChecked = _selectedAdaptor.TcpChecksumOffload.isOptimized();
            toggleRssDisable.IsChecked = _selectedAdaptor.RSS.isOptimized();
            toggleHeadDataSplit.IsChecked = _selectedAdaptor.HeaderDataSplit.isOptimized();
            toggleAckDelay.IsChecked = _selectedAdaptor.TcpDelayedAckTicks.isOptimized();
            toggleLargeSendOffload.IsChecked = _selectedAdaptor.LargeSendOffload.isOptimized();
        }

        public MainWindow()
        {            
            _adaptors = PingTune.NetState.getAdapterState();
            _sys_state = new PingTune.GlobalSystemState();

            string systemLanguage = System.Globalization.CultureInfo.CurrentCulture.ThreeLetterISOLanguageName.ToString();
            _tt = getToolTipsForLanguage(systemLanguage);

            EventManager.RegisterClassHandler(typeof(FrameworkElement), FrameworkElement.ToolTipOpeningEvent, new ToolTipEventHandler(getToolTip));

            InitializeComponent();
            DataContext = this;
            AdapterListBox.ItemsSource = GetAdapterList();
            AdapterListBox.DisplayMemberPath = "Value";
            AdapterListBox.SelectedValuePath = "Key";
            AdapterListBox.SelectedIndex = 0;
            _selectedAdaptor = _adaptors[AdapterListBox.SelectedValue.ToString()];

            MtuOptValueBox.Text = optMtu.ToString();

            toggleThrottling.IsChecked = _sys_state.netThrottling.isOptimized();
            toggleMaxForePriority.IsChecked = _sys_state.backgroundReservedCpuPct.isOptimized();
        }
    }
}
