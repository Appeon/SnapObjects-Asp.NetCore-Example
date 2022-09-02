using Appeon.SnapObjectsDemo.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace Appeon.MvcModelMapperDemo.Models
{
    public class BasePageModel : PageModel
    {
        /// <summary>
        /// Return to the unified json format
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected JsonResult GenJsonResult(int code, string message, int? id)
        {
            var result = new Dictionary<string, object>()
                {
                    { "code", code},
                    { "message",message},
                    { "id",id}
                };

            return new JsonResult(result);
        }
        /// <summary>
        /// convert date format
        /// </summary>
        /// <param name="SalesOrder"></param>
        protected void ConvertData(SalesOrder SalesOrder)
        {
            string orderDate = Request.Form["SalesOrder.OrderDate"];
            string dueDate = Request.Form["SalesOrder.DueDate"];
            string shipDate = Request.Form["SalesOrder.ShipDate"];

            if (!string.IsNullOrEmpty(orderDate))
            {
                if (!orderDate.ToString().Equals("0001/1/1 0:00:00"))
                {
                    try
                    {
                        DateTime dt;
                        DateTime.TryParseExact(orderDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt);
                        SalesOrder.OrderDate = dt;
                    }
                    catch (Exception)
                    { }
                }
                else
                    SalesOrder.OrderDate = DateTime.Now;
            }
            if (!string.IsNullOrEmpty(dueDate))
            {
                if (!dueDate.ToString().Equals("0001/1/1 0:00:00"))
                {
                    try
                    {
                        SalesOrder.DueDate = DateTime.ParseExact(dueDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                    }
                    catch (Exception)
                    { }
                }
                else
                    SalesOrder.DueDate = DateTime.Now;
            }
            if (!string.IsNullOrEmpty(shipDate) && SalesOrder.ShipDate == null)
            {
                if (!shipDate.ToString().Equals("0001/1/1 0:00:00"))
                {
                    try
                    {
                        SalesOrder.ShipDate = (DateTime?)DateTime.ParseExact(shipDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None).AddDays(1);
                    }
                    catch (Exception)
                    { }
                }
                else
                    SalesOrder.ShipDate = DateTime.Now;
            }

            if (SalesOrder.OrderDate.ToString().IndexOf("0001") >= 0)
                SalesOrder.OrderDate = DateTime.Now;

            if (SalesOrder.DueDate.ToString().IndexOf("0001") >= 0)
                SalesOrder.DueDate = DateTime.Now;

            if (SalesOrder.ShipDate.ToString().IndexOf("0001") >= 0)
                SalesOrder.ShipDate = DateTime.Now.AddDays(1);
        }
    }
}
