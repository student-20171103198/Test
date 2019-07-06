using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class ConvertBing
    {
        private double longitude;
        private double Latitude;
        private Boolean isChina;

        public ConvertBing()
        {

        }

        public ConvertBing(double longitude, double latitude)
        {
            this.longitude = longitude;
            Latitude = latitude;
        }

        public double getLongitude()
        {
            return longitude;
        }

        public void setLongitude(double longitude)
        {
            this.longitude = longitude;
        }

        public double getLatitude()
        {
            return Latitude;
        }

        public void setLatitude(double latitude)
        {
            Latitude = latitude;
        }

        public Boolean IsChina()
        {
            return isChina;
        }

        public void setChina(Boolean china)
        {
            isChina = china;
        }
    }

}

