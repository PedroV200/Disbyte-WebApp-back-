namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class FwdtteRepository : IFwdtteRepository
{
 private readonly IConfiguration configuration;
    public FwdtteRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Fwdtte entity)
    {
        var sql = $"INSERT INTO fwdtte (description,paisregion_id) VALUES ('{entity.description}',{entity.paisregion_id})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM fwdtte WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Fwdtte>> GetAllAsync()
    {
        var sql = "SELECT * FROM fwdtte";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Fwdtte>(sql);
        }
    }

    public async Task<IEnumerable<FwdtteVista>> GetAllVistaAsync()
    {
        try
        {
            var sql = "select fwdtte.*,paisregion.description as pais, paisregion.region as region from fwdtte inner join paisregion on fwdtte.paisregion_id=paisregion.id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<FwdtteVista>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Fwdtte> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM fwdtte WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Fwdtte>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Fwdtte entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE fwdtte SET description = @description, paisregion_id = @paisregion_id WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}