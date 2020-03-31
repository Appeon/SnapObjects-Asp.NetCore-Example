using System;
using System.Collections.Generic;
using System.Linq;
using Appeon.MvcModelMapperDemo.Models;
using Appeon.SnapObjectsDemo.Service.Models;
using Appeon.SnapObjectsDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public IActionResult OnGet()
        {
            try
            {
                this.LoadData();
            }
            catch (Exception e)
            {

                return StatusCode(500, e.Message);
            }
            return Page();
        }

        private void LoadData()
        {
            try
            {
                this.QueryCustomers();

                this.QuerySalesPersons();

                this.QueryShipMethods();

                this.QueryOrderProducts();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public JsonResult OnGetRetrieveDddw(int customerId)
        {
            var result = new Dictionary<string, object>()
            {
                { "code", 1},

                { "creditcards", _genericServices.Get<DdCreditcard>()
                    .Retrieve(false, customerId)},

                { "customerAddresses", _genericServices.Get<DdCustomerAddress>()
                    .Retrieve(false, customerId) }
            };
            return new JsonResult(result);
        }


        private void QueryOrderProducts()
        {
            // 0: retrieve all products
            var ddOrderProducts = _genericServices.Get<DdOrderProduct>().Retrieve(false, 0);
            //  query products
            this.OrderProducts = new SelectList(ddOrderProducts, "Product_Productid", "Product_Name");
            //  convert to Dictionary
            this.OrderProductMaps = ddOrderProducts.ToDictionary(x => x.Product_Productid, x => x.Product_Name);
        }

        private void QueryCustomers()
        {
            var ddCustomers = _genericServices.Get<DdCustomer>().Retrieve(false);

            this.Customers = new SelectList(ddCustomers, "Customer_Customerid", "Fullname");
        }

        private void QuerySalesPersons()
        {
            var ddSalesPersons = _genericServices.Get<DdSalesPerson>().Retrieve(false);

            this.SalesPersons = new SelectList(ddSalesPersons, "Salesperson_Businessentityid", "Fullname");
        }

        private void QueryShipMethods()
        {
            var ddShipMethods = _genericServices.Get<DdShipMethod>().Retrieve(false);

            this.ShipMethods = new SelectList(ddShipMethods, "Shipmethodid", "Name");
        }

        public IActionResult OnPost()
        {
            try
            {
                ConvertData(SalesOrder);
                var insertedCount = _salesOrderService.Create(SalesOrder);
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, 0);
            }

            return GenJsonResult(1, "", SalesOrder.SalesOrderID);
        }
    }
}