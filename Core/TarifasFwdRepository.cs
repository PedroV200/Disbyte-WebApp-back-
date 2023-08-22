namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// En conformidad con version 3B Ago15

public class TarifasFwdRepository : ITarifasFwdRepository
{
 private readonly IConfiguration configuration;
    public TarifasFwdRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasFwd entity)
    {
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd hh:mm:ss");
        //var sql = $"INSERT INTO tarifasdepositos (depo, contype, descarga, ingreso, totingreso, carga, armado, egreso, totegreso) VALUES ('{entity.depo}','{entity.contype}','{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totegreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        var sql = $@"INSERT INTO tarifasfwd 
                (  
                    
                    description,
                    fwdtte_id,
                    carga_id,
                    paisregion_id,
                    paisfwd_id,
                    costo,
                    costo_local,
                    gasto_otro1,
                    gasto_otro2,
                    notas,
                    htimestamp
                    ) 
                            VALUES 
                                    ('{entity.description}',
                                      {entity.fwdtte_id},
                                      {entity.carga_id},
                                      {entity.paisregion_id},
                                      {entity.paisfwd_id},
                                     '{entity.costo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.costo_local.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.gasto_otro1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',      
                                     '{entity.gasto_otro2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',                                                                 
                                     '{entity.notas}',
                                     '{tmpString}')";


        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM tarifasfwd WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasFwd>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasfwd";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasFwd>(sql);
        }
    }
    public async Task<TarifasFwd> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasfwd WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasFwd>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(TarifasFwd entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasFwd SET 
        
                        description = @description,
                        fwdtte_id = @fwdtte_id,
                        carga_id = @carga_id,
                        paisregion_id = @paisregion_id,
                        paisfwd_id = @paisfwd_id,
                        costo = @costo,
                        costo_local = @costo_local,
                        gasto_otro1 = @gasto_otro1,
                        gasto_otro2 = @gasto_otro2,
                        notas = @notas,
                        htimestamp = @htimestamp

                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

    // Consulta postgresql que devuelve el row con la cotizacion cdel dia, en el ultimo horario  ...
    //  o
    // La mas cercana en fecha si no existe una entrada para la fecha pasada como parametro.
    public async Task<TarifasFwd> GetByNearestDateAsync(string fecha)
    {
        // Si la fecha por la que consulto tiene una entrada en la base, el criterio es la que tiene la cotizacion
        // con la hora mas tarde.
        var sql = $@"select * from tarifasfwd where htimestamp::date=date '{fecha}' order by htimestamp::time DESC LIMIT 1"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasFwd>(sql);
            // Si la consulta no da resultado, significa que no hay registro para la fecha solicitada.
            // Hago un select basado en la diferencia de tiempo entre la fecha proporcionada y los que hay
            // y me quedo con la diferencia mas chica.
            if(result==null)
            {
                sql = $@"SELECT * FROM tarifasfwd ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}'))) LIMIT 1";
                result = await connection.QuerySingleOrDefaultAsync<TarifasFwd>(sql);
            }
            return result;
        }
    }
}