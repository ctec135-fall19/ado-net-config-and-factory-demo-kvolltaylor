using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration; // requires adding reference/reference update
using System.Data.Common;
using System.Data.SqlClient;
using static System.Console;

namespace FactoryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            #region get config info
            string dataProvider = ConfigurationManager.AppSettings["provider"];
            WriteLine("\tprovider: {0}", dataProvider);

            // alternate way of creating the connection string - see p. 833
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = "Northwind",
                DataSource = @"(localdb)\mssqllocaldb",
                ConnectTimeout = 30,
                IntegratedSecurity = true
            };
            WriteLine($"\tBuilt Connection String: " +
                $"{cnStringBuilder.ConnectionString}\n");
            #endregion

            // get factory
            // I want a DbProviderFactory, but am calling the DbProviderFactories class to get it
            DbProviderFactory factory = 
                DbProviderFactories.GetFactory(dataProvider);

            // using the using keyword allow me to block out my code

            // create a connection object
            // set connection string to object
            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    WriteLine("There was an issue creating the connection");
                    return;
                }
                else WriteLine("-> Connection created");

                connection.ConnectionString = cnStringBuilder.ConnectionString;
                connection.Open();

                // create empty command object
                DbCommand myCommand = factory.CreateCommand();
                if (myCommand == null)
                {
                    WriteLine("There is an issue creating the command");
                }
                else WriteLine($"Your command object is a " +
                    $"{myCommand.GetType().Name}");
                // setting the connection in my command object
                // passed an object in for it to use
                myCommand.Connection = connection;
                myCommand.CommandText = "SELECT * FROM Shippers";

                // create our data reader
                // command text is query so using ExecuteReader
                // creating the dataReader block and iterating thru it
     
                using (DbDataReader dataReader = myCommand.ExecuteReader())
                {
                    // testing as I move through the read
                    // while I can keep returning info keep reading

                    while (dataReader.Read())
                    {
                        // if you index into a table by columns, it is not selfdocumenting
                        // helps to add name to help more easily remember what info
                        WriteLine($"-> shipper # {dataReader["ShipperId"]} " +
                            $"name is {dataReader[1]} phone: {dataReader[2]}"); 
                    }
                }

            }
        }
    }
}
