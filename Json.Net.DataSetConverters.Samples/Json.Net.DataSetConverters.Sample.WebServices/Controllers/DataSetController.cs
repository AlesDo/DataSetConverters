using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Json.Net.DataSetConverters.Sample.WebServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataSet = System.Data.DataSet;

namespace Json.Net.DataSetConverters.Sample.WebServices.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class DataSetController : ControllerBase
   {
      private readonly CustomerService customerService;

      public DataSetController(CustomerService customerService)
      {
         this.customerService = customerService;
      }

      // GET: api/DataSet
      [HttpGet]
      public async Task<DataSet> GetAsync()
      {
         return await customerService.GetCustomerData().ConfigureAwait(false);
      }

      // POST: api/DataSet
      [HttpPost]
      public async Task Post([FromBody] DataSet updatedData)
      {
         await customerService.UpdateCustomerData(updatedData).ConfigureAwait(false);
      }
   }
}
