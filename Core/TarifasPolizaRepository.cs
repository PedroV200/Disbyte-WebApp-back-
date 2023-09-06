namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// En conformidad con version 3B Ago15

public class TarifasPolizaRepository : ITarifasPolizaRepository
{
 private readonly IConfiguration configuration;
    public TarifasPolizaRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasPoliza entity)
    {
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd hh:mm:ss");
        //var sql = $"INSERT INTO tarifasdepositos (depo, contype, descarga, ingreso, totingreso, carga, armado, egreso, totegreso) VALUES ('{entity.depo}','{entity.contype}','{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totegreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        var sql = $@"INSERT INTO tarifaspolizas 
                (  
                    description,
                    poliza_id,
                    carga_id,
                    paisregion_id,
                    prima,
                    demora,
                    impuestos_internos,
                    sellos,
                    factor1,
                    factor2,
                    notas,
                    htimestamp
                    ) 
                            VALUES 
                                    ('{entity.description}',
                                      {entity.poliza_id},
                                      {entity.carga_id},
                                      {entity.paisregion_id},
                                     '{entity.prima.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.demora.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.impuestos_internos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',  
                                     '{entity.sellos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.factor1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.factor2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',                                  
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
        var sql = $"DELETE FROM tarifaspolizas WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasPoliza>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifaspolizas";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasPoliza>(sql);
        }
    }

    public async Task<IEnumerable<TarifasPolizaVista>> GetAllVistaAsync()
    {
        var sql = @"select tarifaspolizas.*, polizas.description as poliza, cargas.description as carga, paisregion.description as pais, paisregion.region as region
                        from tarifaspolizas
                        inner join polizas on polizas.id=tarifaspolizas.poliza_id
                        inner join cargas on cargas.id=tarifaspolizas.carga_id
                        inner join paisregion on paisregion.id=tarifaspolizas.paisregion_id";

        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasPolizaVista>(sql);
        }
    }

    public async Task<IEnumerable<TarifasPolizaVista>> GetAllVistaByDateAsync(string fecha)
    {
        var sql = $@"select tarifaspolizas.*, polizas.description as poliza, cargas.description as carga, paisregion.description as pais, paisregion.region as region
                        from tarifaspolizas
                        inner join polizas on polizas.id=tarifaspolizas.poliza_id
                        inner join cargas on cargas.id=tarifaspolizas.carga_id
                        inner join paisregion on paisregion.id=tarifaspolizas.paisregion_id
                        ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}')))";

        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasPolizaVista>(sql);
        }
    }

    public async Task<TarifasPoliza> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifaspolizas WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasPoliza>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(TarifasPoliza entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifaspolizas SET 

                            description = @description,
                            poliza_id = @poliza_id,
                            carga_id = @carga_id,
                            paisregion_id = @paisregion_id,
                            prima = @prima,
                            demora = @demora,
                            impuestos_internos = @impuestos_internos,
                            sellos = @sellos,
                            factor1 = @factor1,
                            factor2 = @factor2,
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
    public async Task<TarifasPoliza> GetByNearestDateAsync(string fecha, int paisregion_id)
    {
        // Si la fecha por la que consulto tiene una entrada en la base, el criterio es la que tiene la cotizacion
        // con la hora mas tarde.
        var sql = $@"select * from tarifaspolizas where paisregion_id={paisregion_id} AND htimestamp::date=date '{fecha}' order by htimestamp::time DESC LIMIT 1"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasPoliza>(sql);
            // Si la consulta no da resultado, significa que no hay registro para la fecha solicitada.
            // Hago un select basado en la diferencia de tiempo entre la fecha proporcionada y los que hay
            // y me quedo con la diferencia mas chica.
            if(result==null)
            {
                sql = $@"SELECT * FROM tarifaspolizas where paisregion_id={paisregion_id} ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}'))) LIMIT 1";
                result = await connection.QuerySingleOrDefaultAsync<TarifasPoliza>(sql);
            }
            return result;
        }
    }
}