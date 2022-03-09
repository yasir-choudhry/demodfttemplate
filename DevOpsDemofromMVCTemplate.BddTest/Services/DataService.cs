using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace DevOpsDemofromMVCTemplate.BddTest.Services
{
    public class DataService
    {
        private IUnitOfWork _uow;

        public DataService(IUnitOfWork uow)
        {
            _uow = uow;
        }


    }
}
