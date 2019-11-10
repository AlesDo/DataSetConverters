using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Json.Net.DataSetConverters.Sample.WindowsForms.Client
{
   public partial class Form1 : Form
   {
      private HttpClient httpClient = new HttpClient()
      {
         BaseAddress = new Uri("https://localhost:44325/")
      };

      private JsonMediaTypeFormatter jsonMediaTypeFormatter = new JsonMediaTypeFormatter();

      public Form1()
      {
         InitializeHttpClient();
         InitializeJsonSerialization();
         InitializeComponent();
      }

      private void InitializeHttpClient()
      {
         httpClient.DefaultRequestHeaders.Accept.Clear();
         httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      }

      private void InitializeJsonSerialization()
      {
         jsonMediaTypeFormatter.SerializerSettings.Converters = new List<JsonConverter>() { new Json.Net.DataSetConverters.DataSetConverter(), new Json.Net.DataSetConverters.DataTableConverter() };
         JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
         {
            Converters = new List<JsonConverter>() { new Json.Net.DataSetConverters.DataSetConverter(), new Json.Net.DataSetConverters.DataTableConverter() }
         };
      }

      private async void refreshButton_Click(object sender, EventArgs e)
      {
         DataSet dataSet = await GetDataSetAsync();
         customersBindingSource.DataSource = dataSet;
         customersBindingSource.ResetBindings(true);
      }

      private async Task<DataSet> GetDataSetAsync()
      {
         HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("api/dataset");
         if (httpResponseMessage.IsSuccessStatusCode)
         {
            return await httpResponseMessage.Content.ReadAsAsync<DataSet>(new MediaTypeFormatter[] { jsonMediaTypeFormatter });
         }
         return CreateEmptyDataSet();
      }

      private async void updateButton_Click(object sender, EventArgs e)
      {
         HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("api/dataset", (DataSet)customersBindingSource.DataSource, jsonMediaTypeFormatter);
         if (!httpResponseMessage.IsSuccessStatusCode)
         {
            MessageBox.Show("Update of data failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void clearButton_Click(object sender, EventArgs e)
      {
         customersBindingSource.DataSource = CreateEmptyDataSet();

         customersBindingSource.ResetBindings(true);
      }

      private DataSet CreateEmptyDataSet()
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
         initialDataSet.Tables.Add(customerTable);

         return initialDataSet;
      }
   }
}
