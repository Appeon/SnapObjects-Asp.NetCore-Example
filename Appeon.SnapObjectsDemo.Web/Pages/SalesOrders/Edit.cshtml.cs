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
    public class EditModel : BasePageModel
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IGenericServiceFactory _genericServices;
        private readonly ISalesOrderDetailService _salesOrderDetailService;

        public EditModel(ISalesOrderService salesOrderService,
                         IGenericServiceFactory genericServiceFactory,
                         ISalesOrderDetailService salesOrderDetailService)
        {
            _salesOrderService = salesOrderService;
            _genericServices = genericServiceFactory;
            _salesOrderDetailService = salesOrderDetailService;
        }

        [BindProperty]
        public SalesOrder SalesOrder { get; set; }

        [BindProperty]
        public SalesOrderDetail SalesOrderDetail { get; set; }

        public SelectList Customers { get; set; }

        public IDictionary<int, string> CustomerMaps { get; set; }

        public SelectList SalesPersons { get; set; }

        public SelectList ShipMethods { get; set; }

        public SelectList Creditcards { get; set; }

        public SelectList CustomerAddresses { get; set; }

        public SelectList OrderProducts { get; set; }

        public IDictionary<int, string> OrderProductMaps { get; set; }

        public IActionResult OnGet(int id)
        {
            try
            {
                this.RetrieveDddw();

                SalesOrder = _salesOrderService.RetrieveByKey(true, id);

                if (SalesOrder != null)
                {
                    SalesOrderDetail = SalesOrder.OrderDetails?.FirstOrDefault();

                    this.RetrieveDddw(SalesOrder.CustomerID);
                }

                return Page();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        public JsonResult OnGetRetrieveProduct(int id)
        {
            var result = new Dictionary<string, object>()
            {
                { "code", 1},

                { "product", _genericServices.Get<DdOrderProduct>().Retrieve(false, id) }
            };

            return new JsonResult(result);
        }

        private void RetrieveDddw()
        {
            this.RetrieveCustomers();

            this.RetrieveSalesPersons();

            this.RetrieveShipMethods();

            this.RetrieveOrderProducts();
        }

        public void RetrieveDddw(int customerId)
        {
            this.RetrieveCreditcards(customerId);

            this.RetrieveCustomerAddresses(customerId);
        }

        private void RetrieveCustomers()
        {
            var ddCustomers = _genericServices.Get<DdCustomer>().Retrieve(false);

            this.Customers = new SelectList(ddCustomers, "Customer_Customerid", "Fullname");

            this.CustomerMaps = ddCustomers.ToDictionary(x => x.Customer_Customerid, x => x.Fullname);
        }

        private void RetrieveSalesPersons()
        {
            var ddSalesPersons = _genericServices.Get<DdSalesPerson>().Retrieve(false);

            this.SalesPersons = new SelectList(ddSalesPersons, "Salesperson_Businessentityid", "Fullname");
        }

        private void RetrieveShipMethods()
        {
            var ddShipMethods = _genericServices.Get<DdShipMethod>().Retrieve(false);

            this.ShipMethods = new SelectList(ddShipMethods, "Shipmethodid", "Name");
        }

        private void RetrieveCreditcards(int customerId)
        {
            var ddCreditcards = _genericServices.Get<DdCreditcard>()
                                                .Retrieve(false, customerId);

            this.Creditcards = new SelectList(ddCreditcards, "Creditcard_Creditcardid", "Creditcard_CardNumber");
        }

        private void RetrieveCustomerAddresses(int customerId)
        {
            var ddCustomerAddresses = _genericServices.Get<DdCustomerAddress>()
                                                      .Retrieve(false, customerId);

            this.CustomerAddresses = new SelectList(
                ddCustomerAddresses, "Businessentityaddress_Addressid", "Address_Addressline1");
        }

        private void RetrieveOrderProducts()
        {
            // 0: retrieve all products
            var ddOrderProducts = _genericServices.Get<DdOrderProduct>()
                                                  .Retrieve(false, 0);

            this.OrderProducts = new SelectList(
                ddOrderProducts, "Product_Productid", "Product_Name");

            this.OrderProductMaps = ddOrderProducts.ToDictionary(x => x.Product_Productid, x => x.Product_Name);
        }

        public IActionResult OnPost()
        {
            try
            {
                ConvertData(SalesOrder);
                SalesOrder.ModifiedDate = DateTime.Now;
                var modifiedCount = _salesOrderService.Update(SalesOrder);
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, SalesOrder.SalesOrderID);
            }

            return GenJsonResult(1, "", SalesOrder.SalesOrderID);
        }

        public IActionResult OnPostCreateDetail()
        {
            try
            {
                var InsertedCount = _salesOrderDetailService.Create(SalesOrderDetail);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return RedirectToPage("./Edit", new { id = SalesOrderDetail.SalesOrderID });

        }

        public IActionResult OnPostUpdateDetail()
        {
            try
            {
                var modifiedCount = _salesOrderDetailService.Update(SalesOrderDetail);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return RedirectToPage("./Edit", new { id = SalesOrderDetail.SalesOrderID });

        }

        public IActionResult OnGetDeleteDetail(int salesOrderID, int salesOrderDetailID)
        {
            try
            {
                var result = _genericServices.Get<SalesOrderDetail>().DeleteByKey(salesOrderDetailID);
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, salesOrderDetailID);
            }

            return GenJsonResult(1, "", salesOrderDetailID);
        }
    }
}