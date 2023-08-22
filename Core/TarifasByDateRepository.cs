namespace WebApiSample.Core; 

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;  

// LISTED 11_8_2023 09:51     

public class TarifasByDateRepository : ITarifasByDateRepository
{
    private readonly IConfiguration configuration;
    public TarifasByDateRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    } 
    public async Task<int> AddAsync(TarifasByDate entity)
    {
        string tmpString=entity.hTimeStamp.ToString("yyyy-MM-dd hh:mm:ss");
        var sql = $@"INSERT INTO tarifasbydate 
        (
                description,
                paisregionid, 
                freight_type, 
                terminal_provee,
                terminal_gastofijo, 
                terminal_gastovariable,
                fwdtte_provee,
                freight_fwdfrom, 
                freight_cost, 
                freight_gastos1,   
                freight_gastos2, 
                depo_provee, 
                depo_descarga, 
                depo_ingreso, 
                depo_totingreso,
                depo_carga,
                depo_armado,
                depo_egreso,
                depo_total_egreso,
                flete_provee,
                fleteint, 
                flete_devacio, 
                flete_demora, 
                flete_guarderia,
                flete_totgastos, 
                flete_trucksemiid,
                poliza_provee, 
                poliza_prima, 
                poliza_demora,
                poliza_impint, 
                poliza_sellos, 
                despachante_provee,
                despa_fijo,
                despa_variable,
                despa_clasificacion,
                despa_consultoria,
                despa_total_gastos,
                custodia_provee, 
                custodia_fact1,
                custodia_fact2, 
                custodia_gastos,
                banco_provee,
                banco_fact1,
                banco_fact2,
                banco_gastos,
                gestdigdoc_provee,
                gestdigdoc_fact1,
                gestdigdoc_fact2,
                gestdigdoc_gastos,
                htimestamp 
                        ) VALUES 
                ('{entity.description}',
                 {entity.paisregionid},
                 {entity.freight_type},
                 {entity.terminal_provee},
                '{entity.terminal_gastoFijo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.terminal_gastoVariable.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.fwdtte_provee},
                 {entity.freight_fwdfrom},
                '{entity.freight_cost.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.freight_gastos1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.freight_gastos2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.depo_provee},
                '{entity.depo_descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.depo_ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.depo_totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.depo_carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.depo_armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.depo_egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.depo_total_egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.flete_provee},
                '{entity.fleteint.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.flete_devacio.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.flete_demora.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.flete_guarderia.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.flete_totgastos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.flete_trucksemiid},
                 {entity.poliza_provee},
                '{entity.poliza_prima.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.poliza_demora.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.poliza_impint.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.poliza_sellos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.despachante_provee},
                '{entity.despa_fijo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.despa_variable.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.despa_clasificacion.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.despa_consultoria.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.despa_total_gastos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.custodia_provee},
                '{entity.custodia_fact1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.custodia_fact2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.custodia_gastos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.banco_provee},
                '{entity.banco_fact1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.banco_fact2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.banco_gastos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.gestdigdoc_provee},
                '{entity.gestdigdoc_fact1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gestdigdoc_fact2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gestdigdoc_gastos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{tmpString}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int Id)
    {
        var sql = $"DELETE FROM tarifasbydate WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasByDate>>GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasbydate";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasByDate>(sql);
        }
    }


    public async Task<TarifasByDate> GetByIdAsync(int Id)
    {
        var sql = $"SELECT * FROM tarifasbydate WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasByDate>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(TarifasByDate entity)
    {
        
        var sql = @"UPDATE tarifasbydate SET 
                        description = @description, 
                        paisregionid = @paisregionid,
                        freight_type = @freight_type, 
                        terminal_provee = @terminal_provee,
                        terminal_gastofijo = @terminal_gastofijo, 
                        terminal_gastovariable = @terminal_gastovariable,
                        fwdtte_provee = @fwdtte_provee,
                        freight_fwdfrom = @freight_fwdfrom, 
                        freight_cost = @freight_cost, 
                        freight_gastos1 = @freight_gastos1,   
                        freight_gastos2 = @freight_gastos2, 
                        depo_provee = @depo_provee, 
                        depo_descarga = @depo_descarga, 
                        depo_ingreso = @depo_ingreso, 
                        depo_totingreso = @depo_totingreso,
                        depo_carga = @depo_carga,
                        depo_armado = @depo_armado,
                        depo_egreso = @depo_egreso,
                        depo_total_egreso = @depo_total_egreso,
                        flete_provee = @flete_provee,
                        fleteint = @fleteint, 
                        flete_devacio = @flete_devacio, 
                        flete_demora = @flete_demora, 
                        flete_guarderia = @flete_guarderia,
                        flete_totgastos = @flete_totgastos, 
                        flete_trucksemiid = @flete_trucksemiid,
                        poliza_provee = @poliza_provee, 
                        poliza_prima = @poliza_prima, 
                        poliza_demora = @poliza_demora,
                        poliza_impint = @poliza_impint, 
                        poliza_sellos = @poliza_sellos, 
                        despachante_provee = @despachante_provee,
                        despa_fijo = @despa_fijo,
                        despa_variable = @despa_variable,
                        despa_clasificacion = @despa_clasificacion,
                        despa_consultoria = @despa_consultoria,
                        despa_total_gastos = @despa_total_gastos,
                        custodia_provee = @custodia_provee, 
                        custodia_fact1 = @custodia_fact1,
                        custodia_fact2 = @custodia_fact2, 
                        custodia_gastos = @custodia_gastos,
                        banco_provee = @banco_provee,
                        banco_fact1 = @banco_fact1,
                        banco_fact2 = @banco_fact2,
                        banco_gastos = @banco_gastos,
                        gestdigdoc_provee = @gestdigdoc_provee,
                        gestdigdoc_fact1 = @gestdigdoc_fact1,
                        gestdigdoc_fact2 = @gestdigdoc_fact2,
                        gestdigdoc_gastos = @gestdigdoc_gastos,
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
    public async Task<TarifasByDate> GetByNearestDateAsync(string fecha)
    {
        //var sql = $@"SELECT * FROM tarifasbydate ORDER BY abs(date(htimestamp) - date('{fecha}')) LIMIT 1";
        //var sql = $@"SELECT * FROM tarifasbydate ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fechahora}'))) LIMIT 1";

        // Si la fecha por la que consulto tiene una entrada en la base, el criterio es la que tiene la cotizacion
        // con la hora mas tarde.
        var sql = $@"select * from tarifasbydate where htimestamp::date=date '{fecha}' order by htimestamp::time DESC LIMIT 1"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasByDate>(sql);
            // Si la consulta no da resultado, significa que no hay registro para la fecha solicitada.
            // Hago un select basado en la diferencia de tiempo entre la fecha proporcionada y los que hay
            // y me quedo con la diferencia mas chica.
            if(result==null)
            {
                sql = $@"SELECT * FROM tarifasbydate ORDER BY abs(extract(epoch from (htimestamp - timestamp '{fecha}'))) LIMIT 1";
                result = await connection.QuerySingleOrDefaultAsync<TarifasByDate>(sql);
            }
            return result;
        }
    }
}