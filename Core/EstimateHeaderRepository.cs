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
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd hh:mm:ss");
        //entity.hTimeStamp=DateOnly.FromDateTime(DateTime.Now);
        var sql = $@"INSERT INTO estimateheader 
                (  
                description,
                estnumber,
                estvers,
                status,
                paisregion_id,
                carga_id,
                fwdpaisregion_id,
                own,
                dolar,
                tarifasfwd_id,
                tarifasflete_id,
                tarifasterminales_id,
                tarifaspolizas_id,
                tarifasdepositos_id,
                tarifasdespachantes_id,
                tarifasbancos_id,
                tarifasgestdigdoc_id,
                gasto_otro1,
                gasto_otro2,
                gasto_otro3,
                gasto_otro_description,
                constantes_id,
                ivaexcento,
                fob_grand_total,
                cbm_grand_total,
                gw_grand_total,
                cif_grand_total,
                gastos_loc_total,
                extragastos_total,
                impuestos_total,
                cantidad_contenedores,
                iibb_total,                
                htimestamp) 
                            VALUES 
                                    ('{entity.description}',
                                     {entity.estnumber},
                                     {entity.estvers},
                                     {entity.status},                  
                                     {entity.paisregion_id},
                                     {entity.carga_id},
                                     {entity.fwdpaisregion_id},
                                    '{entity.own}',
                                    '{entity.dolar.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     {entity.tarifasfwd_id},
                                     {entity.tarifasflete_id},
                                     {entity.tarifasterminales_id},
                                     {entity.tarifaspolizas_id},
                                     {entity.tarifasdepositos_id},
                                     {entity.tarifasdespachantes_id},
                                     {entity.tarifasbancos_id},
                                     {entity.tarifasgestdigdoc_id},
                                    '{entity.gasto_otro1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gasto_otro2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gasto_otro3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gasto_otro_description}',
                                     {entity.constantes_id},
                                    '{entity.ivaexcento}',
                                    '{entity.fob_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cbm_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gw_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cif_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gastos_loc_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extragastos_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.impuestos_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cantidad_contenedores.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.iibb_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
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
                        status = @status,
                        paisregion_id = @paisregion_id,
                        carga_id = @carga_id,
                        fwdpaisregion_id = @fwdpaisregion_id,
                        own = @own,
                        dolar = @dolar,
                        tarifasfwd_id = @tarifasfwd_id,
                        tarifasflete_id = @tarifasflete_id,
                        tarifasterminales_id = @tarifasterminales_id,
                        tarifaspolizas_id = @tarifaspolizas_id,
                        tarifasdepositos_id = @tarifasdepositos_id,
                        tarifasdespachantes_id = @tarifasdespachantes_id,
                        tarifasbancos_id = @tarifasbancos_id,
                        tarifasgestdigdoc_id = @tarifasgestdigdoc_id,
                        gasto_otro1 = @gasto_otro1,
                        gasto_otro2 = @gasto_otro2,
                        gasto_otro3 = @gasto_otro3,
                        gasto_otro_description = @gasto_otro_description,
                        constantes_id = @constantes_id,
                        ivaexcento = @ivaexcento,
                        fob_grand_total = @fob_grand_total,
                        cbm_grand_total = @cbm_grand_total,
                        gw_grand_total = @gw_grand_total,
                        cif_grand_total = @cif_grand_total,
                        gastos_loc_total = @gastos_loc_total,
                        extragastos_total = @extragastos_total,
                        impuestos_total = @impuestos_total,
                        cantidad_contenedores = @cantidad_contenedores,
                        iibb_total = @iibb_total,
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