using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_Demo.Model
{
    public enum Sensor
    {
        // Format: Name = characters 4 to 8 of the UUID a
        Accelerometer = 0xaa10,
        Gyroscope = 0xAA50,
        Humidity = 0xAA20,
        Keys = 0xFFE0,
        Magnetometer = 0xAA30,
        Pressure = 0xAA40,
        Temperature = 0xAA00,
        TestService = 0xAA60
        // UUID for Over-The-Air-Dowload firmware service was intentionally ommited so that no one accidentally bricks a tag.
    }

    static class SensorsExtensions
    {
        /// <summary>
        /// The common part that all sensor UUIDs share.
        /// The x:s are placeholders to be replaced.
        /// IMPORTANT: CASE SENSITIVE for some reason. Leads to unhandled exception otherwise.
        /// </summary>
        private const String TI_BASE_UUID = "f000XXXX-0451-4000-b000-000000000000";

        public static String GetServiceUUID(this Sensor sensor)
        {
            if (Sensor.Keys == sensor)
                return "0000ffe0-0000-1000-8000-00805f9b34fb";
            return TI_BASE_UUID.Replace("XXXX", ((int)sensor).ToString("x4"));
        }

        public static String GetDataUUID(this Sensor sensor)
        {
            if (Sensor.Keys == sensor)
                return "0000ffe1-0000-1000-8000-00805f9b34fb";
            return TI_BASE_UUID.Replace("XXXX", ((int)sensor + 1).ToString("x4"));
        }
    }
}
