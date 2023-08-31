namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// LISTED 17_8_2023 12:08PM (agrega paisregion_id  y test post/put)

public class BancoRepository : IBancoRepository
{
 private readonly IConfiguration configuration;
    public BancoRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Banco entity)
    {
        try
        {
            var sql = $"INSERT INTO banco (description,paisregion_id) VALUES ('{entity.description}',{entity.paisregion_id})";
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
        var sql = $"DELETE FROM banco WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
        }catch (Exception)
        {
            return -1;
        }
    }
    public async Task<IEnumerable<Banco>> GetAllAsync()
    {
        try
        {
            var sql = "SELECT * FROM banco";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<Banco>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<IEnumerable<BancoVista>> GetAllVistaAsync()
    {
        try
        {
            var sql = "select banco.*,paisregion.description as pais,paisregion.region as region from banco inner join paisregion on banco.paisregion_id=paisregion.id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<BancoVista>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Banco> GetByIdAsync(int id)
    {
        try
        {
            var sql = $"SELECT * FROM banco WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Banco>(sql);
                return result;
            }
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<Banco>> GetByPaisAsync(int paisreg)
    {
        try
        {
            var sql = $"SELECT * FROM banco WHERE paisregion_id = {paisreg}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<Banco>(sql);
                return result;
            }
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> UpdateAsync(Banco entity)
    {
        try
        {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE banco SET description = @description, paisregion_id = @paisregion_id WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
        }catch(Exception)
        {
            return -1;
        }
    }
}