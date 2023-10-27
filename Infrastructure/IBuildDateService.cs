using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IBuilDateService : IGenericService<string>
{
        public string getBuildDate();
}