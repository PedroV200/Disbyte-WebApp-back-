using Dapper;
using Npgsql;
using WebApiSample.Infrastructure;
using WebApiSample.Models;
namespace WebApiSample.Core
{// LISTED 4_8_2023 17:10
    public class PaisRegionRepository : IPaisRegionRepository
    {
        private readonly IConfiguration configuration;
        public PaisRegionRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<int> AddAsync(PaisRegion entity)
        {
            var sql = $"INSERT INTO paisregion (description,region,moneda,puerto) VALUES ('{entity.description}','{entity.region}','{entity.moneda}','{entity.puerto}')";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        public async Task<int> DeleteAsync(int id)
        {
            var sql = $"DELETE FROM paisregion WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var result = await connection.ExecuteAsync(sql);

                return result;
            }
        }
        public async Task<IEnumerable<PaisRegion>> GetAllAsync()
        {
            var sql = "SELECT * FROM paisregion";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<PaisRegion>(sql);
            }
        }
        public async Task<PaisRegion> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM paisregion WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<PaisRegion>(sql);
                return result;
            }
        }
        public async Task<int> UpdateAsync(PaisRegion entity)
        {
            //entity.ModifiedOn=DateTime.Now;
            //entity.ModifiedOn=DateTime.Now;
            //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
            var sql = @"UPDATE paisregion SET description = @description, region=@region, moneda=@moneda, puerto=@puerto WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
