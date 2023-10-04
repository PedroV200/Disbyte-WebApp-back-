namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// En conformidad con version 3B Ago15

public class TarifasDepositoRepository : ITarifasDepositoRepository
{
 private readonly IConfiguration configuration;
    public TarifasDepositoRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasDeposito entity)
    {
        entity.htimestamp=DateTime.Now;
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd HH:mm:ss");
        //var sql = $"INSERT INTO tarifasdepositos (depo, contype, descarga, ingreso, totingreso, carga, armado, egreso, totegreso) VALUES ('{entity.depo}','{entity.contype}','{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totegreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        var sql = $@"INSERT INTO tarifasdepositos 
                (  
                    description,
                    depositos_id,
                    carga_id,
                    paisregion_id,
                    trucksemi_id,
                    descarga,
                    ingreso,
                    total_ingreso,
                    carga,
                    armado,
                    egreso,
                    total_egreso,
                    gasto_otro1,
                    gasto_otro2,
                    notas,
                    htimestamp) 
                            VALUES 
                                    ('{entity.description}',
                                      {entity.depositos_id},
                                      {entity.carga_id},
                                      {entity.paisregion_id},
                                      {entity.trucksemi_id},
                                     '{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.total_ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.total_egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
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
        var sql = $"DELETE FROM tarifasdepositos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasDeposito>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasdepositos";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasDeposito>(sql);
        }
    }

    public async Task<IEnumerable<TarifasDepositoVista>> GetAllVistaAsync()
    {
        var sql = @"select tarifasdepositos.*, depositos.description as deposito, cargas.description as freight, paisregion.description as pais, paisregion.region as region, trucksemi.description as semi 
                    from tarifasdepositos 
                    inner join depositos on tarifasdepositos.depositos_id=depositos.id 
                    inner join cargas on tarifasdepositos.carga_id=cargas.id 
                    inner join paisregion  on tarifasdepositos.paisregion_id=paisregion.id 
                    inner join trucksemi on tarifasdepositos.trucksemi_id=trucksemi.id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasDepositoVista>(sql);
        }
    }

    public async Task<IEnumerable<TarifasDepositoVista>> GetAllVistaByDateAsync(string fecha)
    {
        var sql = $@"select tarifasdepositos.*, depositos.description as deposito, cargas.description as freight, paisregion.description as pais, paisregion.region as region, trucksemi.description as semi 
                    from tarifasdepositos 
                    inner join depositos on tarifasdepositos.depositos_id=depositos.id 
                    inner join cargas on tarifasdepositos.carga_id=cargas.id 
                    inner join paisregion  on tarifasdepositos.paisregion_id=paisregion.id 
                    inner join trucksemi on tarifasdepositos.trucksemi_id=trucksemi.id
                    ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}')))";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasDepositoVista>(sql);
        }
    }

    public async Task<TarifasDeposito> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasdepositos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasDeposito>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(TarifasDeposito entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasdepositos SET 
                            description = @description,
                            depositos_id = @depositos_id,
                            carga_id = @carga_id,
                            paisregion_id = @paisregion_id,
                            trucksemi_id = @trucksemi_id,
                            descarga = @descarga,
                            ingreso = @ingreso,
                            total_ingreso = @total_ingreso,
                            carga = @carga,
                            armado = @armado,
                            egreso = @egreso,
                            total_egreso = @total_egreso,
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
    public async Task<TarifasDeposito> GetByNearestDateAsync(string fecha, int carga_id, int paisregion_id)
    {
        // Si la fecha por la que consulto tiene una entrada en la base, el criterio es la que tiene la cotizacion
        // con la hora mas tarde.
        var sql = $@"select * from tarifasdepositos where paisregion_id={paisregion_id} AND carga_id={carga_id} AND htimestamp::date=date '{fecha}' order by htimestamp::time DESC LIMIT 1"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasDeposito>(sql);
            // Si la consulta no da resultado, significa que no hay registro para la fecha solicitada.
            // Hago un select basado en la diferencia de tiempo entre la fecha proporcionada y los que hay
            // y me quedo con la diferencia mas chica.
            if(result==null)
            {
                sql = $@"SELECT * FROM tarifasdepositos where paisregion_id={paisregion_id} AND carga_id={carga_id} ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}'))) LIMIT 1";
                result = await connection.QuerySingleOrDefaultAsync<TarifasDeposito>(sql);
            }
            return result;
        }
    }
}