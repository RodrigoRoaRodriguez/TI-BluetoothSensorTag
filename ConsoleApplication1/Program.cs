using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace UUID_Test
{
    class Program
    {
        public class SensorValueChangedEventArgs : EventArgs
        {
            static void Main(string[] args)
            {
                PrintUUIDs();
                //OLD CODE: READING DATA (you NEED to run the UI library first!!!)

                //Console.WriteLine("Accelerometer:");
                //const string XYZ = "x : {0,-4} y : {1,-4} z : {1,-4}";

                ////Do this many(=50) times so that people can see change
                //for (int i = 0; i < 50; i++)
                //{
                //    //I have managed to connect to a device and read data.
                //    //These are the three bytes from the accelerometer
                //    byte[] b = ReadData(Sensor.Accelerometer).Result;

                //    //This writes it out in a human friendly format
                //    Console.WriteLine(String.Format(XYZ, b[0], b[1], b[2]));
                //}

                //Console.ReadLine();//This is only so that terminal doesn't close immediately

                //NEW CODE: USING NOTIFICATIONS (you DON'T need to run the UI library first!!!)
                Console.WriteLine("Buttons:");

                executeOnNotification(Sensor.Keys, buttonPressed);

                Console.ReadLine();//This is only so that terminal doesn't close immediately

            }


            private static async void executeOnNotification(Sensor sensor, Windows.Foundation.TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> methodToExecute)
            {
                //Get gatt characteristic
                GattCharacteristic characteristic = await GetCharacteristic(sensor);

                //Enable notifications
                GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                //WARNING!!! the "+=" tells event listener to CALL a delagate method.
                characteristic.ValueChanged +=  methodToExecute;
            }

            static byte[] getDataBytes(GattValueChangedEventArgs args)
            {
                //Create a byte array(with same size as the caracteristics value)
                var data = new byte[args.CharacteristicValue.Length];

                //Read to byte array from args
                DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
                return data;
            }

            
            /// <summary>
            /// This is an eventHandler delegate that will be called by an event handler.
            /// </summary>
            /// <param name="sender">the gatt characteristic that was changed</param>
            /// <param name="args">contains all kinds of data</param>
            static void buttonPressed(GattCharacteristic sender, GattValueChangedEventArgs args)
            {
                //Create a byte array(with same size as the caracteristics value)
                Byte[] data = getDataBytes(args);

                //Display "HIT" on console and print out data.
                Console.WriteLine("HIT");
                //1 is LEFT BUTTON, 2 is RIGHT BUTTON, 3 is BOTH.
                Console.WriteLine(data[0]);
            }

            /// <summary>
            /// This will print all the sensor UUID that I have implemented
            /// </summary>
            static void PrintUUIDs()
            {
                const string FORMAT = "{0,-20}{1}";

                Console.WriteLine(String.Format(FORMAT, "SENSOR NAME:", "UUID"));
                foreach (Sensor item in Enum.GetValues(typeof(Sensor)))
                {
                    Console.WriteLine(String.Format(FORMAT, item.ToString(), item.GetServiceUUID()));
                }
            }

            /// <summary>
            /// Reads data from a sensor by using it's data service.
            /// </summary>
            /// <param name="sensor"></param>
            /// <returns></returns>
            static async Task<byte[]> ReadData(Sensor sensor)
            {
                //Get characteristic
                GattCharacteristic gattCharacteristic = GetCharacteristic(sensor).Result;

                //Read value
                GattReadResult read = await gattCharacteristic.ReadValueAsync(Windows.Devices.Bluetooth.BluetoothCacheMode.Uncached);

                if (read.Status == GattCommunicationStatus.Unreachable)
                    throw new Exception("Device unreachable");

                //Get the result into a byte array
                Byte[] data = new byte[read.Value.Length];
                DataReader.FromBuffer(read.Value).ReadBytes(data);

                //Finished!
                return data;
            }


            /// <summary>
            /// Returns a GATT characteristic for a sensor's data service.
            /// </summary>
            /// <param name="sensor">the sensor you want to read</param>
            /// <returns>the GATT characteristic</returns>
            static async Task<GattCharacteristic> GetCharacteristic(Sensor sensor)
            {

                //Get a query for devices with this service
                string deviceSelector = GattDeviceService.GetDeviceSelectorFromUuid(new Guid(sensor.GetServiceUUID()));

                //seek devices using the query
                var deviceCollection = await DeviceInformation.FindAllAsync(deviceSelector);

                //return info for the first device you find
                DeviceInformation device = deviceCollection.FirstOrDefault();

                if (device == null)
                    throw new Exception("Device not found.");

                // using the id get the service
                GattDeviceService service = await GattDeviceService.FromIdAsync(device.Id);

                //get all characteristics
                IReadOnlyList<GattCharacteristic> characteristics = service.GetCharacteristics(new Guid(sensor.GetDataUUID()));

                if (characteristics.Count == 0)
                    throw new Exception("characteristic not found.");

                //Reaturn event handler for first characteristic
                return characteristics[0];
            }
        }
    }
}
