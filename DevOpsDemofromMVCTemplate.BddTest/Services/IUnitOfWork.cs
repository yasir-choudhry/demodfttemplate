using System;
using System.Data;

namespace DevOpsDemofromMVCTemplate.BddTest.Services
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection GetConnection();
    }
}
