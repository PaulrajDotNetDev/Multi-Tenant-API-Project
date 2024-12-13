using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace MultiDB.Repository
{
    public  interface IGenericRepository<T> where T : class //, IEntity
    {
        void Insert(T entiy);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllQuery(string query);
        T Get(int id);
        T Find(Expression<Func<T, object>> predicate, object value);
        T GetFromQuery(string query, Dictionary<string, object> parms = null);
        bool Update(T entity);
        bool Delete(T entity);
        //int Delete(string where, Dictionary<string, object> parms = null);
        void ExecuteQuery(string query, Dictionary<string, object> parms = null);
        T ExecuteScalar(string query, Dictionary<string, object> parms = null);
        List<long> ExecuteScalarList(string query, Dictionary<string, object> parms = null);
        void ExecuteSP(string storedProcedureName, Dictionary<string, object> parms = null);
        void ExecuteSPobject(string storedProcedureName, object parms = null);
        T ExecuteSPSingle(string storedProcedureName, Dictionary<string, object> parms = null);
        IEnumerable<T> ExecuteSPList(string storedProcedureName, object parms = null);
        List<List<T>> QueryMultiple(string storedProcedureName, Dictionary<string, object> parms = null);
     
    }
}
