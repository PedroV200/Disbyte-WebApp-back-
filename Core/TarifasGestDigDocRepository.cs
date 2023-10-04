namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// En conformidad con version 3B Ago15

public class TarifasGestDigDocRepository : ITarifasGestDigDocRepository
{
 private readonly IConfiguration configuration;
    public TarifasGestDigDocRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasGestDigDoc entity)
    {
        entity.htimestamp=DateTime.Now;
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd HH:mm:ss");
        //var sql = $"INSERT INTO tarifasdepositos (depo, contype, descarga, ingreso, totingreso, carga, armado, egreso, totegreso) VALUES ('{entity.depo}','{entity.contype}','{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totegreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        var sql = $@"INSERT INTO tarifasgestdigdoc 
                (  
                    description,
                    gestdigdoc_id,
                    paisregion_id,
                    costo,
                    factor1,
                    gasto_otro1,
                    notas,
                    htimestamp    
                    ) 
                            VALUES 
                                    ('{entity.description}',
                                      {entity.gestdigdoc_id},
                                      {entity.paisregion_id},
                                     '{entity.costo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.factor1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.gasto_otro1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',                                    
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
        var sql = $"DELETE FROM tarifasgestdigdoc WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasGestDigDoc>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasgestdigdoc";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasGestDigDoc>(sql);
        }
    }

    public async Task<IEnumerable<TarifasGestDigDocVista>> GetAllVistaAsync()
    {
        var sql = @"select tarifasgestdigdoc.*, gestdigdoc.description as gestdigdoc, paisregion.description as pais, paisregion.region as region 
                        from tarifasgestdigdoc
                        inner join gestdigdoc on gestdigdoc.id=tarifasgestdigdoc.gestdigdoc_id
                        inner join paisregion on paisregion.id=tarifasgestdigdoc.paisregion_id";

        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasGestDigDocVista>(sql);
        }
    }

    public async Task<IEnumerable<TarifasGestDigDocVista>> GetAllVistaByDateAsync(string fecha)
    {
        var sql = $@"select tarifasgestdigdoc.*, gestdigdoc.description as gestdigdoc, paisregion.description as pais, paisregion.region as region 
                        from tarifasgestdigdoc
                        inner join gestdigdoc on gestdigdoc.id=tarifasgestdigdoc.gestdigdoc_id
                        inner join paisregion on paisregion.id=tarifasgestdigdoc.paisregion_id
                        ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}')))";

        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasGestDigDocVista>(sql);
        }
    }

    public async Task<TarifasGestDigDoc> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasgestdigdoc WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasGestDigDoc>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(TarifasGestDigDoc entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasgestdigdoc SET 

                            description = @description,
                            gestdigdoc_id = @gestdigdoc_id,
                            paisregion_id = @paisregion_id,
                            costo = @costo,
                            factor1 = @factor1,
                            gasto_otro1 = @gasto_otro1,
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
    public async Task<TarifasGestDigDoc> GetByNearestDateAsync(string fecha, int paisregion_id)
    {
        // Si la fecha por la que consulto tiene una entrada en la base, el criterio es la que tiene la cotizacion
        // con la hora mas tarde.
        var sql = $@"select * from tarifasgestdigdoc where paisregion_id={paisregion_id} AND htimestamp::date=date '{fecha}' order by htimestamp::time DESC LIMIT 1"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasGestDigDoc>(sql);
            // Si la consulta no da resultado, significa que no hay registro para la fecha solicitada.
            // Hago un select basado en la diferencia de tiempo entre la fecha proporcionada y los que hay
            // y me quedo con la diferencia mas chica.
            if(result==null)
            {
                sql = $@"SELECT * FROM tarifasgestdigdoc where paisregion_id={paisregion_id} ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}'))) LIMIT 1";
                result = await connection.QuerySingleOrDefaultAsync<TarifasGestDigDoc>(sql);
            }
            return result;
        }
    }
}