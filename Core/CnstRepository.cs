namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class CnstRepository : ICnstRepository
{
 private readonly IConfiguration configuration;
    public CnstRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(CONSTANTES entity)
    {
        var sql = $@"INSERT INTO constantes 
        (
            cnst_gastos_despa_cif_min,
            cnst_gastos_despa_cif_mult,
            cnst_gastos_despa_cif_thrhld,
            cnst_gastos_custodia_thrshld,
            cnst_gastos_gestdigdoc_mult,
            cnst_gastos_bancarios_mult,
            const_ncm_die_min,
            cnst_estad061_thrhldmax,
            cnst_estad061_thrhldmin,
            cnst_gcias_424_mult,
            cnst_seguro_porct,
            cnst_arancel_sim,
            cnst_freight_porct_arg,
            paisreg_china_shezhen
            paisreg_mex_guad,
            carga20,
            carga220,
            carga40,
            carga240,
            fwdtte_id,
            flete_id,
            terminal_id,
            despachantes_id,
            trucksemi_id,
            cnst_sp12,
            hTimeStamp = @hTimeStamp    
            ) VALUES 
                (
                '{entity.CNST_GASTOS_DESPA_Cif_Min.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_GASTOS_DESPA_Cif_Mult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_GASTOS_DESPA_Cif_Thrhld.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_GASTOS_CUSTODIA_Thrshld.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_GASTOS_GESTDIGDOC_Mult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CONST_NCM_DIE_Min.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_ESTAD061_ThrhldMAX.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_ESTAD061_ThrhldMIN.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_GCIAS_424_Mult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SEGURO_PORCT.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_ARANCEL_SIM.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_FREIGHT_PORCT_ARG.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                {entity.paisreg_china_shezhen},
                {entity.paisreg_mex_guad},
                {entity.carga20},
                {entity.carga220},
                {entity.carga40},
                {entity.carga240},
                {entity.fwdtte_id},
                {entity.flete_id},
                {entity.terminal_id},
                {entity.despachantes_id},
                {entity.trucksemi_id},
                '{entity.CNST_SP12.ToString(CultureInfo.CreateSpecificCulture("en-US"))}'
            
                )";
        try
        {
            
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        try
        {
        var sql = $"DELETE FROM constantes WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
        }catch (Exception)
        {
            return -1;
        }
    }
    public async Task<IEnumerable<CONSTANTES>> GetAllAsync()
    {
        try
        {
            var sql = "SELECT * FROM constantes";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<CONSTANTES>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<CONSTANTES> GetLastIdAsync()
    {
        try
        {
            var sql = "select * from constantes order by id DESC LIMIT 1";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QuerySingleOrDefaultAsync<CONSTANTES>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<CONSTANTES> GetByIdAsync(int id)
    {
        try
        {
            var sql = $"SELECT * FROM constantes WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<CONSTANTES>(sql);
                return result;
            }
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    public async Task<int> UpdateAsync(CONSTANTES entity)
    {
        try
        {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql =  @"UPDATE constantes SET 
                   
                        cnst_gastos_despa_cif_min = @CNST_GASTOS_DESPA_Cif_Min,
                        cnst_gastos_despa_cif_mult = @CNST_GASTOS_DESPA_Cif_Mult
                        cnst_gastos_despa_cif_thrhld = @CNST_GASTOS_DESPA_Cif_Thrhld,
                        cnst_gastos_custodia_thrshld = @CNST_GASTOS_CUSTODIA_Thrshld,
                        cnst_gastos_gestdigdoc_mult = @CNST_GASTOS_GESTDIGDOC_Mult,
                        cnst_gastos_bancarios_mult = @CNST_GASTOS_BANCARIOS_Mult,
                        const_ncm_die_min = @CONST_NCM_DIE_Min,
                        cnst_estad061_thrhldmax = @CNST_ESTAD061_ThrhldMAX,
                        cnst_estad061_thrhldmin = @NST_ESTAD061_ThrhldMIN,
                        cnst_gcias_424_mult = @CNST_GCIAS_424_Mult,
                        cnst_seguro_porct = @CNST_SEGURO_PORCT,
                        cnst_arancel_sim = @CNST_ARANCEL_SIM,
                        const_freight_porct_arg =@ CNST_FREIGHT_PORCT_ARG,
                        paisreg_china_shezhen = @paisreg_china_shezhen,
                        paisreg_mex_guad = @paisreg_mex_guad,
                        pcarga20 = @pcarga20,
                        carga220 = @carga220,
                        carga40 = @carga40,
                        carga240 = @carga240,
                        fwdtte_id = @fwdtte_id,
                        flete_id = @flete_id,
                        terminal_id = @terminal_id,
                        despachantes_id = @despachantes_id,
                        trucksemi_id =@ trucksemi_id,               
                        cnst_sp12 = @CNST_SP12
                                                    WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
        }catch(Exception)
        {
            return -1;
        }
    }
}