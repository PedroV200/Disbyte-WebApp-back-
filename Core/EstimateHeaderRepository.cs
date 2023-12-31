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
// LISTED 28_9_2023 Se agrega el entrypoint para consultar la historia de versiones y usuarios de un determinado estnumber
// LISTED 19_10_2023 Repara bug en estimateheader que no enviaba en el query el estag_src_notas.


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
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd HH:mm:ss");
        string fechaEmbarque=entity.fecha_embarque.ToString("yyyy-MM-dd HH:mm:ss");
        string fechaPedimiento=entity.fecha_pedimiento.ToString("yyyy-MM-dd HH:mm:ss");

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
                    tarifupdate,
                    tarifrecent,
                    tarifasfwd_id,
                    tarifasflete_id,
                    tarifasterminales_id,
                    tarifaspolizas_id,
                    tarifasdepositos_id,
                    tarifasdespachantes_id,
                    tarifasbancos_id,
                    tarifasgestdigdoc_id,
                    gloc_fwd,
                    gloc_flete,
                    gloc_terminales,
                    gloc_polizas,
                    gloc_depositos,
                    gloc_despachantes,
                    gloc_bancos,
                    gloc_gestdigdoc,
                    gloc_descarga,
                    extrag_src1,
                    extrag_src2,
                    extrag_src_notas,
                    extrag_comex1,
                    extrag_comex2,
                    extrag_comex3,
                    extrag_comex_notas,
                    extrag_finanformula1_id,
                    extrag_finanformula2_id,
                    extrag_finanformula3_id,
                    extrag_finanformula4_id,
                    extrag_finanformula5_id,
                    extrag_finan1,
                    extrag_finan2,
                    extrag_finan3,
                    extrag_finan4,
                    extrag_finan5,
                    extrag_finan_notas,
                    constantes_id,
                    ivaexcento,
                    usarmoneda_local,
                    fob_grand_total,
                    cbm_grand_total,
                    gw_grand_total,
                    cif_grand_total,
                    gastos_loc_total,
                    extragastos_total,
                    impuestos_total,
                    cantidad_contenedores,
                    freight_cost,
                    freight_insurance_cost,
                    iibb_total,
                    project,
                    embarque,
                    bl,
                    fecha_embarque,
                    pedimiento,
                    fecha_pedimiento,
                    avatar_url,
                    limite_carga,
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
                                     {entity.tarifupdate},
                                     {entity.tarifrecent},
                                     {entity.tarifasfwd_id},
                                     {entity.tarifasflete_id},
                                     {entity.tarifasterminales_id},
                                     {entity.tarifaspolizas_id},
                                     {entity.tarifasdepositos_id},
                                     {entity.tarifasdespachantes_id},
                                     {entity.tarifasbancos_id},
                                     {entity.tarifasgestdigdoc_id},
                                    '{entity.gloc_fwd.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gloc_flete.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gloc_terminales.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gloc_polizas.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gloc_depositos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                                    '{entity.gloc_despachantes.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gloc_bancos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gloc_gestdigdoc.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gloc_descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_src1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_src2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_src_notas}',
                                    '{entity.extrag_comex1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_comex2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_comex3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_comex_notas}',
                                     {entity.extrag_finanformula1_id},
                                     {entity.extrag_finanformula2_id},
                                     {entity.extrag_finanformula3_id},
                                     {entity.extrag_finanformula4_id},
                                     {entity.extrag_finanformula5_id},
                                    '{entity.extrag_finan1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_finan2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_finan3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_finan4.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_finan5.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extrag_finan_notas}',
                                     {entity.constantes_id},
                                    '{entity.ivaexcento}',
                                    '{entity.usarmoneda_local}',
                                    '{entity.fob_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cbm_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gw_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cif_grand_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.gastos_loc_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.extragastos_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.impuestos_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.cantidad_contenedores.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.freight_cost.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.freight_insurance_cost.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.iibb_total.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.project}',
                                    '{entity.embarque}',
                                    '{entity.bl}',
                                    '{fechaEmbarque}',
                                    '{entity.pedimiento}',
                                    '{fechaPedimiento}',
                                    '{entity.avatar_url}',
                                     {entity.limite_carga},
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

        public async Task<EstimateHeaderDB> GetByEstNumberLastVers_1ROW_Async(int estnumber)
    {
        var sql = $"SELECT * FROM estimateheader WHERE estnumber={estnumber} ORDER BY estvers DESC LIMIT 1";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QuerySingleOrDefaultAsync<EstimateHeaderDB>(sql);
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

    public async Task<EstimateHeaderDBVista>GetByEstNumberLastVersBySectionVistaAsync(int estnumber, int code)
    {
        var sql = @$"select estimateheader.*, cargas.description as carga_str, po.description as paisorig, pd.description as paisdest
                    from estimateheader
                    inner join paisregion as po on po.id=estimateheader.fwdpaisregion_id 
					inner join paisregion as pd on pd.id=estimateheader.paisregion_id 
                    inner join cargas  on estimateheader.carga_id=cargas.id WHERE estnumber={estnumber} AND status={code} ORDER BY estvers DESC LIMIT 1";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QuerySingleOrDefaultAsync<EstimateHeaderDBVista>(sql);
        }
    }

    public async Task<EstimateHeaderDBVista> GetByEstNumberAnyVersVistaAsync(int estnumber, int estVers)
    {
        var sql = @$"select estimateheader.*, cargas.description as carga_str, po.description as paisorig, pd.description as paisdest
                    from estimateheader
                    inner join paisregion as po on po.id=estimateheader.fwdpaisregion_id 
					inner join paisregion as pd on pd.id=estimateheader.paisregion_id 
                    inner join cargas  on estimateheader.carga_id=cargas.id WHERE estnumber={estnumber} AND estVers={estVers}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QuerySingleOrDefaultAsync<EstimateHeaderDBVista>(sql);
        }
    }
