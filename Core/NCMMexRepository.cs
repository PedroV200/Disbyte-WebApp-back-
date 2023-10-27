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
// LISTED 27_10_2023 Cambia de NCM_Mex a NCM_Mex_py. El objeto _py esta en NCM_Mex.cs

public class NCM_MexRepository : INCM_MexRepository
{
private readonly IConfiguration configuration;
    public NCM_MexRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(NCM_Mex_py entity)
    {
        string tmpString=entity.last_update.ToString("yyyy-MM-dd hh:mm:ss");

        var sql = $@"INSERT INTO ncm_mex_py 
                (                      
                    code
                    igi
                    iva
                    dta
                    gravamenes_acuerdo
                    bk
                    description
                    documentacion_obligatoria_instancia_aduanera
                    lealtad_comercial
                    documentacion_requerida_para_ingreso_a_deposito
                    last_update 
                    ) 
                            VALUES 
                                    ('{entity.code}',
                                     '{entity.igi.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.iva.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.dta.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                     '{entity.gravamenes_acuerdo}',
                                     '{entity.bk}',                                   
                                     '{entity.description}',
                                     '{entity.documentacion_obligatoria_instancia_aduanera}',
                                     '{entity.lealtad_comercial}',
                                     '{entity.documentacion_requerida_para_ingreso_a_deposito}',
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
        var sql = $"DELETE FROM ncm_mex_py WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<int> DeleteByStrAsync(string code)
    {
        var sql = $"DELETE FROM ncm_mex_py WHERE code = '{code}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<NCM_Mex_py>> GetAllAsync()
    {
        var sql = "SELECT * FROM ncm_mex_py";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<NCM_Mex_py>(sql);
        }
    }
    public async Task<NCM_Mex_py> GetByIdAsync(int code)
    {
        var sql = $"SELECT * FROM ncm_mex_py WHERE id = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<NCM_Mex_py>(sql);
            return result;
        }
    }

    public async Task<NCM_Mex_py> GetByIdStrAsync(string code)
    {
        var sql = $"SELECT * FROM ncm_mex_py WHERE code = '{code}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<NCM_Mex_py>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(NCM_Mex_py entity)
    {
        
        var sql = @"UPDATE ncm_mex_py SET 

                                    code=@code,
                                    igi=@igi,
                                    iva=@iva,
                                    dta=@dta,
                                    gravamenes_acuerdo=@gravamenes_acuerdo,
                                    bk=@bk,
                                    description=@description,
                                    documentacion_obligatoria_instancia_aduanera=@documentacion_obligatoria_instancia_aduanera,
                                    lealtad_comercial=@lealtad_comercial,
                                    documentacion_requerida_para_ingreso_a_deposito=@documentacion_requerida_para_ingreso_a_deposito,
                                    last_update=@last_update

                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}
