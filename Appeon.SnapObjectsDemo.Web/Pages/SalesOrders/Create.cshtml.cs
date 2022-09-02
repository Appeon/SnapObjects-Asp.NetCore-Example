using Appeon.MvcModelMapperDemo.Models;
using Appeon.SnapObjectsDemo.Service.Models;
using Appeon.SnapObjectsDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appeon.MvcModelMapperDemo.Pages.SalesOrders
{
    public class CreateModel : BasePageModel
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IGenericServiceFactory _genericServices;

        public CreateModel(ISalesOrderService salesOrderService,
                         IGenericServiceFactory genericServiceFactory)
        {
            _salesOrderService = salesOrderService;
            _genericServices = genericServiceFactory;
        }

        [BindProperty]
        public SalesOrder SalesOrder { get; set; }

        [BindProperty]
        public SalesOrderDetail SalesOrderDetail { get; set; }

        public SelectList Customers { get; set; }

        public SelectList SalesPersons { get; set; }

        public SelectList ShipMethods { get; set; }

        public IDictionary<int, string> OrderProductMaps { get; set; }

        public SelectList OrderProducts { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadData();
            }
            catch (Exception e)
            {

                return StatusCode(500, e.Message);
            }
            return Page();
        }

        private async Task LoadData()
        {
            try
            {
                await QueryCustomers();

                await QuerySalesPersons();

                await QueryShipMethods();

                await QueryOrderProducts();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JsonResult> OnGetRetrieveDddw(int customerId)
        {
            var result = new Dictionary<string, object>()
            {
                { "code", 1},

                { "creditcards", await _genericServices.Get<DdCreditcard>()
                    .RetrieveAsync(false, new object[] { customerId })},

                { "customerAddresses", await _genericServices.Get<DdCustomerAddress>()
                    .RetrieveAsync(false, new object[] { customerId }) }
            };
            return new JsonResult(result);
        }

        private async Task QueryOrderProducts()
        {
            // 0: retrieve all products
            var ddOrderProducts = await _genericServices.Get<DdOrderProduct>().RetrieveAsync(false, new object[] { 0 });
            //  query products
            OrderProducts = new SelectList(ddOrderProducts, "Product_Productid", "Product_Name");
            //  convert to Dictionary
            OrderProductMaps = ddOrderProducts.ToDictionary(x => x.Product_Productid, x => x.Product_Name);
        }

        private async Task QueryCustomers()
        {
            var ddCustomers = await _genericServices.Get<DdCustomer>().RetrieveAsync(false, default);

            Customers = new SelectList(ddCustomers, "Customer_Customerid", "Fullname");
        }

        private async Task QuerySalesPersons()
        {
            var ddSalesPersons = await _genericServices.Get<DdSalesPerson>().RetrieveAsync(false, default);

            SalesPersons = new SelectList(ddSalesPersons, "Salesperson_Businessentityid", "Fullname");
        }

        private async Task QueryShipMethods()
        {
            var ddShipMethods = await _genericServices.Get<DdShipMethod>().RetrieveAsync(false, default);

            ShipMethods = new SelectList(ddShipMethods, "Shipmethodid", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ConvertData(SalesOrder);
                var insertedCount = await _salesOrderService.CreateAsync(SalesOrder);
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, 0);
            }

            return GenJsonResult(1, "", SalesOrder.SalesOrderID);
        }

    }
}