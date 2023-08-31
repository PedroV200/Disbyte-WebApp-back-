namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class DespachanteRepository : IDespachanteRepository
{
    private readonly IConfiguration configuration;
    public DespachanteRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Despachante entity)
    {
        try
        {
            var sql = $"INSERT INTO despachantes (description,paisregion_id) VALUES ('{entity.description}',{entity.paisregion_id})";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        try
        {
            var sql = $"DELETE FROM despachantes WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var result = await connection.ExecuteAsync(sql);

                return result;
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }
    public async Task<IEnumerable<Despachante>> GetAllAsync()
    {
        try
        {
            var sql = "SELECT * FROM despachantes";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<Despachante>(sql);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<DespachanteVista>> GetAllVistaAsync()
    {
        try
        {
            var sql = "select despachantes.*,paisregion.description as pais,paisregion.region as region from despachantes inner join paisregion on despachantes.paisregion_id=paisregion.id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<DespachanteVista>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Despachante> GetByIdAsync(int id)
    {
        try
        {
            var sql = $"SELECT * FROM despachantes WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Despachante>(sql);
                return result;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<int> UpdateAsync(Despachante entity)
    {
        try
        {
            //entity.ModifiedOn=DateTime.Now;
            //entity.ModifiedOn=DateTime.Now;
            //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
            var sql = @"UPDATE despachantes SET description = @description, paisregion_id = @paisregion_id WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }
}