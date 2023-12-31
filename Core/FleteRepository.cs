namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class FleteRepository : IFleteRepository
{
 private readonly IConfiguration configuration;
    public FleteRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Flete entity)
    {
        var sql = $"INSERT INTO flete (description,paisregion_id) VALUES ('{entity.description}',{entity.paisregion_id})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM flete WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Flete>> GetAllAsync()
    {
        var sql = "SELECT * FROM flete";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Flete>(sql);
        }
    }

    public async Task<IEnumerable<FleteVista>> GetAllVistaAsync()
    {
        try
        {
            var sql = "select flete.*,paisregion.description as pais,paisregion.region as region from flete inner join paisregion on flete.paisregion_id=paisregion.id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<FleteVista>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Flete> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM flete WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Flete>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Flete entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE flete SET description = @description, paisregion_id = @paisregion_id WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}