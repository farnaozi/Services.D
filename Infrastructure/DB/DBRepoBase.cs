using Dapper;
using Services.D.Core.Interfaces;
using Services.D.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Services.D.Infrastructure.DB
{
    public class DBRepoBase
    {
        #region *** private

        ILoggerRepo _logger;

        #endregion
        #region *** private methods



        #endregion
        #region *** ctor

        public DBRepoBase(IOptions<AppSettings> settings, ILoggerRepo logger)
        {
            Settings = settings.Value;
            _logger = logger;

            Console.WriteLine($"-->Connection string {Settings.ConnectionString}");
        }

        #endregion
        #region *** dapper base

        protected async Task ExecuteWithSingleTransaction(string procName, object procParams = null)
        {
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                connection.Open();

                var trans = connection.BeginTransaction();

                try
                {
                    connection.ExecuteAsync(procName, procParams, commandType: CommandType.StoredProcedure, commandTimeout: 0, transaction: trans).Wait();

                    trans.Commit();
                }
                catch (Exception err)
                {
                    trans.Rollback();

                    await _logger.LogError($"Error on execute procedure: {procName}, error: {err.Message}");

                    throw;
                }
            }
        }
        protected async Task ExecuteWithTransaction(Action<SqlConnection, IDbTransaction> action)
        {
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                connection.Open();

                var trans = connection.BeginTransaction();

                try
                {
                    action(connection, trans);

                    trans.Commit();
                }
                catch (Exception err)
                {
                    trans.Rollback();

                    await _logger.LogError($"Error on executution , error: {err.Message}");

                    throw;
                }
            }
        }
        protected async Task Execute(string procName, object procParams = null, SqlConnection connection = null, IDbTransaction trans = null)
        {
            if (connection == null)
            {
                using (var con = new SqlConnection(Settings.ConnectionString))
                {
                    await con.ExecuteAsync(procName, procParams, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }
            }
            else
            {
                await connection.ExecuteAsync(procName, procParams, commandType: CommandType.StoredProcedure, commandTimeout: 0, transaction: trans);
            }
        }
        protected async Task<T> ExecuteScalar<T>(string query, object queryParams = null, CommandType commandType = CommandType.StoredProcedure, SqlConnection connection = null, IDbTransaction trans = null)
        {
            if (connection == null)
            {
                using (var con = new SqlConnection(Settings.ConnectionString))
                {
                    return await con.ExecuteScalarAsync<T>(query, queryParams, commandType: commandType, commandTimeout: 0);
                }
            }
            else
            {
                return await connection.ExecuteScalarAsync<T>(query, queryParams, commandType: commandType, commandTimeout: 0, transaction: trans);
            }
        }
        protected async Task<IEnumerable<T>> Query<T>(string query, object queryParams = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                return await connection.QueryAsync<T>(query, queryParams, commandType: commandType, commandTimeout: 0);
            }
        }

        #endregion
        #region *** protected

        protected AppSettings Settings { get; }

        #endregion
    }
}
