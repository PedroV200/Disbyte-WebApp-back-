namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// LISTED 23_8_2023 11:26AM. Todos los controllers devuelven el pais (nombre) añadido a la tabla
// via un endpoint. 

public class TerminalRepository : ITerminalRepository
{
 private readonly IConfiguration configuration;
    public TerminalRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Terminal entity)
    {
        var sql = $"INSERT INTO terminal (description,paisregion_id) VALUES ('{entity.description}',{entity.paisregion_id})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM terminal WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Terminal>> GetAllAsync()
    {
        var sql = "SELECT * FROM terminal";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Terminal>(sql);
        }
    }

    public async Task<IEnumerable<TerminalVista>> GetAllVistaAsync()
    {
        try
        {
            var sql = "select terminal.*,paisregion.description as pais from terminal inner join paisregion on terminal.paisregion_id=paisregion.id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<TerminalVista>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Terminal> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM terminal WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Terminal>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Terminal entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE terminal SET description = @description, paisregion_id= @paisregion_id WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}