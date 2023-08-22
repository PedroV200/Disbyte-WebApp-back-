namespace WebApiSample.Core;

using WebApiSample.Infrastructure; 
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

// LISTED 14_6_2023 12:57
// REFACTOR, LISTED 9_8_2023 16:53

public class EstimateDetailDBRepository : IEstimateDetailDBRepository
{
    private readonly IConfiguration configuration;
    public EstimateDetailDBRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    } 
    public async Task<int> AddAsync(EstimateDetailDB entity)
    {
        var sql = $@"INSERT INTO estimatedetails 
        (
                modelo, 
                oemprovee, 
                sku, 
                imageurl, 
                ncm, 
                exw, 
                fobunit,
                qty,
                pcsctn,
                cbmctn,
                gwctn,
                str_sp1,
                str_sp2,
                val_sp1,
                val_sp2,
                val_sp3,
                estheaderid, 
                imp_sp1,
                imp_sp2,
                imp_sp3,
                ncm_arancelgrav, 
                ncm_te_dta_otro, 
                ncm_iva, 
                ncm_ivaad,
                gcias,
                calc_sp1,
                calc_sp2,
                calc_sp3
                        ) VALUES 
                ('{entity.modelo}',
                 {entity.oemprovee},
                '{entity.sku}',
                '{entity.imageurl}',
                 {entity.ncm},
                '{entity.exw.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.fobunit.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.qty},
                 {entity.pcsctn},
                '{entity.cbmctn.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gwctn.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.str_sp1}',
                '{entity.str_sp2}',
                '{entity.val_sp1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.val_sp2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.val_sp3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                 {entity.estheaderid},
                '{entity.imp_sp1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.imp_sp2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.imp_sp3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_arancelgrav.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_te_dta_otro.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_iva.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.ncm_ivaad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.gcias.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.calc_sp1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.calc_sp2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                '{entity.calc_sp3.ToString(CultureInfo.CreateSpecificCulture("en-US"))}'
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
                modelo = @modelo, 
                oemprovee = @oemprovee, 
                sku = @sku, 
                imageurl = @imageurl, 
                ncm = @ncm, 
                exw = @exw, 
                fobunit = @fobunit,
                qty = @qty,
                pcsctn = @pcsctn,
                cbmctn = @cbmctn,
                gwctn = @gwctn,
                str_sp1 = @str_sp1,
                str_sp2 = @str_sp2,
                val_sp1 = @val_sp1,
                val_sp2 = @val_sp2,
                val_sp3 = @val_sp3,
                estheaderid = @estheaderid, 
                imp_sp1 = @imp_sp1,
                imp_sp2 = @imp_sp2,
                imp_sp3 = @imp_sp3,
                ncm_arancelgrav = @ncm_arancelgrav, 
                ncm_te_dta_otro = @ncm_te_dta_otro, 
                ncm_iva = @ncm_iva, 
                ncm_ivaad = @ncm_ivaad,
                gcias = @gcias,
                calc_sp1 = @calc_sp1,
                calc_sp2 = @calc_sp2,
                calc_sp3 = @calc_sp3
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}