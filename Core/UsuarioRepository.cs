namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

//LISTED 18_8_2023 16:56 

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IConfiguration configuration;
    public UsuarioRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Usuario entity)
    {
        try
        {
           var sql = $@"INSERT INTO usuarios
                (                
                username,
                email,
                password,
                role,
                paisregion_id
                        ) VALUES 
                ('{entity.username}',
                 '{entity.email}',
                 '{entity.password}',
                 '{entity.role}',                
                  {entity.paisregion_id}
                )";
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
            var sql = $"DELETE FROM usuarios WHERE userid = {id}";
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
    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        try
        {
            var sql = "SELECT * FROM usuarios";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<Usuario>(sql);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Usuario> GetByIdAsync(int id)
    {
        try
        {
            var sql = $"SELECT * FROM usuarios WHERE userid = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Usuario>(sql);
                return result;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<int> UpdateAsync(Usuario entity)
    {
        try
        {
             var sql = @"UPDATE usuarios SET 

                            userid = @userid,
                            username = @username,
                            email = @email,
                            password = @password,
                            role = @role,
                            paisregion_id = @paisregion_id
                
                             WHERE userid = @userid";
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