namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class CargaRepository : ICargaRepository
{
 private readonly IConfiguration configuration;
    public CargaRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Carga entity)
    {
        var sql = $"INSERT INTO cargas (description, weight, volume) VALUES ('{entity.description}','{entity.weight.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.volume.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM cargas WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<Carga>> GetAllAsync()
    {
        var sql = "SELECT * FROM cargas";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Carga>(sql);
        }
    }
    public async Task<Carga> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM cargas WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Carga>(sql);
            return result;
        }
    }
        public async Task<Carga> GetByTipoContAsync(string type)
    {
        var sql = $"SELECT * FROM cargas WHERE description = '{type}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Carga>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Carga entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE cargas SET 
                    description = @description, 
                    weight = @weight, 
                    volume = @volume
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}