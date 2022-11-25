using System;
using System.Collections.Generic;
using System.Text;

namespace HotelDBConnection
{
    class Facility
    {
        public int Facility_No { get; set; }
        public string Name { get; set; }

        public int PricePerUnit { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Facility_No}, Name: {Name}";
        }
    }
}
