using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLE_Demo.Controller;
using BLE_Demo.Model;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BLE_Demo.View
{
    public partial class MainWindow : Window
    {
        private bool notificationsEnabled = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonReadData_Click(object sender, RoutedEventArgs e)
        {
            //TODO: make a prompt that tells user that data cannot be read before shutting down notifications.
            if (!notificationsEnabled) ;
        }

        private void buttonEnableNotifications_Click(object sender, RoutedEventArgs e)
        {
            notificationsEnabled = !notificationsEnabled;

            if (notificationsEnabled)
            {
                Title = "Demo - Notifications [On]";
                BLE_Utilities.executeOnNotification(Sensor.Keys, ButtonPressed);
            }

            else
            {
                Title = "Demo - Notifications [Off]";
            }


        }

        public void setAccelerometer(float x, float y, float z)
        {
            string xstr = "X: " + x.ToString("0.00") + " m/s";
            string ystr = "Y: " + y.ToString("0.00") + " m/s";
            string zstr = "Z: " + z.ToString("0.00") + " m/s";

            labelAccelerometer.Content = xstr + "   " + ystr + "   " + zstr;
        }

        public void setGyroscope(float v)
        {
            labelGyroscope.Content = v.ToString("0.0000") + " °/s";
        }

        public void setHumidity(float v)
        {
            labelHumidity.Content = v.ToString("0.0000") + " %";
        }

        public void setMagnometer(float v)
        {
            labelMagnometer.Content = v.ToString("0.0000") + " µT";
        }

        public void setPressure(float v)
        {
            labelPressure.Content = v.ToString("0.0000") + " hPa";
        }

        public void setTemperature(float v)
        {
            labelTemperature.Content = v.ToString("0.0000") + " °";
        }

        public void setButtons(byte s)
        {
            switch (s)
            {
                case 1: labelButtons.Content = "Left: Down     Right: Up"; break;
                case 2: labelButtons.Content = "Left: Up     Right: Down"; break;
                case 3: labelButtons.Content = "Left: Down     Right: Down"; break;
                default: labelButtons.Content = "Left: Up     Right: Up"; break;
            }
        }


        // EVENT HANDLERS FOR NOTIFICATIONS


        async void ButtonPressed(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] data = BLE_Utilities.getDataBytes(args);

            //My method is actually " setButton setButtons(data[0]) "
            //But if you want to change ANYTHING in the UI you have to wrap 
            //like I did below.
            await this.Dispatcher.BeginInvoke((Action)(()=> setButtons(data[0])));
        }
    }
}