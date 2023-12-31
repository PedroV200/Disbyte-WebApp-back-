﻿using Dapper;
using Npgsql;
using WebApiSample.Infrastructure;
using WebApiSample.Models;

namespace WebApiSample.Core
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly IConfiguration configuration;
        public EmpresaRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<int> AddAsync(Empresa entity)
        {
            var sql = $"INSERT INTO empresas (description,paisregion_id) VALUES ('{entity.description}',{entity.paisregion_id})";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        public async Task<int> DeleteAsync(int id)
        {
            var sql = $"DELETE FROM empresas WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var result = await connection.ExecuteAsync(sql);

                return result;
            }
        }
        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            var sql = "SELECT * FROM empresas";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<Empresa>(sql);
            }
        }
        public async Task<IEnumerable<EmpresaVista>> GetAllVistaAsync()
        {
            try
            {
                var sql = "select empresas.*,paisregion.description as pais,paisregion.region as region from empresas inner join paisregion on empresas.paisregion_id=paisregion.id";
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    return await connection.QueryAsync<EmpresaVista>(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
    }

        public async Task<Empresa> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM empresas WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Empresa>(sql);
                return result;
            }
        }
        public async Task<int> UpdateAsync(Empresa entity)
        {
            //entity.ModifiedOn=DateTime.Now;
            //entity.ModifiedOn=DateTime.Now;
            //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
            var sql = @"UPDATE empresas SET description = @description, paisregion_id = @paisregion_id WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
