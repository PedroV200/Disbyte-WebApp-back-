using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class TruckSemiRepository : ITruckSemiRepository
{
        private readonly IConfiguration configuration;
        public TruckSemiRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<int> AddAsync(TruckSemi entity)
        {
            var sql = $@"INSERT INTO trucksemi 
        (
                description,
                pesomin,
                pesomax,
                largo,
                costindex1,
                costindex2,
                paisregion_id   
                        ) VALUES 
                ('{entity.description}',
                '{entity.pesomin.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.pesomax.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.largo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costindex1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costindex2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.paisregion_id}
                )";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        public async Task<int> DeleteAsync(int id)
        {
            var sql = $"DELETE FROM trucksemi WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var result = await connection.ExecuteAsync(sql);

                return result;
            }
        }
        public async Task<IEnumerable<TruckSemi>> GetAllAsync()
        {
            var sql = "SELECT * FROM trucksemi";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<TruckSemi>(sql);
            }
        }
        public async Task<TruckSemi> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM trucksemi WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TruckSemi>(sql);
                return result;
            }
        }
        public async Task<int> UpdateAsync(TruckSemi entity)
        {
            //entity.ModifiedOn=DateTime.Now;
            //entity.ModifiedOn=DateTime.Now;
            //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
          var sql = @"UPDATE trucksemi SET 

                            description = @description,
                            pesomin = @pesomin,
                            pesomax = @pesomax,
                            largo = @largo,
                            costindex1 = @costindex1,
                            costindex2 = @costindex2,
                            paisregion_id = @paisregion_id  
                
                             WHERE Id = @Id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
