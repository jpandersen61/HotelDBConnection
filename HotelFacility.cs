using System;
using System.Collections.Generic;
using System.Text;

namespace HotelDBConnection
{
    class HotelFacility
    {
        public int HotelFacility_No { get; set; }
        public int Facility_No { get; set; }
        public int Hotel_No { get; set; }

        public override string ToString()
        {
            return $"HotelFacility#: {HotelFacility_No}, Hotel#: {Hotel_No}, Facility#: {Facility_No}";
        }
    }
}