/*public string bl{get;set;}
    public DateTime fecha_embarque{get;set;}
    public string pedimiento{get;set;}
    public DateTime fecha_pedimiento{get;set;}
    public string avatar_url{get;set;}*/
    public async Task<int> UpdateAsync(EstimateHeaderDB entity) 
    {
        string fechaEmbarque=entity.fecha_embarque.ToString("yyyy-MM-dd HH:mm:ss");
        string fechaPedimiento=entity.fecha_pedimiento.ToString("yyyy-MM-dd HH:mm:ss");
        
        var sql = @$"UPDATE estimateheader SET 
                   
                        description = @description,
                        estnumber = @estnumber,
                        estvers = @estvers,
                        status = @status,
                        paisregion_id = @paisregion_id,
                        carga_id = @carga_id,
                        fwdpaisregion_id = @fwdpaisregion_id,
                        own = @own,
                        dolar = @dolar,
                        tarifupdate = @tarifupdate,
                        tarifrecent = @tarifrecent,
                        tarifasfwd_id = @tarifasfwd_id,
                        tarifasflete_id = @tarifasflete_id,
                        tarifasterminales_id = @tarifasterminales_id,
                        tarifaspolizas_id = @tarifaspolizas_id,
                        tarifasdepositos_id = @tarifasdepositos_id,
                        tarifasdespachantes_id = @tarifasdespachantes_id,
                        tarifasbancos_id = @tarifasbancos_id,
                        tarifasgestdigdoc_id = @tarifasgestdigdoc_id,
                        gloc_fwd = @gloc_fwd,
                        gloc_flete = @gloc_flete,
                        gloc_terminales = @gloc_terminales,
                        gloc_spolizas = @gloc_spolizas,
                        gloc_depositos = @gloc_depositos,
                        gloc_despachantes = @gloc_despachantes,
                        gloc_bancos = @gloc_bancos,
                        gloc_gestdigdoc = @gloc_gestdigdoc,
                        gloc_descarga = @gloc_descarga,
                        extrag_src1 = @extrag_src1,
                        extrag_src2 = @extrag_src2,
                        extrag_src_notas = @extrag_src_notas,
                        extrag_comex1 = @extrag_comex1,
                        extrag_comex2 = @extrag_comex2,
                        extrag_comex3 = @extrag_comex3,
                        extrag_comex_notas = @extrag_comex_notas,
                        extrag_finanformula1_id = @extrag_finanformula1_id,
                        extrag_finanformula2_id = @extrag_finanformula2_id,
                        extrag_finanformula3_id = @extrag_finanformula3_id,
                        extrag_finanformula4_id = @extrag_finanformula4_id,
                        extrag_finanformula5_id = @extrag_finanformula5_id,
                        extrag_finan1 = @extrag_finan1,
                        extrag_finan2 = @extrag_finan2,
                        extrag_finan3 = @extrag_finan3,
                        extrag_finan4 = @extrag_finan4,
                        extrag_finan5 = @extrag_finan5,
                        extrag_finan_notas = @extrag_finan_notas,
                        constantes_id = @constantes_id,
                        ivaexcento = @ivaexcento,
                        usarmoneda_local = @usarmoneda_local,
                        fob_grand_total = @fob_grand_total,
                        cbm_grand_total = @cbm_grand_total,
                        gw_grand_total = @gw_grand_total,
                        cif_grand_total = @cif_grand_total,
                        gastos_loc_total = @gastos_loc_total,
                        extragastos_total = @extragastos_total,
                        impuestos_total = @impuestos_total,
                        cantidad_contenedores = @cantidad_contenedores,
                        freight_cost = @freight_cost,
                        freight_insurance_cost = @freight_insurance_cost,
                        iibb_total = @iibb_total,
                        project = @project,
                        embarque = @embarque,
                        bl = @bl,
                        fecha_embarque = @{fechaEmbarque},
                        pedimiento = @pedimiento,
                        fecha_pedimiento = @{fechaPedimiento},
                        avatar_url = @avatar_url,
                        limite_carga = @limite_carga,
                        htimestamp = @htimestamp
                             WHERE Id = @Id"; 
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

    public async Task<IEnumerable<OwnPresup>>GetOwnersList()
    {
        var sql = $"select DISTINCT ON(own) own,id from estimateheader";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QueryAsync<OwnPresup>(sql);
        }
    }

    public async Task<IEnumerable<TraceUser>> GetUserTraceByEstNumberUDAsync(int estnumber)
    {
        string sql="";
       
        sql = @$"select own,
                        estvers,
                        estnumber,
                        status,
                        fob_grand_total,
                        cif_grand_total,
                        tarifupdate,
                        htimestamp 
                                from estimateHeader WHERE estnumber={estnumber} ORDER BY id DESC";
       
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QueryAsync<TraceUser>(sql);
        }
    }
}