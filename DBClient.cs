using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;

namespace HotelDBConnection
{
    class DBClient
    {
        //Database connection string - replace it with the connnection string to your own database 
        const string connectionString = @"Data Source = (localdb)\ProjectModels;Initial Catalog = HotelFacilityDB; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        
        private int GetMaxDBKey<T>(SqlConnection connection)
        {
            //Keep type T's name - e.g. 'Hotel' 
            string typeName = typeof(T).Name;

            Console.WriteLine($"Calling -> GetMaxDBKey<{typeName}>");

            //This SQL command will fetch one row from the Demo<typeName> table:
            //The row with the max primary key
            string queryStringMaxDBKey = $"SELECT MAX({typeName}_No) FROM Demo{typeName}";
            Console.WriteLine($"SQL applied: {queryStringMaxDBKey}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(queryStringMaxDBKey, connection);
            SqlDataReader reader = command.ExecuteReader();

            //Assume undefined value 0 for max database key
            int MaxDBKey = 0;

            //Is there any rows in the query
            if (reader.Read())
            {
                //Yes, get max database
                MaxDBKey = reader.GetInt32(0); //Reading int from 1st column - Always assuming int
            }

            //Close reader
            reader.Close();

            Console.WriteLine($"Max database key#: {MaxDBKey}");
            Console.WriteLine();

            //Return max database table key - in fact primary of the table 
            return MaxDBKey;
        }     
        private int Delete<T>(SqlConnection connection, int item_no)
        {
            //Keep type T's name - e.g. 'Hotel' 
            string typeName = typeof(T).Name;

            Console.WriteLine($"Calling -> Delete<{typeName}>");

            //This SQL command will delete one row from the Demo<typeName> table:
            //The row with primary key item_No
            string deleteCommandString = $"DELETE FROM Demo{typeName} WHERE {typeName}_No = {item_no}";
            Console.WriteLine($"SQL applied: {deleteCommandString}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(deleteCommandString, connection);
            Console.WriteLine($"Deleting item #{item_no}");
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            //Return number of rows affected
            return numberOfRowsAffected;
        }
        private int Update<T>(SqlConnection connection, T t)
        {
            //Keep type T's name - e.g. 'Hotel' 
            string typeName = typeof(T).Name;

            Console.WriteLine($"Calling -> Update<{typeName}>");

            //Compose SQL command will update one row from the Demo<typeName> table:
            //The row with primary key <typeName>_No
            
            //Item with primary key 'itemId' not known yet
            string itemId = null;
            
            //Start composing SQL command string
            string updateCommandString = $"UPDATE Demo{typeName} SET ";

            //Find out how many properties that parameter t of type T has
            int numOfProps = t.GetType().GetProperties().Length;

            //Get paramter t's PropertyInfo (reflection information)  
            PropertyInfo[] pInfo = t.GetType().GetProperties();

            //Iterate over the properties  in order to construct the UPDATE SQL stateemnt 
            for (int i = 0; i < numOfProps; i++)
            {
                //Prepend comma separator to values - skip for the first value
                if (i > 1)
                {
                    updateCommandString += ",";
                }

                //First property?
                if (i == 0)
                {
                    //Keep item id - primary of the database row to be updated
                    itemId += t.GetType().GetProperties()[i].GetValue(t, null);
                }
                else
                {
                    //Otherwise add to values part of UPDATE statement

                    //Append name of the i'th property/attribute and the '=' sign
                    updateCommandString += pInfo[i].Name;
                    updateCommandString += "=";

                    //If string value prepend ' to value
                    if (pInfo[i].PropertyType.Name == "String")
                    {
                        updateCommandString += "'";
                    }

                    //Append value 
                    updateCommandString += t.GetType().GetProperties()[i].GetValue(t, null);

                    //If string value append ' to value
                    if (pInfo[i].PropertyType.Name == "String")
                    {
                        updateCommandString += "'";
                    }
                }
            }

            //Append WHERE clause 
            updateCommandString += $" WHERE {typeName}_No = {itemId}";

            Console.WriteLine($"SQL applied: {updateCommandString}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(updateCommandString, connection);
            Console.WriteLine($"Updating item #{itemId}");
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            //Return number of rows affected
            return numberOfRowsAffected;
        }
        private int Insert<T>(SqlConnection connection, T t) 
        //Require the T has a default constructor - i.e. one without parameters    
        where T : new()
        {
            //Keep type T's name - e.g. 'Hotel' 
            string typeName = typeof(T).Name;

            Console.WriteLine($"Calling -> Insert<{typeName}>");

            //This SQL command will insert one row into the Demo<typeName> table
            //with primary key as first the first property in t

            //Construct start of string 
            string insertCommandString = $"INSERT INTO Demo{typeName} VALUES(";

            //Find out how many properties that parameter t of type T has
            int numOfProps = t.GetType().GetProperties().Length;

            //Get paramter t's PropertyInfo (reflection information)
            PropertyInfo[] pInfo = t.GetType().GetProperties();

            //Construct value part by iterating over properties of t
            for (int i = 0; i < numOfProps; i++)
            {
                //Prepend comma to value - except for the first value 
                if (i > 0)
                {
                    insertCommandString += ",";
                }

                //Prepend ' to value - if it is a string
                if (pInfo[i].PropertyType.Name == "String")
                {
                    insertCommandString += "'";
                }

                //Append the actual value of the property
                insertCommandString += t.GetType().GetProperties()[i].GetValue(t, null);

                //Append ' to value - if it is a string
                if (pInfo[i].PropertyType.Name == "String")
                {
                    insertCommandString += "'";
                }
            }
            
            //Finish SQL statement
            insertCommandString += ")";

            Console.WriteLine($"SQL applied: {insertCommandString}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(insertCommandString, connection);

            Console.WriteLine($"Creating item");
            int numberOfRowsAffected = command.ExecuteNonQuery();

            Console.WriteLine($"Number of rows affected: {numberOfRowsAffected}");
            Console.WriteLine();

            //Return number of rows affected 
            return numberOfRowsAffected;
        }
        private List<T> ListAll<T>(SqlConnection connection)
        //Require the T has a default constructor - i.e. one without parameters    
        where T : new()
        {
            //Keep type T's name - e.g. 'Hotel' 
            string typeName = typeof(T).Name;

            Console.WriteLine($"Calling -> ListAll: {typeName}");

            //This SQL command will fetch all rows and columns from the Demo<typeName> table
            string queryStringAllItems = $"SELECT * FROM Demo{typeName}";
            Console.WriteLine($"SQL applied: {queryStringAllItems}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(queryStringAllItems, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine($"Listing all {typeName} items:");

            //NO rows in the query            
            if (!reader.HasRows)
            {
                //End here
                Console.WriteLine($"No such items ({typeName}) in database");
                reader.Close();

                //Return null for 'no items found'
                return null;
            }

            //Create list of items found
            List<T> items = new List<T>();
            while (reader.Read())
            {
                //If we reached here, there is still one item to be put into the list 

                //Create a new object of class T - it could be a Hotel
                T nextItem = new T();
                
                //Keep number of fields/attributes in the query result 
                int fc = reader.FieldCount;
                
                //Iterate over the fields in order to fill in the values into the T object 
                for (int i = 0; i < fc; i++)
                {
                    Object val = reader.GetValue(i);
                    string attributeName = reader.GetName(i);
                    nextItem.GetType().GetProperty(attributeName).SetValue(nextItem, val);
                }
                
                //Add T object to list
                items.Add(nextItem);

                Console.WriteLine(nextItem);
            }

            //Close reader
            reader.Close();
            Console.WriteLine();

            //Return list of hotels
            return items;
        }
        private T Get<T>(SqlConnection connection, int item_no)
        //Require the T has a default constructor - i.e. one without parameters
        where T : new()
        {
            //Keep type T's name - e.g. 'Hotel'
            string typeName = typeof(T).Name;

            //Let result have it's default value
            T result = default;

            Console.WriteLine($"Calling -> Get<{typeName}>");

            //This SQL command will fetch the row with primary key item_no from the Demo<typeName> table
            string queryStringOneItem = $"SELECT * FROM Demo{typeName} WHERE {typeName}_no = {item_no}";
            Console.WriteLine($"SQL applied: {queryStringOneItem}");

            //Apply SQL command
            SqlCommand command = new SqlCommand(queryStringOneItem, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine($"Finding item#: {item_no}");

            //NO rows in the query? 
            if (!reader.HasRows)
            {
                //End here
                Console.WriteLine($"No {typeName} item in database");
                reader.Close();

                //Return null for 'no item found'
                return result;
            }

            //Fetch item object from the database           
            if (reader.Read())
            {
                //Create a new object of class T - it could be a Hotel
                result = new T();

                //Keep number of fields/attributes in the query result 
                int fc = reader.FieldCount;

                //Iterate over the fields in order to fill in the values into the T object
                for (int i = 0; i < fc; i++)
                {
                    Object val = reader.GetValue(i);
                    string attributeName = reader.GetName(i);
                    result.GetType().GetProperty(attributeName).SetValue(result, val);
                }
                Console.WriteLine(result);
            }

            //Close reader
            reader.Close();
            Console.WriteLine();

            //Return found item
            return result;
        }
        public void Start()
        {
            //Apply 'using' to connection (SqlConnection) in order to call Dispose (interface IDisposable) 
            //whenever the 'using' block exits
            #pragma warning disable IDE0063
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Open connection
                connection.Open();

                Console.WriteLine();
                Console.WriteLine("PROCESSING HOTEL CRUD");
                Console.WriteLine();

                //List all hotels in the database
                ListAll<Hotel>(connection);

                //Create a new hotel with primary key equal to current max primary key plus 1
                Hotel newHotel = new Hotel()
                {
                    Hotel_No = GetMaxDBKey<Hotel>(connection) + 1,
                    Name = "New Hotel",
                    Address = "Maglegaardsvej 2, 4000 Roskilde"
                };

                Insert<Hotel>(connection, newHotel);

                //List all hotels in the database
                ListAll<Hotel>(connection);

                //Get the newly inserted hotel from the database in order to update it 
                Hotel hotelToBeUpdated = Get<Hotel>(connection, newHotel.Hotel_No);

                //Alter Name and Addess properties
                hotelToBeUpdated.Name += "(updated)";
                hotelToBeUpdated.Address += "(updated)";

                //Update the hotel in the database 
                Update<Hotel>(connection, hotelToBeUpdated);

                //List all hotels including the updated one
                ListAll<Hotel>(connection);

                //Get the updated hotel in order to delete it
                Hotel hotelToBeDeleted = Get<Hotel>(connection, hotelToBeUpdated.Hotel_No);

                //Delete the hotel
                Delete<Hotel>(connection, hotelToBeDeleted.Hotel_No);

                //List all hotels - now without the deleted one
                ListAll<Hotel>(connection);

                Console.WriteLine();
                Console.WriteLine("PROCESSING FACILITY CRUD");
                Console.WriteLine();

                //List all facilities in the database
                ListAll<Facility>(connection);

                //Create a new facility with primary key equal to current max primary key plus 1
                Facility newFacility = new Facility()
                {
                    Facility_No  = GetMaxDBKey<Facility>(connection) + 1,
                    Name = "New Facility",
                    PricePerUnit = 42
                };

                Insert<Facility>(connection, newFacility);

                //List all facilities in the database
                ListAll<Facility>(connection);

                //Get the newly inserted facility from the database in order to update it                
                Facility facilityToBeUpdated = Get<Facility>(connection, newFacility.Facility_No);

                //Alter Name and PricePerUnit properties
                facilityToBeUpdated.Name += "(updated)";
                facilityToBeUpdated.PricePerUnit += 1;

                //Update the facility in the database 
                Update<Facility>(connection, facilityToBeUpdated);

                //List all facilities including the updated one
                ListAll<Facility>(connection);

                //Get the updated facility in order to delete it
                Facility facilityToBeDeleted = Get<Facility>(connection, facilityToBeUpdated.Facility_No);

                //Delete the facility
                Delete<Facility>(connection, facilityToBeDeleted.Facility_No);

                //List all facilities - now without the deleted one
                ListAll<Facility>(connection);

                Console.WriteLine();
                Console.WriteLine("PROCESSING HOTEL-FACILITY CRUD");
                Console.WriteLine();

                //List all hotel facilities
                ListAll<HotelFacility>(connection);

                //Create a new hotel-facility with primary key equal to current max primary key plus 1
                HotelFacility newHotelFacility = new HotelFacility()
                {
                    HotelFacility_No = GetMaxDBKey<HotelFacility>(connection) + 1,
                    
                    //In this case we use max primary key on Hotel
                    Hotel_No = GetMaxDBKey<Hotel>(connection),

                    //In this case we use max primary key on Facility
                    Facility_No = GetMaxDBKey<Facility>(connection)
                };

                Insert<HotelFacility>(connection, newHotelFacility);

                //List all hotel-facilities in the database
                ListAll<HotelFacility>(connection);

                //Get the newly inserted hotel-facility from the database in order to delete it 
                HotelFacility hotelFacilityToBeDeleted = Get<HotelFacility>(connection, newHotelFacility.HotelFacility_No);

                //Delete the hotel-facility
                Delete<HotelFacility>(connection, hotelFacilityToBeDeleted.HotelFacility_No);

                //List all hotel-facilities - now without the deleted one
                ListAll<HotelFacility>(connection);
            }
            #pragma warning restore IDE0063
        
        }

    }
}
