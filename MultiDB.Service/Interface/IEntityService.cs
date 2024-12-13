using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace MultiDB.Services
{
    public interface IEntityService<T>
      where T : class
    {
        void Insert(T entity);
        void InsertBulk(List<T> entity);
        long InsertWithReturn(T entity);
        

        IEnumerable<T> GetModel();
        IEnumerable<T> GetModelQuery(string query);
        T Get(int id);
        T Find(Expression<Func<T, object>> predicate, object value);

        T GetDataQuery(string query, Dictionary<string, object> parms = null);
        bool UpdateEntity(T entity);
        bool Delete(T entity);
        //int Delete(string where, Dictionary<string, object> parms = null);
        void ExecuteQuery(string query, Dictionary<string, object> parms = null);
        T ExecuteQueryScalar(string query, Dictionary<string, object> parms = null);
        List<long> ExecuteQueryScalarList(string query, Dictionary<string, object> parms = null);
        void ExecuteSP(string storedProcedureName, Dictionary<string, object> parms = null);
        void ExecuteSPobject(string storedProcedureName, object parms = null);
        T ExecuteSPSingle(string storedProcedureName, Dictionary<string, object> parms = null);
        IEnumerable<T> ExecuteSPList(string storedProcedureName, object parms = null);
        IEnumerable<T> ExecuteIPAList(string storedProcedureName, object parms = null);

        List<List<T>> QueryMultiple(string storedProcedureName, Dictionary<string, object> parms = null);
         
    }
}
