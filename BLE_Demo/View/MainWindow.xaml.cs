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
using BLE_Demo.Model;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Timers;

namespace BLE_Demo.View
{
    public partial class MainWindow : Window
    {
        private bool notificationsEnabled = false;
        private Timer timer;
        public MainWindow()
        {
            InitializeComponent();
            EnableSensors();
            timer = new Timer(5000);
            timer.Elapsed += refreshNotifications;
            timer.Start();
        }

        private async void refreshNotifications(object sender, ElapsedEventArgs e)
        {
            if (notificationsEnabled)
            {
                await BLE_Utilities.executeOnNotification(Sensor.Keys, notifyButton);
                await BLE_Utilities.executeOnNotification(Sensor.Accelerometer, NotifyAccelerometer);
                await BLE_Utilities.executeOnNotification(Sensor.Humidity, NotifyHumidity);
                await BLE_Utilities.executeOnNotification(Sensor.Gyroscope, NotifyGyroscope);
                await BLE_Utilities.executeOnNotification(Sensor.Magnetometer, NotifyMagnetometer);
                await BLE_Utilities.executeOnNotification(Sensor.Temperature, NotifyTemperature);

            }
        }

        private async void buttonReadData_Click(object sender, RoutedEventArgs e)
        {
            await EnableSensors();

            //TODO: make a prompt that tells user that data cannot be read before shutting down notifications.
            await ReadTemperature();
            await ReadAccelerometer();
            await ReadGyroscope();
            await ReadHumidity();
            await ReadMagnetometer();

        }

        private async Task EnableSensors()
        {
            await BLE_Utilities.EnableSensor(Sensor.Accelerometer);
            await BLE_Utilities.EnableSensor(Sensor.Gyroscope);
            await BLE_Utilities.EnableSensor(Sensor.Temperature);
            await BLE_Utilities.EnableSensor(Sensor.Magnetometer);
            await BLE_Utilities.EnableSensor(Sensor.Pressure);
            await BLE_Utilities.EnableSensor(Sensor.Humidity);
        }

        private async Task DisableSensors()
        {
            await BLE_Utilities.DisableSensor(Sensor.Accelerometer);
            await BLE_Utilities.DisableSensor(Sensor.Gyroscope);
            await BLE_Utilities.DisableSensor(Sensor.Temperature);
            await BLE_Utilities.DisableSensor(Sensor.Magnetometer);
            await BLE_Utilities.DisableSensor(Sensor.Pressure);
            await BLE_Utilities.DisableSensor(Sensor.Humidity);
        }

        private async void buttonEnableNotifications_Click(object sender, RoutedEventArgs e)
        {
            notificationsEnabled = !notificationsEnabled;
            if (notificationsEnabled)
            {
                buttonEnableNotifications.Content = "Disable Notifications";
                Title = "Demo - Notifications [On]";

                await BLE_Utilities.executeOnNotification(Sensor.Keys, notifyButton);
                await BLE_Utilities.executeOnNotification(Sensor.Accelerometer, NotifyAccelerometer);
                await BLE_Utilities.executeOnNotification(Sensor.Humidity, NotifyHumidity);
                await BLE_Utilities.executeOnNotification(Sensor.Gyroscope, NotifyGyroscope);
                await BLE_Utilities.executeOnNotification(Sensor.Magnetometer, NotifyMagnetometer);
                await BLE_Utilities.executeOnNotification(Sensor.Temperature, NotifyTemperature);
                
            }
            else
            {
                Title = "Demo - Notifications [Off]";
                buttonEnableNotifications.Content = "Enable Notifications";

                await BLE_Utilities.dispose(Sensor.Keys);
                await BLE_Utilities.dispose(Sensor.Accelerometer);
                await BLE_Utilities.dispose(Sensor.Humidity);
                await BLE_Utilities.dispose(Sensor.Gyroscope);
                await BLE_Utilities.dispose(Sensor.Magnetometer);
                await BLE_Utilities.dispose(Sensor.Temperature);
            }
        }

