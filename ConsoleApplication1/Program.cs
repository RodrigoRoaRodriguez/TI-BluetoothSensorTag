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
        static void Main(string[] args)
        {
            PrintUUIDs();

            Console.WriteLine("Accelerometer:");
            const string XYZ = "x : {0,-4} y : {1,-4} z : {1,-4}";

            //Do this many(=50) times so that people can see change
            for (int i = 0; i < 50; i++)
            {
                //I have managed to connect to a device and read data.
                //These are the three bytes from the accelerometer
                byte[] b = ReadData(Sensor.Accelerometer).Result;

                //This writes it out in a human friendly format
                Console.WriteLine(String.Format(XYZ, b[0], b[1], b[2]));
            }

            Console.ReadLine();//This is only so that terminal doesn't close immediately
        }

        /// <summary>
        /// This will print all the sensor UUID that I have implemented
        /// </summary>
        static void PrintUUIDs()
        {
            const string FORMAT = "{0,-20}{1}";

            Console.WriteLine(String.Format(FORMAT,"SENSOR NAME:", "UUID"));
            foreach (Sensor item in Enum.GetValues(typeof(Sensor)))
            {
                Console.WriteLine(String.Format(FORMAT, item.ToString(), item.GetServiceUUID()));
            }
        }



        static async Task<byte[]> ReadData(Sensor sensor)
        {
            byte[] invalid = { 13 };

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

            //Read value of first characteristic
            GattCharacteristic gattCharacteristic = characteristics[0];

            GattReadResult read = await gattCharacteristic.ReadValueAsync(Windows.Devices.Bluetooth.BluetoothCacheMode.Uncached);

            if (read.Status == GattCommunicationStatus.Unreachable)
                throw new Exception("Device unreachable");

            //Get the result into a byte array
            Byte[] data = new byte[read.Value.Length];
            DataReader.FromBuffer(read.Value).ReadBytes(data);

            //Finished!
            return data;
        }

    }
}
