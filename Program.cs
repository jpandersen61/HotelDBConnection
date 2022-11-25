using System;
using HotelDBConnection;

namespace HotelDBConnectionFacility
{
    class Program
    {
        static void Main(string[] args)
        {
            DBClient dbc = new DBClient();
            dbc.Start();
        }
    }
}
