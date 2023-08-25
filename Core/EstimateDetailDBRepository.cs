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
                sku,
                description,
                imageurl,
                exw,
                fobunit,
                fobunit_adj,
                fobunit_adj_description,
                qty,
                pcsctn,
                cbmctn,
                gwctn,
                freight_charge_adj,
                freight_insurance_adj,
                cif,
                cif_adj,
                cif_adj_description,
                ncm_id,
                ncm_arancel,
                ncm_te_dta_otro,
                ncm_iva,
                ncm_ivaad,
                gcias,
                ncm_sp1,
                ncm_sp2,
                gloc_fwd_adj,
                gloc_flete_adj,
                gloc_terminal_adj,
                gloc_poliza_adj,
                gloc_deposito_adj,
                gloc_despachante_adj,
                gloc_banco_adj,
                gloc_gestdigdoc_adj,
                gasto_otro1_adj,
                gasto_otro2_adj,
                gasto_otro3_adj,
                ajuste_expre1,
                ajuste_expre2,
                ajuste_expre3,
                gloc_adj_description,
                precio_u,
                costo_u,
                costo_u_provisorio,
                costo_u_provisorio_adj,
                costo_u_financiero,
                costo_u_financiero_adj,
                extra_gasto1,
                extra_gasto2,
                extra_gasto3,
                extra_gasto4,
                extra_gasto5,
                extra_gasto6,
                extra_gasto7,
                extra_gasto8,
                extra_gasto9,
                extra_gasto10,
                extra_gasto_expre,
                htimestamp
                        ) VALUES 
                (
                 {entity.estimateheader_id}, 
                 {entity.proveedores_id},  
                '{entity.sku}',
                '{entity.description}',
                '{entity.imageurl}',
                '{entity.exw.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.fobunit.ToString(CultureInfo.CreateSpecificCulture("en-US"))}', 
                '{entity.fobunit_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',    
                '{entity.fobunit_adj_description}',
                '{entity.qty.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.pcsctn.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.cbmctn.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gwctn.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.freight_charge_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.freight_insurance_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.cif.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.cif_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.cif_adj_description}',
                 {entity.ncm_id},
                '{entity.ncm_arancel.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_te_dta_otro.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_iva.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_ivaad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gcias.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_sp1}',
                '{entity.ncm_sp2}',
                '{entity.gloc_fwd_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gloc_flete_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gloc_terminal_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gloc_poliza_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gloc_deposito_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gloc_despachante_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gloc_banco_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gloc_gestdigdoc_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gasto_otro1_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gasto_otro2_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gasto_otro3_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ajuste_expre1}',
                '{entity.ajuste_expre2}',
                '{entity.ajuste_expre3}',
                '{entity.gloc_adj_description}',
                '{entity.precio_u.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costo_u.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costo_u_provisorio.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costo_u_provisorio_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costo_u_financiero.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.costo_u_financiero_adj.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto4.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto5.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto6.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto7.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto8.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto9.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto10.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.extra_gasto_expre}',
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
        var sql = $"SELECT * FROM estimatedetails WHERE IdEstHeader={Id}";
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
                    sku = @sku,
                    description = @description,
                    imageurl = @imageurl,
                    exw = @exw,
                    fobunit = @fobunit,
                    fobunit_adj = @fobunit_adj,
                    fobunit_adj_description = @fobunit_adj_description,
                    qty = @qty,
                    pcsctn = @pcsctn,
                    cbmctn = @cbmctn,
                    gwctn = @gwctn,
                    freight_charge_adj = @freight_charge_adj,
                    freight_insurance_adj = @freight_insurance_adj,
                    cif = @cif,
                    cif_adj = @cif_adj,
                    cif_adj_description = @cif_adj_description,
                    ncm_id = @ncm_id,
                    ncm_arancel = @ncm_arancel,
                    ncm_te_dta_otro = @ncm_te_dta_otro,
                    ncm_iva = @ncm_iva,
                    ncm_ivaad = @ncm_ivaad,
                    ncm_sp1 = @ncm_sp1,
                    ncm_sp2 = @ncm_sp2,
                    gcias = @gcias,
                    gloc_fwd_adj = @gloc_fwd_adj,
                    gloc_flete_adj = @gloc_flete_adj,
                    gloc_terminal_adj = @gloc_terminal_adj,
                    gloc_poliza_adj = @gloc_poliza_adj,
                    gloc_deposito_adj = @gloc_deposito_adj,
                    gloc_despachante_adj = @gloc_despachante_adj,    
                    gloc_banco_adj = @gloc_banco_adj,
                    gloc_gestdigdoc_adj = @gloc_gestdigdoc_adj,
                    gasto_otro1_adj = @gasto_otro1_adj,
                    gasto_otro2_adj = @gasto_otro2_adj,
                    gasto_otro3_adj = @gasto_otro3_adj,
                    ajuste_expre1 = @ajuste_expre1,
                    ajuste_expre2 = @ajuste_expre2,
                    ajuste_expre3 = @ajuste_expre3,
                    gloc_adj_description = @gloc_adj_description,
                    precio_u = @precio_u,
                    costo_u = @costo_u,
                    costo_u_provisorio = @costo_u_provisorio,
                    costo_u_provisorio_adj = @costo_u_provisorio_adj,
                    costo_u_financiero = @costo_u_financiero,
                    costo_u_financiero_adj = @costo_u_financiero_adj,
                    extra_gasto1 = @extra_gasto1,
                    extra_gasto2 = @extra_gasto2,
                    extra_gasto3 = @extra_gasto3,
                    extra_gasto4 = @extra_gasto4,
                    extra_gasto5 = @extra_gasto5,
                    extra_gasto6 = @extra_gasto6,
                    extra_gasto7 = @extra_gasto7,
                    extra_gasto8 = @extra_gasto8,
                    extra_gasto9 = @extra_gasto9,
                    extra_gasto10 = @extra_gasto10,
                    extra_gasto_expre = @extra_gasto_expre,
                    htimestamp = @htimestamp
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}