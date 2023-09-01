namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// En conformidad con version 3B Ago15

public class TarifasFleteRepository : ITarifasFleteRepository
{
 private readonly IConfiguration configuration;
    public TarifasFleteRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasFlete entity)
    {
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd hh:mm:ss");
        //var sql = $"INSERT INTO tarifasdepositos (depo, contype, descarga, ingreso, totingreso, carga, armado, egreso, totegreso) VALUES ('{entity.depo}','{entity.contype}','{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totegreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        var sql = $@"INSERT INTO tarifasflete 
                (  
                    description,
                    flete_id,
                    carga_id,
                    paisregion_id,
                    trucksemi_id,
                    flete_interno,
                    devolucion_vacio,
                    demora,
                    guarderia,
                    costo,
                    descarga_depo,
                    gasto_otro1,
                    gasto_otro2,
                    description_depo,
                    peso_minimo,
                    peso_maximo,
                    notas,
                    htimestamp
                    ) 
                            VALUES 
                                    ('{entity.description}',
                                      {entity.flete_id},
                                      {entity.carga_id},
                                      {entity.paisregion_id},
                                      {entity.trucksemi_id},
                                     '{entity.flete_interno.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.devolucion_vacio.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.demora.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',   
                                     '{entity.guarderia.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.costo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.descarga_depo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',   
                                     '{entity.gasto_otro1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.gasto_otro2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                                     '{entity.description_depo}',    
                                     '{entity.peso_minimo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.peso_maximo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',                                                   
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
        var sql = $"DELETE FROM tarifasflete WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasFlete>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasflete";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasFlete>(sql);
        }
    }

    public async Task<IEnumerable<TarifasFleteVista>> GetAllVistaAsync()
    {
        var sql = @"select tarifasflete.*, flete.description as flete, cargas.description as carga, paisregion.description as pais, paisregion.region as region, trucksemi.description as semi 
                    from tarifasflete 
                    inner join flete on tarifasflete.flete_id=flete.id 
                    inner join cargas on tarifasflete.carga_id=cargas.id 
                    inner join paisregion  on tarifasflete.paisregion_id=paisregion.id 
                    inner join trucksemi on tarifasflete.trucksemi_id=trucksemi.id";

        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasFleteVista>(sql);
        }
    }
    public async Task<TarifasFlete> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasflete WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasFlete>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(TarifasFlete entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasflete SET 

                        description = @description,
                        flete_id = @flete_id,
                        carga_id = @carga_id,
                        paisregion_id = @paisregion_id,
                        trucksemi_id = @trucksemi_id,
                        flete_interno = @flete_interno,
                        devolucion_vacio = @devolucion_vacio,
                        demora = @demora,
                        guarderia = @guarderia,
                        costo = @costo,
                        descarga_depo = @descarga_depo,
                        gasto_otro1 = @gasto_otro1,
                        gasto_otro2 = @gasto_otro2,
                        description_depo = @description_depo,
                        peso_minimo = @peso_minimo,
                        peso_maximo = @peso_maximo,
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
    public async Task<TarifasFlete> GetByNearestDateAsync(string fecha,int carga_id,int paisregion_id)
    {
        // Si la fecha por la que consulto tiene una entrada en la base, el criterio es la que tiene la cotizacion
        // con la hora mas tarde.
        var sql = $@"select * from tarifasflete where carga_id={carga_id} AND paisregion_id={paisregion_id} AND htimestamp::date=date '{fecha}' order by htimestamp::time DESC LIMIT 1"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasFlete>(sql);
            // Si la consulta no da resultado, significa que no hay registro para la fecha solicitada.
            // Hago un select basado en la diferencia de tiempo entre la fecha proporcionada y los que hay
            // y me quedo con la diferencia mas chica.
            if(result==null)
            {
                sql = $@"SELECT * FROM tarifasflete WHERE carga_id={carga_id} AND paisregion_id={paisregion_id} ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}'))) LIMIT 1";
                result = await connection.QuerySingleOrDefaultAsync<TarifasFlete>(sql);
            }
            return result;
        }
    }
}