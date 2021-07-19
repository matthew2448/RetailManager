using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface ISaleData
    {
        List<SaleReportModel> getSaleReport();
        void SaveSale(SaleModel saleInfo, string cashierID);
    }
}