using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class ProductData
    {
        private readonly IConfiguration _config;
        public ProductData(IConfiguration config)
        {
            _config = config;
        }
        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var output = sql.LoadData <ProductModel,
                dynamic>("dbo.spProduct_GetAll", new { }, "RmData");
            
            return output;
        }

        public ProductModel GetProductById(int productid)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var output = sql.LoadData<ProductModel,
                dynamic>("dbo.spProduct_GetById", new { Id = productid}, "RmData").FirstOrDefault();

            return output;
        }
    }
}
