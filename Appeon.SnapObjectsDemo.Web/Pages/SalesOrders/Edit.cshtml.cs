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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                await RetrieveDddw();

                SalesOrder = await _salesOrderService.RetrieveByKeyAsync(true, new object[] { id });

                if (SalesOrder != null)
                {
                    SalesOrderDetail = SalesOrder.OrderDetails?.FirstOrDefault();

                    await RetrieveDddw(SalesOrder.CustomerID);
                }

                return Page();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        public async Task<JsonResult> OnGetRetrieveProduct(int id)
        {
            var result = new Dictionary<string, object>()
            {
                { "code", 1},

                { "product", await _genericServices.Get<DdOrderProduct>().RetrieveAsync(false, new object[]{id }) }
            };

            return new JsonResult(result);
        }

        private async Task RetrieveDddw()
        {
            await RetrieveCustomers();

            await RetrieveSalesPersons();

            await RetrieveShipMethods();

            await RetrieveOrderProducts();
        }

        public async Task RetrieveDddw(int customerId)
        {
            await RetrieveCreditcards(customerId);

            await RetrieveCustomerAddresses(customerId);
        }

        private async Task RetrieveCustomers()
        {
            var ddCustomers = await _genericServices.Get<DdCustomer>().RetrieveAsync(false, default);

            Customers = new SelectList(ddCustomers, "Customer_Customerid", "Fullname");

            CustomerMaps = ddCustomers.ToDictionary(x => x.Customer_Customerid, x => x.Fullname);
        }

        private async Task RetrieveSalesPersons()
        {
            var ddSalesPersons = await _genericServices.Get<DdSalesPerson>().RetrieveAsync(false, default);

            SalesPersons = new SelectList(ddSalesPersons, "Salesperson_Businessentityid", "Fullname");
        }

        private async Task RetrieveShipMethods()
        {
            var ddShipMethods = await _genericServices.Get<DdShipMethod>().RetrieveAsync(false, default);

            ShipMethods = new SelectList(ddShipMethods, "Shipmethodid", "Name");
        }

        private async Task RetrieveCreditcards(int customerId)
        {
            var ddCreditcards = await _genericServices.Get<DdCreditcard>()
                                                .RetrieveAsync(false, new object[] { customerId });

            Creditcards = new SelectList(ddCreditcards, "Creditcard_Creditcardid", "Creditcard_CardNumber");
        }

        private async Task RetrieveCustomerAddresses(int customerId)
        {
            var ddCustomerAddresses = await _genericServices.Get<DdCustomerAddress>()
                                                      .RetrieveAsync(false, new object[] { customerId });

            CustomerAddresses = new SelectList(
                ddCustomerAddresses, "Businessentityaddress_Addressid", "Address_Addressline1");
        }

        private async Task RetrieveOrderProducts()
        {
            // 0: retrieve all products
            var ddOrderProducts = await _genericServices.Get<DdOrderProduct>()
                                                  .RetrieveAsync(false, new object[] { 0 });

            OrderProducts = new SelectList(
                ddOrderProducts, "Product_Productid", "Product_Name");

            OrderProductMaps = ddOrderProducts.ToDictionary(x => x.Product_Productid, x => x.Product_Name);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ConvertData(SalesOrder);
                SalesOrder.ModifiedDate = DateTime.Now;
                var modifiedCount = await _salesOrderService.UpdateAsync(SalesOrder);
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, SalesOrder.SalesOrderID);
            }

            return GenJsonResult(1, "", SalesOrder.SalesOrderID);
        }

        public async Task<IActionResult> OnPostCreateDetail()
        {
            try
            {
                var InsertedCount = await _salesOrderDetailService.CreateAsync(SalesOrderDetail);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return RedirectToPage("./Edit", new { id = SalesOrderDetail.SalesOrderID });

        }

        public async Task<IActionResult> OnPostUpdateDetail()
        {
            try
            {
                SalesOrderDetail.ModifiedDate = DateTime.Now;
                var modifiedCount = await _salesOrderDetailService.UpdateAsync(SalesOrderDetail);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return RedirectToPage("./Edit", new { id = SalesOrderDetail.SalesOrderID });

        }

        public async Task<IActionResult> OnGetDeleteDetail(int salesOrderID, int salesOrderDetailID)
        {
            try
            {
                var result = await _genericServices.Get<SalesOrderDetail>().DeleteByKeyAsync(new object[] { salesOrderDetailID });
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, salesOrderDetailID);
            }

            return GenJsonResult(1, "", salesOrderDetailID);
        }

    }
}