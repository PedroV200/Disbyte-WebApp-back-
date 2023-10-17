namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// LISTED 17_10_2023 13:54PM CREACION entidad producto basado en template de datos pasador por PE
// donde se importaran 1770 productos con SKU a una tabla homonima. 

public class ProductoRepository : IProductoRepository
{
 private readonly IConfiguration configuration;
    public ProductoRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Producto entity)
    {
       var sql = $@"INSERT INTO productos 
                (                
	                codigo, 
	                name, 
	                alto, 
	                largo, 
	                peso, 
	                profundidad,
    	            tipodeproducto,
	                volumen, 
	                unidadesporbulto,
	                categoriacompleta
                ) 
                            VALUES 
                                    ('{entity.codigo}',
                                     '{entity.name}',                                      
                                     '{entity.alto.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.largo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.peso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',    
                                     '{entity.profundidad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',    
                                     '{entity.tipodeproducto}',
                                     '{entity.volumen.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                      {entity.unidadesporbulto},                       
                                     '{entity.categoriacompleta}'
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
        var sql = $"DELETE FROM productos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Producto>> GetAllAsync()
    {
        var sql = "SELECT * FROM productos";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Producto>(sql);
        }
    }

    
    public async Task<Producto> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM productos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Producto>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Producto entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE productos SET 
                                
                    codigo = @codigo, 
	                name = @name, 
	                alto = @alto, 
	                largo = @largo, 
	                peso = @peso, 
	                profundidad = @profundidad,
    	            tipodeproducto = @tipodeproducto,
	                volumen = @volumen, 
	                unidadesporbulto = @unidadesporbulto,
	                categoriacompleta = @categoriacompleta   

                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}