using Appeon.MvcModelMapperDemo.Models;
using Appeon.SnapObjectsDemo.Service.Models;
using Appeon.SnapObjectsDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Appeon.MvcModelMapperDemo.Pages.SalesOrders
{
    public class IndexModel : BasePageModel
    {
        private readonly ISalesOrderService _salesOrderService;

        public IndexModel(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        public IList<SalesOrder> SalesOrders { get; set; }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? StartOrderDate { get; set; } = new DateTime(2011, 1, 1).Date;

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? EndOrderDate { get; set; } = new DateTime(2012, 1, 31).Date;

        [BindProperty(SupportsGet = true)]
        public int? CustomerID { get; set; }

        public void OnGetAsync()
        {

        }

        public async Task<IActionResult> OnGetDelete(string ids)
        {
            try
            {
                string[] idArr = ids.Split(",");
                foreach (var id in idArr)
                {
                    var result = await _salesOrderService.DeleteByKeyAsync(new object[] { id });
                }
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, 0);
            }

            return GenJsonResult(1, "", null);
        }

        public async Task<IActionResult> OnGetDeleteById(string id)
        {
            try
            {
                var result = await _salesOrderService.DeleteByKeyAsync(new object[] { id });
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, 0);
            }

            return GenJsonResult(1, "", null);
        }

        public async Task<ActionResult> OnPostSearch(DataTable dt)
        {
            try
            {
                int pageSize = dt.pageSize ?? 10;
                int pageIndex = dt.pageIndex;

                //query data by page
                Page<SalesOrder> page = await _salesOrderService
                    .LoadByPageAsync(pageIndex, pageSize, false, new object[] { CustomerID ?? 0, StartOrderDate, EndOrderDate });

                SalesOrders = page.Items;
                dt.recordsTotal = page.TotalItems;
                dt.recordsFiltered = page.TotalItems;
                dt.data = SalesOrders.ToList();

                return new JsonResult(JsonConvert.SerializeObject(dt));
            }
            catch (Exception e)
            {
                return GenJsonResult(-1, e.Message, 0);
            }
        }
    }
}