﻿using System;
using System.Collections.Generic;
using System.Linq;
using Appeon.SnapObjectsDemo.Service.Models;
using Appeon.SnapObjectsDemo.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Appeon.MvcModelMapperDemo.Pages
{
    public class IndexModel : PageModel
    {
        public int orderTotalNum;
        public int orderNewNum;
        public int orderPendingNum;
        public int ordershipedNum;
        public int orderCancelNum;

        private readonly IOrderReportService _reportService;

        public IndexModel(IOrderReportService reportService)
        {
            _reportService = reportService;
        }

        public CategorySalesReportByYear categoryReportByYear { get; set; }

        public ProductCategorySalesReport productCategorySalesReport { get; set; }

        public IList<ProductSalesReport> productSalesReport { get; set; }

        public Dictionary<string, int> totalData { get; set; }

        public string loginName { get; set; }

        public void OnGet()
        {
            queryPieReport();
            queryTotalReport();
            queryBarReportByYear();
        }

        private void queryTotalReport()
        {
            totalData = _reportService.RetrieveSalesOrderTotalReport();
            //转换json
            String Json_totalData = Newtonsoft.Json.JsonConvert.SerializeObject(totalData);
            categoryReportByYear.Json_totalData = Json_totalData;
        }

        private void queryPieReport()
        {
            String curDate = "2013-01-01";
            var curYear = DateTime.Parse(curDate).Year.ToString();
            var lastYear = DateTime.Parse(curDate).AddYears(-1).Year.ToString();
            var master = new CategorySalesReportByYear();
            categoryReportByYear = _reportService.RetrieveCategorySalesReportByYear(master, curYear, lastYear);
            //转换json
            String categorys = JsonConvert.SerializeObject(categoryReportByYear.SalesReportByCategory
                                                            .Select(x => x.ProductCategoryName));

            String categorysData = JsonConvert.SerializeObject(categoryReportByYear.SalesReportByCategory
                                                            .Select(x => new
                                                            {
                                                                name = x.ProductCategoryName, value = x.TotalSalesqty
                                                            }));
            categoryReportByYear.Json_Categorys = categorys;
            categoryReportByYear.Json_categorysData = categorysData;
        }

        private void queryBarReportByYear()
        {
            string salesYear = "2013";
            object[] yearMonth = new object[12];
            object[] resultMonth = new object[12];

            //yearMonth[0] = subCategoryId;
            for (int month = 0; month < 12; month++)
            {
                yearMonth[month] = salesYear + string.Format("{0:00}", month + 1);
                resultMonth[month] = string.Format("{0:00}", month + 1) + "/" + salesYear;
            }

            var master = new ProductCategorySalesReport();
            productCategorySalesReport = _reportService.RetrieveProductCategorySalesReport(master, yearMonth);
            ConvertDataForReport(productCategorySalesReport, resultMonth);
        }

        /// <summary>
        /// Convert the database data to report data
        /// </summary>
        /// <param name="productCategorySalesReport"></param>
        /// <param name="yearMonth"></param>
        private void ConvertDataForReport(ProductCategorySalesReport productCategorySalesReport, object[] yearMonth)
        {
            List<string> ProCategoryName = productCategorySalesReport.OrderReportMonth1
                                                                     .Select(x => x.ProductCategoryName).ToList();

            List<int> salesQtys = null;
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            foreach (var name in ProCategoryName)
            {
                salesQtys = new List<int>();
                salesQtys.Add(productCategorySalesReport.OrderReportMonth1.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth2.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth3.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth4.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth5.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth6.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth7.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth8.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth9.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth10.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth11.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                salesQtys.Add(productCategorySalesReport.OrderReportMonth12.Where(x => x.ProductCategoryName.Equals(name)).FirstOrDefault().TotalSalesqty);
                result.Add(name, salesQtys);
            }

            String proCat =JsonConvert.SerializeObject(ProCategoryName);
            String proCatQty =JsonConvert.SerializeObject(result
                                                            .Select(x => new
                                                            {
                                                                name = x.Key, type = "bar", data = x.Value
                                                            }));
            categoryReportByYear.Json_ProductSaleMonth = JsonConvert.SerializeObject(yearMonth);
            categoryReportByYear.Json_ProductCategory = proCat;
            categoryReportByYear.Json_ProductSaleSqty = proCatQty;
        }
    }
}