        public void setAccelerometer(float x, float y, float z)
        {
            string xstr = "X: " + x.ToString("0.00") + " m/s";
            string ystr = "Y: " + y.ToString("0.00") + " m/s";
            string zstr = "Z: " + z.ToString("0.00") + " m/s";

            labelAccelerometer.Content = xstr + "   " + ystr + "   " + zstr;
        }

        public void setGyroscope(float x, float y, float z)
        {
            string xstr = "X: " + x.ToString("0.00") + " ˚/s";
            string ystr = "Y: " + y.ToString("0.00") + " ˚/s";
            string zstr = "Z: " + z.ToString("0.00") + " ˚/s";

            labelGyroscope.Content = xstr + "   " + ystr + "   " + zstr;
        }

        public void setHumidity(float v)
        {
            labelHumidity.Content = v.ToString("0.0000") + " %";
        }

        public void setMagnetometer(float x, float y, float z)
        {
            string xstr = "X: " + x.ToString("0.00") + " µT";
            string ystr = "Y: " + y.ToString("0.00") + " µT";
            string zstr = "Z: " + z.ToString("0.00") + " µT";
            labelMagnometer.Content = xstr + "   " + ystr + "   " + zstr;
        }

        public void setPressure(float v)
        {
            labelPressure.Content = v.ToString("0.0000") + " hPa";
        }

        public void setTemperature(float v)
        {
            labelTemperature.Content = v.ToString() + " °";
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

        // Methods for reading data
        // "Read's" methods involves in read-mode
        // "Notify's" methods involves in notification-mode

        async Task ReadAccelerometer()
        {
            byte[] rawData = await BLE_Utilities.ReadData(Sensor.Accelerometer);
            float[] vals = SensorConvert.convertAccelerometer(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setAccelerometer(vals[0], vals[1], vals[2])));
        }

        async void NotifyAccelerometer(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] rawData = BLE_Utilities.getDataBytes(args);
            float[] vals = SensorConvert.convertAccelerometer(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setAccelerometer(vals[0], vals[1], vals[2])));
        }


        async Task ReadGyroscope()
        {
            byte[] rawData = await BLE_Utilities.ReadData(Sensor.Gyroscope);
            float[] vals = SensorConvert.convertGyroscope(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setGyroscope(vals[0], vals[1], vals[2])));
        }

        async void NotifyGyroscope(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] rawData = BLE_Utilities.getDataBytes(args);
            float[] vals = SensorConvert.convertGyroscope(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setGyroscope(vals[0], vals[1], vals[2])));
        }

        //Read values method
        async Task ReadHumidity()
        {
            byte[] rawData = await BLE_Utilities.ReadData(Sensor.Humidity);
            float acthum = SensorConvert.convertHumidity(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setHumidity(acthum)));
        }
        //Notification event handler
        async void NotifyHumidity(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] rawData = BLE_Utilities.getDataBytes(args);
            float acthum = SensorConvert.convertHumidity(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setHumidity(acthum)));
        }

        async Task ReadMagnetometer()
        {
            byte[] rawData = await BLE_Utilities.ReadData(Sensor.Accelerometer);
            float[] vals = SensorConvert.convertAccelerometer(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setMagnetometer(vals[0], vals[1], vals[2])));
        }

        async void NotifyMagnetometer(GattCharacteristic sender, GattValueChangedEventArgs args)
        {

            byte[] rawData = BLE_Utilities.getDataBytes(args);
            float[] vals = SensorConvert.convertAccelerometer(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setMagnetometer(vals[0], vals[1], vals[2])));
        }

        async Task ReadTemperature()
        {
            byte[] rawData = await BLE_Utilities.ReadData(Sensor.Temperature);
            float temp = SensorConvert.convertTemperature(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setTemperature(temp)));
        }

        async void NotifyTemperature(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] rawData = BLE_Utilities.getDataBytes(args);
            float temp = SensorConvert.convertTemperature(rawData);
            await this.Dispatcher.BeginInvoke((Action)(() => setTemperature(temp)));
        }

        async void notifyButton(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] data = BLE_Utilities.getDataBytes(args);

            //My method is actually " setButton setButtons(data[0]) "
            //But if you want to change ANYTHING in the UI you have to wrap 
            //like I did below.
            await this.Dispatcher.BeginInvoke((Action)(() => setButtons(data[0])));
        }
    }
}