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

namespace BLE_Demo
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
            // TODO: Call the method that reads data from the device.
        }

        private void buttonEnableNotifications_Click(object sender, RoutedEventArgs e)
        {
            notificationsEnabled = !notificationsEnabled;

            if (notificationsEnabled)
            {
                Title = "Demo - Notifications [On]";
            }

            else
            {
                Title = "Demo - Notifications [Off]";
            }

            // TODO: Call the method that enables notifications from the device.
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
    }
}