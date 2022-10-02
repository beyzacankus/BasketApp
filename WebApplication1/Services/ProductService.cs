using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductService(
        IOptions<FarmasiDatabaseSettings> farmasiDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                farmasiDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                farmasiDatabaseSettings.Value.DatabaseName);

            _productCollection = mongoDatabase.GetCollection<Product>(
                farmasiDatabaseSettings.Value.ProductCollectionName);
        }
        public List<ProductModel> Get()
        {
            return _productCollection.Find(_ => true).ToList().Select(x => new ProductModel()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price
            }).ToList();
        }
        public List<ProductModel> GetByIds(List<string> idList)
        {
            return idList.Any() ? _productCollection.Find(x => idList.Contains(x.Id)).ToList().Select(x => new ProductModel()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price
            }).ToList() : new List<ProductModel>();
        }
    }
}
