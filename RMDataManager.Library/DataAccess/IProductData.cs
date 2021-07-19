using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public interface IProductData
    {
        ProductModel GetProductById(int productid);
        List<ProductModel> GetProducts();
    }
}