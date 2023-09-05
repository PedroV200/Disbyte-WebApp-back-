namespace WebApiSample.Core;

using WebApiSample.Infrastructure; 
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

// LISTED 14_6_2023 12:57
// REFACTOR, LISTED 9_8_2023 16:53
// REFACTOR, LISTED 24_8_2023 17:05 - Version 3C
// REFACTOR, LISTED 30_8_2023 10:35 - Version 3D

public class EstimateDetailDBRepository : IEstimateDetailDBRepository
{
    private readonly IConfiguration configuration;
    public EstimateDetailDBRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    } 
    public async Task<int> AddAsync(EstimateDetailDB entity)
    {
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd hh:mm:ss");
        var sql = $@"INSERT INTO estimatedetails 
        (
             
                estimateheader_id,
                proveedores_id,
                ncm_id,
                ncm_ack,
                sku,
                description,
                imageurl,
                exw_u,
                fob_u,
                qty,
                pcsctn,
                cbmctn,
                gwctn,
                cambios_notas,
                ncm_arancel,
                ncm_te_dta_otro,
                ncm_iva,
                ncm_ivaad,
                gcias,
                ncm_sp1,
                ncm_sp2,
                precio_u,
                extrag_comex1,
                extrag_comex2,
                extrag_comex3,
                extrag_comex_notas,
                extrag_local1,
                extrag_local2,
                extrag_finan1,
                extrag_finan2,
                extrag_finan3,
                extrag_finan_notas,
                costo_u_est,
                costo_u_prov,
                costo_u,
                updated,
                htimestamp
                        ) VALUES 
                (
                 {entity.estimateheader_id}, 
                 {entity.proveedores_id}, 
                 {entity.ncm_id},
                '{entity.ncm_ack}',
                '{entity.sku}',
                '{entity.description}',
                '{entity.imageurl}',
                '{entity.exw_u.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.fob_u.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                 {entity.qty},
                 {entity.pcsctn},
                '{entity.cbmctn.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gwctn.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.cambios_notas}',              
                '{entity.ncm_arancel.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_te_dta_otro.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_iva.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_ivaad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gcias.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_sp1}',
                '{entity.ncm_sp2}',
                '{entity.precio_u.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_comex1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_comex2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_comex3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_comex_notas}',
                '{entity.extrag_local1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_local2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_finan1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_finan2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_finan3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extrag_finan_notas}',
                '{entity.costo_u_est.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costo_u_prov.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costo_u.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.updated}',
                '{tmpString}'
                )";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int Id)
    {
        var sql = $"DELETE FROM estimatedetails WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<int> DeleteByIdEstHeaderAsync(int IdEstHeader)
    {
        var sql = $"DELETE FROM estimatedetails WHERE IdEstHeader = {IdEstHeader}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<EstimateDetailDB>>GetAllAsync()
    {
        var sql = "SELECT * FROM estimatedetails";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateDetailDB>(sql);
        }
    }
        public async Task<IEnumerable<EstimateDetailDB>>GetAllByIdEstHeadersync(int Id)
    {
        var sql = $"SELECT * FROM estimatedetails WHERE estimateheader_id={Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateDetailDB>(sql);
        }
    }

    public async Task<EstimateDetailDB> GetByIdAsync(int Id)
    {
        var sql = $"SELECT * FROM estimatedetails WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<EstimateDetailDB>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(EstimateDetailDB entity)
    {
        
        var sql = @"UPDATE estimatedetails SET 

                    estimateheader_id = @estimateheader_id,
                    proveedores_id = @proveedores_id,
                    ncm_id = @ncm_id,
                    ncm_ack = @ncm_ack,
                    sku = @sku,
                    description = @description,
                    imageurl = @imageurl,
                    exw_u = @exw_u,
                    fob_u = @fob_u,
                    qty = @qty,
                    pcsctn = @pcsctn,
                    cbmctn = @cbmctn,
                    gwctn = @gwctn,
                    cambios_notas = @cambios_notas,
                    ncm_arancel = @ncm_arancel,
                    ncm_te_dta_otro = @ncm_te_dta_otro,
                    ncm_iva = @ncm_iva,
                    ncm_ivaad = @ncm_ivaad,
                    gcias = @gcias,
                    ncm_sp1 = @ncm_sp1,
                    ncm_sp2 = @ncm_sp2,
                    precio_u = @precio_u,
                    extrag_comex1 = @extrag_comex1,
                    extrag_comex2 = @extrag_comex2,
                    extrag_comex3 = @extrag_comex3,
                    extrag_comex_notas = @extrag_comex_notas,
                    extrag_local1 = @extrag_local1,
                    extrag_local2 = @extrag_local2,
                    extrag_finan1 = @extrag_finan1,
                    extrag_finan2 = @extrag_finan2,
                    extrag_finan3 = @extrag_finan3,
                    extrag_finan_notas = @extrag_finan_notas,
                    costo_u_est = @costo_u_est,
                    costo_u_prov = @costo_u_prov,
                    costo_u = @costo_u,
                    htimestamp = @htimestamp
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

    public async Task<int> ClearFlagsByEstimateHeaderIdAsync(int id)
    {
        
        var sql = $"update estimatedetails set updated=false WHERE estimateheader_id={id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql);
            return result;
        }
    }
}
