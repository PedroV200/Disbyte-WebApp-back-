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
            cnst_sp1,
            cnst_sp2,
            cnst_sp3,
            cnst_sp4,
            cnst_sp5,
            cnst_sp6,
            cnst_sp7,
            cnst_sp8,
            cnst_sp9,
            cnst_sp10,
            cnst_sp11,
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
                '{entity.CNST_SP1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP4.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP5.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP6.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP7.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP8.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP9.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP10.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.CNST_SP11.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
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
                        cnst_sp1 = @CNST_SP1,
                        cnst_sp2 = @CNST_SP2,
                        cnst_sp3 = @CNST_SP3,
                        cnst_sp4 = @CNST_SP4,
                        cnst_sp5 = @CNST_SP5
                        cnst_sp6 = @CNST_SP6,
                        cnst_sp7 = @CNST_SP7,
                        cnst_sp8 = @CNST_SP8,
                        cnst_sp9 = @CNST_SP9,
                        cnst_sp10 = @CNST_SP10,
                        cnst_sp11 = @CNST_SP11,
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