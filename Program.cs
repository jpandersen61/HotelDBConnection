using System;

namespace HotelDBConnection
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
