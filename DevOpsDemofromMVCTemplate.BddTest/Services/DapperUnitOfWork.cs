using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace DevOpsDemofromMVCTemplate.BddTest.Services
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private IDbConnection _connection;

        private IDbTransaction _transaction;
        private bool _disposed;

        public DapperUnitOfWork(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        public IDbConnection GetConnection()
        {

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            return _connection;
        }

        public void Dispose()
        {

            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {

            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                    }
                }
                _disposed = true;
            }
        }

        ~DapperUnitOfWork()
        {
            dispose(false);
        }


    }
}
