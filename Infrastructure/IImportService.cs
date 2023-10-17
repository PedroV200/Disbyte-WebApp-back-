using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IImportService : IGenericService<TableModel>
{
   public void fopen(string fileName);
}