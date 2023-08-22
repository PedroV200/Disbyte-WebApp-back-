namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

// LISTED 9_8_2023 15:22
// LISTED 26_7_2023 17:03
// LISTED 29_6_2023 17:56 
// REFACTOR agrega IDs (FKs a los maestros) para todos los proveedores (servicios u OEM) 
// REFACTOR para tratar al proveedor de poliza igual que al resto de los proveedores (descrip / ID)
// REFACTOR 3_8_2023 nueva version "Multiregion" basado en los sheets de Mexico y WIP Argentina


public class EstimateHeaderDBRepository : IEstimateHeaderDBRepository
{
    private readonly IConfiguration configuration;
    public EstimateHeaderDBRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(EstimateHeaderDB entity)
    {

        // Convierto la fecha al formato que postgre acepta. Le molesta AAAA/MM//dd. Tiene que ser AAAA-MM-dd
        //entity.hTimeStamp=DateTime.Now;
        string tmpString=entity.hTimeStamp.ToString("yyyy-MM-dd hh:mm:ss");
        //entity.hTimeStamp=DateOnly.FromDateTime(DateTime.Now);
        var sql = $@"INSERT INTO estimateheader 
                (  
                description, 
                estnumber, 
                estvers, 
                own, 
                pais_region,
                dolar,
                ivaexcento, 
                freight_type,
                freight_fwd, 
                freight_cost,
                freight_insurance_cost,
                cantidadcontenedores,
                constantesid,
                tarifasbydateid,
                
                totalgastosloc_uss,
                fob_grandtotal,
                cbm_grandtotal,
                gw_grandtotal,
                cif_grandtotal,
                iibbtotal,
                total_impuestos,
                sp1_grandtotal,
                sp2_grandtotal,
                sp3_grandtotal,
                sp4_grandtotal,
                htimestamp) 
                            VALUES 
                                    ('{entity.description}',
                                     {entity.estnumber},
                                     {entity.estvers},
                                    '{entity.own}',
                                     {entity.pais_region},
                                    '{entity.dolar.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.ivaexcento}',

                                     {entity.freight_type},
                                     {entity.freight_fwd},
                                    '{entity.freight_cost.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.freight_insurance_cost.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cantidadcontenedores.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',

                                     {entity.constantesid},
                                     {entity.tarifasbydateid},
                                     
                                    
                                    
                                    '{entity.totalgastosloc_uss.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.fob_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cbm_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gw_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cif_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.iibbtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.total_impuestos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',

                                    '{entity.sp1_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.sp2_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.sp3_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.sp4_grandtotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',

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
        var sql = $"DELETE FROM estimateheader WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<EstimateHeaderDB>>GetAllAsync()
    {
        var sql = "SELECT * FROM estimateheader";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateHeaderDB>(sql);
        }
    }
    public async Task<EstimateHeaderDB> GetByIdAsync(int Id)
    {
        var sql = $"SELECT * FROM estimateheader WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<EstimateHeaderDB>(sql);
            return result;
        }
    }
 
    // 6/7/2023 Busca entrada cuyo campo description contiene el string pasado como param
    public async Task<IEnumerable<EstimateHeaderDB>> GetByDescripAsync(string descrip)
    {
        var sql = $"select * from estimateheader where description LIKE '%{descrip}%'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QueryAsync<EstimateHeaderDB>(sql);
            return result;
        }
    }


    public async Task<IEnumerable<EstimateHeaderDB>> GetByEstNumberLastVersAsync(int estnumber)
    {
        var sql = $"SELECT * FROM estimateheader WHERE estnumber={estnumber} ORDER BY estvers DESC";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QueryAsync<EstimateHeaderDB>(sql);
        }
    }

    // 6/7/2023 Trae el proximo ID LIBRE para estNumber.
    public async Task<int> GetNextEstNumber()
    {
        var sql = $"SELECT MAX(estnumber) from estimateheader";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return (await connection.QuerySingleOrDefaultAsync<int>(sql)+1);
        }
    }

    // 6/7/2023 Trae la proxima version LIBRE de un determinado presupuesto
    public async Task<int> GetNextEstVersByEstNumber(int estNumber)
    {
        var sql = $"select MAX(estVers) from estimateheader where estnumber={estNumber}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return (await connection.QuerySingleOrDefaultAsync<int>(sql)+1);
        }
    }

    // 6/7/2023. Trae todos las versiones de un presupuesto
    public async Task<IEnumerable<EstimateHeaderDB>> GetAllVersionsFromEstimate(int estNumber)
    {
        var sql = $"SELECT * from estimateheader where estnumber={estNumber}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QueryAsync<EstimateHeaderDB>(sql);
        }
    }

    public async Task<EstimateHeaderDB> GetByEstNumberAnyVersAsync(int estnumber, int estVers)
    {
        var sql = $"SELECT * FROM estimateheader WHERE estnumber={estnumber} AND estVers={estVers}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QuerySingleOrDefaultAsync<EstimateHeaderDB>(sql);
        }
    }

    public async Task<int> UpdateAsync(EstimateHeaderDB entity)
    {
        var sql = @"UPDATE estimateheader SET 
                   
                description = @description, 
                estnumber = @estnumber, 
                estvers = @estvers, 
                own = @own, 
                pais_region = @pais_region,
                dolar = @dolar,
                ivaexcento = @ivaexcento, 
                freight_type = @freight_type,
                freight_fwd = @freight_fwd, 
                freight_cost = @freight_cost,
                freight_insurance_cost = @freight_insurance_cost,
                cantidadcontenedores = @cantidadcontenedores,
                constantesid = @constantesid,
                tarifasbydateid = @tarifasbydateid,
                
                totalgastosloc_uss = @totalgastosloc_uss,
                fob_grandtotal = @fob_grandtotal,
                cbm_grandtotal = @cbm_grandtotal,
                gw_grandtotal = @gw_grandtotal,
                cif_grandtotal = @cif_grandtotal,
                iibbtotal = @iibbtotal,
                total_impuestos = @total_impuestos,
                sp1_grandtotal = @sp1_grandtotal,
                sp2_grandtotal = @sp2_grandtotal,
                sp3_grandtotal = @sp3_grandtotal,
                sp4_grandtotal = @sp4_grandtotal,
                htimestamp = @htimestamp
                             WHERE Id = @Id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}