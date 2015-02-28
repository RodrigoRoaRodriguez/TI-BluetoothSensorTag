﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using BLE_Demo.Model;

namespace BLE_Demo.Model
{
    class BLE_Utilities
    {

        /// <summary>
        /// Returns a GATT characteristic for a sensor's data service.
        /// </summary>
        /// <param name="sensor">the sensor you want to read</param>
        /// <returns>the GATT characteristic</returns>
        public static async Task<GattCharacteristic> GetCharacteristic(Sensor sensor)
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


        /// <summary>
        /// Attaches an EventHandler method to a given sensor so that it is called every time that its value changes.
        /// 
        /// </summary>
        /// <param name="sensor"> the targeted sensor</param>
        /// <param name="methodToExecute"> the method to be called on value change</param>
        public static async void executeOnNotification(Sensor sensor, Windows.Foundation.TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> methodToExecute)
        {
            //Get gatt characteristic
            GattCharacteristic characteristic = await GetCharacteristic(sensor);

            //Enable notifications
            GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

            //WARNING!!! the "+=" tells event listener to CALL a delagate method.
            characteristic.ValueChanged += methodToExecute;
        }



        public static byte[] getDataBytes(GattValueChangedEventArgs args)
        {
            //Create a byte array(with same size as the caracteristics value)
            byte[] data = new byte[args.CharacteristicValue.Length];

            //Read to byte array from args
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
            return data;
        }


        /// <summary>
        /// Reads data from a sensor's data service. 
        /// Please be aware that no all sensors will allow you to directly read data from them. Buttons, for instance, do not.
        ///
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        static async Task<byte[]> ReadData(Sensor sensor)
        {
            //Get characteristic
            GattCharacteristic gattCharacteristic = GetCharacteristic(sensor).Result;

            //Fetch result
            GattReadResult read = await gattCharacteristic.ReadValueAsync(Windows.Devices.Bluetooth.BluetoothCacheMode.Uncached);

            if (read.Status == GattCommunicationStatus.Unreachable)
                throw new Exception("Device unreachable");

            //Extract data from result object to a byte array
            Byte[] data = new byte[read.Value.Length];
            DataReader.FromBuffer(read.Value).ReadBytes(data);

            //Finished!
            return data;
        }
    }
}
