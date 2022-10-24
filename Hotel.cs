using System;
using System.Collections.Generic;
using System.Text;

namespace HotelDBConnection
{
    public class Hotel
    {
        public int Hotel_No { get; set; }
        public string  Name { get; set; }

        public string Address { get; set; }

        public override string ToString()
        {
            return $"ID: {Hotel_No}, Name: {Name}, Address: {Address}"; 
        }
    }
}
