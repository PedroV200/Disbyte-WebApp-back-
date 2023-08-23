namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// LISTED 17_8_2023 12:11PM (agrega paisregion_id y verifica put/post)

public class CustodiaRepository : ICustodiaRepository
{
 private readonly IConfiguration configuration;
    public CustodiaRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Custodia entity)
    {
        var sql = $"INSERT INTO custodia (description,paisregion_id) VALUES ('{entity.description}',{entity.paisregion_id})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM custodia WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Custodia>> GetAllAsync()
    {
        var sql = "SELECT * FROM custodia";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Custodia>(sql);
        }
    }

    public async Task<IEnumerable<CustodiaVista>> GetAllPaisAsync()
    {
        try
        {
            var sql = "select custodia.*,paisregion.description as pais from custodia inner join paisregion on custodia.paisregion_id=paisregion.id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<CustodiaVista>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Custodia> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM custodia WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Custodia>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Custodia entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE custodia SET description = @description, paisregion_id = @paisregion_id WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}