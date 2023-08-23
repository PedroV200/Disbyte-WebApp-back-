using WebApiSample.Models;

namespace WebApiSample.Infrastructure
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        public Task<IEnumerable<UsuarioVista>> GetAllVistaAsync();
    }
}
