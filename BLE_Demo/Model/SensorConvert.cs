using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE_Demo.Model
{
    public static class SensorConvert
    {
        public static float[] convertAccelerometer(byte[] rawData)
        {
            float x = rawData[0] / 64f;
            float y = rawData[1] / 64f;
            float z = rawData[2] / 64f;
            return new float[] {x, y, z};
        }

        public static float convertHumidity(byte[] rawData)
        {
            int hum = BitConverter.ToUInt16(rawData, 2);
            hum &= ~0x0003; // clear bits [1..0] (status bits)
            //-- calculate relative humidity [%RH] --
            float acthum = -6.0f + 125.0f / 65536f * (float)hum; // RH= -6 + 125 * SRH/2^16
            return acthum;
        }

        public static float convertTemperature(byte[] rawData)
        {
            return (float)(BitConverter.ToUInt16(rawData, 2) / 128.0);
        }

        public static float convertPressure(byte[] rawData)
        {
            return 0f;
        }

        public static float[] convertGyroscope(byte[] rawData)
        {
            float x = BitConverter.ToInt16(rawData, 0) * (500f / 65536f);
            float y = BitConverter.ToInt16(rawData, 2) * (500f / 65536f);
            float z = BitConverter.ToInt16(rawData, 4) * (500f / 65536f);

            return new float[] { x, y, z };
        }
    }
}

