using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IImportService : IGenericService<TableModel>
{
   public void ImportNCM_Mex(string fileName);
   public void ImportProductos(string fileName);
}