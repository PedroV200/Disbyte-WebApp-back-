namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;


// ADVERTENCIA: El PK de la base de datos es la posicion arancelaria, que no es
// un valor umerico, son numeros separados por puntos como si fuera una IP
// para poder honrar la Interface que require que tanto el metodo GET(by ID) como 
// el metodo DELETE usaran el id automatico agregado por la DB y no la posicion
// arancelaria en si misma.
// Solo UPDATE que recibe el entity completo puede hacer x posicion arancelaria (CODE)

public class NCM_MexRepository : INCM_MexRepository
{
private readonly IConfiguration configuration;
    public NCM_MexRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(NCM_Mex entity)
    {
        string tmpString=entity.htimestamp.ToString("yyyy-MM-dd hh:mm:ss");

        var sql = $@"INSERT INTO ncm_mex 
                (                      
                    description,
                    code,
                    igi,
                    iva,
                    dta,
                    sp1,
                    sp2,
                    gravamen_acuerdo,
                    bk,
                    bsp1,
                    docum_aduanera,
                    lealtad_com,
                    docum_depo,
                    otras_notas,

                    htimestamp) 
                            VALUES 
                                    ('{entity.description}',
                                     '{entity.code}',
                                     '{entity.igi.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.iva.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.dta.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.sp1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.sp2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.gravamen_acuerdo}',
                                     '{entity.bk}',
                                     '{entity.bsp1}',
                                     '{entity.docum_aduanera}',
                                     '{entity.lealtad_com}',
                                     '{entity.docum_depo}',
                                     '{entity.otras_notas}',
                                     '{tmpString}')";

        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM ncm_mex WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<int> DeleteByStrAsync(string code)
    {
        var sql = $"DELETE FROM ncm_mex WHERE code = '{code}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<NCM_Mex>> GetAllAsync()
    {
        var sql = "SELECT * FROM ncm_mex";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<NCM_Mex>(sql);
        }
    }
    public async Task<NCM_Mex> GetByIdAsync(int code)
    {
        var sql = $"SELECT * FROM ncm_mex WHERE id = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<NCM_Mex>(sql);
            return result;
        }
    }

    public async Task<NCM_Mex> GetByIdStrAsync(string code)
    {
        var sql = $"SELECT * FROM ncm_mex WHERE code = '{code}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<NCM_Mex>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(NCM_Mex entity)
    {
        
        var sql = @"UPDATE ncm_mex SET 

                        description = @description,
                        code = @code,
                        igi = @igi,
                        iva = @iva,
                        dta = @dta,
                        sp1 = @sp1,
                        sp2 = @sp2,
                        gravamen_acuerdo = @gravamen_acuerdo,
                        bk = @bk,
                        bsp1 = @bsp1,
                        docum_aduanera = @docum_aduanera,
                        lealtad_com = @lealtad_com,
                        docum_depo = @docum_depo,
                        otras_notas = @otras_notas,
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
