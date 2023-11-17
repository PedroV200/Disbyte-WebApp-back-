namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// En conformidad con version 3B Ago15

public class TarifasMexRepository : ITarifasMexRepository
{
 private readonly IConfiguration configuration;
    public TarifasMexRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasMex entity)
    {
        entity.fecha=DateTime.Now;
        string tmpString=entity.fecha.ToString("yyyy-MM-dd HH:mm:ss");
        //var sql = $"INSERT INTO tarifasdepositos (depo, contype, descarga, ingreso, totingreso, carga, armado, egreso, totegreso) VALUES ('{entity.depo}','{entity.contype}','{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totegreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        var sql = $@"INSERT INTO tarifasmex 
                (  
                    fecha,
	                flete_1p40sthq,
	                flete_1p20ft,  
	                seguro,
                	gloc_fwd_1p40sthq,
	                gloc_fwd_1p20ft,
	                terminal_1p40sthq,
	                terminal_1p20ft,
	                fleteint_1p40sthq_guad,
	                fleteint_1p20ft_guad,
	                fleteint_2p40sthq_guad,
	                fleteint_2p20ft_guad,
	                fleteint_1p40sthq_cdmx,
	                fleteint_1p20ft_cdmx,
	                fleteint_2p40sthq_cdmx,
	                fleteint_2p20ft_cdmx,
	                descarga_meli_1p40sthq_guad,
	                descarga_meli_1p20ft_guad,
	                descarga_meli_1p40sthq_cdmx,
	                descarga_meli_1p20ft_cdmx,
	                despa_fijo,
	                despa_var,
	                despa_clasific,
	                despa_consult 
                    ) 
                            VALUES 
                                    ('{tmpString}',                                     
                                     '{entity.flete_1p40sthq.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.flete_1p20ft.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.seguro.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',  

                                     '{entity.gloc_fwd_1p40sthq.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.gloc_fwd_1p20ft.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.terminal_1p40sthq.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                                     '{entity.terminal_1p20ft.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 

                                     '{entity.fleteint_1p40sthq_guad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.fleteint_1p20ft_guad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.fleteint_2p40sthq_guad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                                     '{entity.fleteint_2p20ft_guad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',   

                                     '{entity.fleteint_1p40sthq_cdmx.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.fleteint_1p20ft_cdmx.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.fleteint_2p40sthq_cdmx.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                                     '{entity.fleteint_2p20ft_cdmx.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',

                                     '{entity.descarga_meli_1p40sthq_guad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.descarga_meli_1p20ft_guad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.descarga_meli_1p40sthq_cdmx.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                                     '{entity.descarga_meli_1p20ft_cdmx.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',      

                                     '{entity.despa_fijo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.despa_var.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.despa_clasific.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                                     '{entity.despa_consult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}'                                              
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
        var sql = $"DELETE FROM tarifasmex WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasMex>> GetAllAsync()
    {
        var sql = $"SELECT * FROM tarifasmex ORDER BY abs(extract(epoch from (fecha - timestamp '{DateTime.Now.ToString("yyyy-MM-dd")}'))) ASC";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasMex>(sql);
        }
    }

    

    public async Task<TarifasMex> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasmex WHERE id = {id} ";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasMex>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(TarifasMex entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasmex SET 

                            fecha = @fecha,
	                        flete_1p40sthq = @flete_1p40sthq,
	                        flete_1p20ft = @flete_1p20ft,  
	                        seguro = @seguro,
                	        gloc_fwd_1p40sthq = @gloc_fwd_1p40sthq,
	                        gloc_fwd_1p20ft = @gloc_fwd_1p20ft,
	                        terminal_1p40sthq = @terminal_1p40sthq,
	                        terminal_1p20ft = @terminal_1p20ft,
	                        fleteint_1p40sthq_guad = @fleteint_1p40sthq_guad,
	                        fleteint_1p20ft_guad = @fleteint_1p20ft_guad,
	                        fleteint_2p40sthq_guad = @fleteint_2p40sthq_guad,
	                        fleteint_2p20ft_guad = @fleteint_2p20ft_guad,
	                        fleteint_1p40sthq_cdmx = @fleteint_1p40sthq_cdmx,
	                        fleteint_1p20ft_cdmx = @fleteint_1p20ft_cdmx,
	                        fleteint_2p40sthq_cdmx = @fleteint_2p40sthq_cdmx,
	                        fleteint_2p20ft_cdmx = @fleteint_2p20ft_cdmx,
	                        descarga_meli_1p40sthq_guad = @descarga_meli_1p40sthq_guad,
	                        descarga_meli_1p20ft_guad = @descarga_meli_1p20ft_guad,
	                        descarga_meli_1p40sthq_cdmx = @descarga_meli_1p40sthq_cdmx,
	                        descarga_meli_1p20ft_cdmx = @descarga_meli_1p20ft_cdmx,
	                        despa_fijo = @despa_fijo,
	                        despa_var = @despa_var,
	                        despa_clasific = @despa_clasific,
	                        despa_consult = @despa_consult

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
    public async Task<TarifasMex> GetByNearestDateAsync(string fecha)
    {

        // SIMPLIFICADA. Solo se consulta x fecha. La hora no se tiene en cuenta. Las Tarifas se ingresan una vez x semana
        var sql = $@"SELECT * FROM tarifasmex ORDER BY abs(extract(epoch from (fecha - timestamp '{fecha}'))) LIMIT 1";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasMex>(sql);
            return result;
        }

        // ORIGINAL
      /*  // Si la fecha por la que consulto tiene una entrada en la base, el criterio es la que tiene la cotizacion
        // con la hora mas tarde.
        var sql = $@"select * from tarifasmex where fecha::date=date '{fecha}' order by fecha::time DESC LIMIT 1"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasMex>(sql);
            // Si la consulta no da resultado, significa que no hay registro para la fecha solicitada.
            // Hago un select basado en la diferencia de tiempo entre la fecha proporcionada y los que hay
            // y me quedo con la diferencia mas chica.
            if(result==null)
            {
                sql = $@"SELECT * FROM tarifasmex ORDER BY abs(extract(epoch from (fecha - timestamp '{fecha}'))) LIMIT 1";
                result = await connection.QuerySingleOrDefaultAsync<TarifasMex>(sql);
            }
            return result;
        }*/
    }
}