using Bogus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataSet = System.Data.DataSet;

namespace Json.Net.DataSetConverters.Sample.WebServices.Services
{
   public class CustomerService
   {
      private DataSet customerData;

      public CustomerService()
      {
         customerData = BuildInitialDataSet();
      }

      public Task<DataSet> GetCustomerData()
      {
         // simulate async loading from database
         return Task.FromResult(customerData);
      }

      public Task UpdateCustomerData(DataSet updatedCustomerData)
      {
         // simulate save to database and store in memory
         updatedCustomerData.AcceptChanges();
         customerData = updatedCustomerData;
         return Task.CompletedTask;
      }

      private DataSet BuildInitialDataSet()
      {
         DataSet initialDataSet = new DataSet();
         DataTable customerTable = new DataTable("Customer");
         customerTable.Columns.Add(new DataColumn("Id")
         {
            AllowDBNull = false,
            AutoIncrement = true,
            DataType = typeof(int)
         });
         customerTable.Columns.Add("FirstName", typeof(string));
         customerTable.Columns.Add("LastName", typeof(string));
         AddCustomerData(customerTable);
         initialDataSet.Tables.Add(customerTable);
         initialDataSet.AcceptChanges();
         return initialDataSet;
      }

      private static void AddCustomerData(DataTable customerTable)
      {
         Faker<DataRow> rowGenerator = new Faker<DataRow>()
            .CustomInstantiator((faker) => customerTable.NewRow())
            .Rules((faker, dataRow) =>
            {
               dataRow.SetField<string>("FirstName", faker.Name.FirstName());
               dataRow.SetField<string>("LastName", faker.Name.LastName());
            });
         for (int rowCount = 0; rowCount < 1000; rowCount++)
         {
            customerTable.Rows.Add(rowGenerator.Generate());
         }
      }
   }
}
