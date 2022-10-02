using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IProductService
    {
        List<ProductModel> Get();
        List<ProductModel> GetByIds(List<string> idList);
    }
}
