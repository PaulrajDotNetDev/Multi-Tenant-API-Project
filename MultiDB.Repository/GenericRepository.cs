using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using DapperExtensions;
using System.Data.SqlClient;

namespace MultiDB.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private string tablename = typeof(T).Name;

        IUnitOfWork _unitOfWork = default;
        IDbConnection _dbConnect = null;
        public GenericRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbConnect = unitOfWork.Connection;
        }

        public IEnumerable<T> GetAll()
        {
            string sql = string.Format("SELECT * FROM {0}", tablename);
            return _dbConnect.Query<T>(sql);
        } 
        public IEnumerable<T> GetAllQuery(string query)
        {
            return _dbConnect.Query<T>(query);
        }
        public void Insert(T entity)
        {
            try
            {
                var columns = GetColumns(entity);
                var columnString = string.Join(",", columns);
                var valueString = string.Join(",", columns.Select(d => "@" + d));
                var insetQuery = $"insert INTO public.\"{tablename}\"({columnString}) values ({valueString})";
                _dbConnect.QueryAsync(insetQuery, entity, _unitOfWork.Transaction).Wait();
                // Commit the transaction (assuming _unitOfWork.Transaction is a valid transaction)
                _unitOfWork.Transaction.Commit();
            }
            catch (Exception ex) { }
        }
        private List<string> GetColumns(T entity)
        {
            var fields = entity.GetType().GetProperties().Select(x => x.Name).ToList();
            var columns = fields.Where(d => d != "id").ToList();
            return columns;
        }
        public long InsertWithReturn(T entity)
        {

            int insertedPrimaryKey = 0;
            try
            {
                var columns = GetColumns(entity);
                var columnString = string.Join(",", columns);
                var valueString = string.Join(",", columns.Select(d => "@" + d));
                var insertQuery = $"insert INTO public.\"{tablename}\"({columnString}) values ({valueString})";
                var  insertedPrimaryKey1 = _dbConnect.QueryFirstOrDefault<int>(insertQuery, entity, _unitOfWork.Transaction); 
                // Commit the transaction (assuming _unitOfWork.Transaction is a valid transaction)
                _unitOfWork.Transaction.Commit();

                // Fetch the inserted primary key using a SELECT query
                insertedPrimaryKey = _dbConnect.QueryFirstOrDefault<int>($"SELECT id FROM public.\"{tablename}\" WHERE id = lastval()");

            }
            catch (Exception ex) { }



            //// Perform the insert operation here
            //var valu = _dbConnect.Insert<T>(entity, _unitOfWork.Transaction);

            // Return the inserted entity


            return insertedPrimaryKey;
        }

        public T Get(int id)
        {
            return _dbConnect.Get<T>(id); ;
        }

        //public T Find(Expression<Func<T, object>> expression, object value)
        //{
        //    var binaryExpression = (BinaryExpression)((UnaryExpression)expression.Body).Operand;
        //    var exp = Expression.Lambda<Func<T, object>>(Expression.Convert(binaryExpression.Left, typeof(object)), expression.Parameters[0]);
        //    //var value = binaryExpression.Right.GetType().GetProperty("Value").GetValue(binaryExpression.Right);
        //    // var theOperator = Util.DetermineOperator(binaryExpression);

        //    //o var predicate = Predicates.Field<T>(exp, theOperator, value);
        //    //var predicate = DapperExt.Predicates.Field<T>(expression, DapperExt.Operator.Eq, value,true);
        //    return _dbConnect.GetList<T>("").FirstOrDefault();
        //}
        public T GetFromQuery(string query, Dictionary<string, object> parms = null)
        {
            T entity;
            if (parms != null)
            {
                entity = _dbConnect.Query<T>(query, new DynamicParameters(parms)).FirstOrDefault();
            }
            else
            {
                entity = _dbConnect.Query<T>(query).FirstOrDefault();
            }
            return entity;
        }
        public T Find(Expression<Func<T, object>> expression, object value)
        {
            var propertyName = GetPropertyName(expression.Body);
            var tableName = typeof(T).Name;

            // Building the SQL query
            var sqlQuery = $"SELECT * FROM \"{tableName}\" WHERE \"{propertyName}\" = @Value LIMIT 1";

            // Creating parameters
            var parameters = new DynamicParameters();
            parameters.Add("Value", value);

            // Executing the query
            var result = _dbConnect.Query<T>(sqlQuery, parameters).FirstOrDefault();
            return result;
        }


        private string GetPropertyName(Expression expression)
        {
            // Handle direct MemberExpression
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            // Handle UnaryExpression that wraps a MemberExpression
            if (expression is UnaryExpression unaryExpression)
            {
                // Handle Left and Right sides of the binary expression
                var left = GetPropertyName(unaryExpression.Operand);
                var right = GetPropertyName(unaryExpression.Operand);
                // Return the property name if found in Left or Right side
                return left ?? right;

            }

            // Handle BinaryExpression (for comparisons or complex expressions)
            if (expression is BinaryExpression binaryExpression)
            {
                // Handle Left and Right sides of the binary expression
                var left = GetPropertyName(binaryExpression.Left);
                var right = GetPropertyName(binaryExpression.Right);
                // Return the property name if found in Left or Right side
                return left ?? right;
            }

            // Handle LambdaExpression (if the lambda itself contains a complex expression)
            if (expression is LambdaExpression lambdaExpression)
            {
                return GetPropertyName(lambdaExpression.Body);
            }

            // Throw exception if none of the above types match
            throw new ArgumentException("Invalid expression");
        }
        public bool Update(T entity)
        {
            var columns = GetColumns(entity);
            var columnMappingString = string.Join(",", columns.Select(k => k + "=@" + k));
            var updateQuery = $"Update public.\"{tablename}\" SET {columnMappingString} WHERE id=@id";
            _dbConnect.QueryAsync(updateQuery, entity, _unitOfWork.Transaction).Wait();
            return true;
        }

        public bool UpdateBatch(T entity)
        {
            var columns = GetColumns(entity);
            var columnMappingString = string.Join(",", columns.Select(k => k + "=@" + k));
            var updateQuery = $"Update public.\"{tablename}\" SET {columnMappingString} WHERE batchid=@batchid";
            _dbConnect.QueryAsync(updateQuery, entity, _unitOfWork.Transaction).Wait();
            return true;
        }

        public bool Delete(T entity)
        {
            var columns = GetColumns(entity);
            var columnMappingString = string.Join(",", columns.Select(k => k + "=@" + k));
            var updateQuery = $"Delete from  public.\"{tablename}\"  WHERE id=@id";
            _dbConnect.QueryAsync(updateQuery, entity, _unitOfWork.Transaction).Wait();
            return true;
        }
        public void ExecuteQuery(string query, Dictionary<string, object> parms = null)
        {
            _dbConnect.Query<T>(query, param: parms, _unitOfWork.Transaction);
        }

        public T ExecuteScalar(string query, Dictionary<string, object> parms = null)
        {
            return _dbConnect.ExecuteScalar<T>(query, param: parms);
        }
        public List<long> ExecuteScalarList(string query, Dictionary<string, object> parms = null)
        {
            //var resultList = new List<T>();
            var resultList = _dbConnect.Query<long>(query, param: parms);
            //while (!reader.IsConsumed)
            //{
            //    resultList = reader.Read<T>().ToList();
            //}
            return resultList.ToList();
        }

        public void ExecuteSP(string storedProcedureName, Dictionary<string, object> parms = null)
        {
            _dbConnect.Execute(storedProcedureName, parms, _unitOfWork.Transaction, commandType: CommandType.StoredProcedure);
        }

        public void ExecuteSPobject(string storedProcedureName, object parms = null)
        {
            _dbConnect.Execute(storedProcedureName, parms, _unitOfWork.Transaction, commandType: CommandType.StoredProcedure);
        }

        public T ExecuteSPSingle(string storedProcedureName, Dictionary<string, object> parms = null)
        {
            IEnumerable<T> entities;
            entities = _dbConnect.Query<T>(storedProcedureName, parms, commandType: CommandType.StoredProcedure);
            if (entities != null && entities.Any())
                return entities.FirstOrDefault();
            else
                return default;
        }
        public IEnumerable<T> ExecuteSPList(string storedProcedureName, object parms = null)
        {
            IEnumerable<T> output = _dbConnect.Query<T>(storedProcedureName, param: parms, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            return output;
        }
        public IEnumerable<T> ExecuteIPAList(string storedProcedureName, object parms = null)
        {
            IEnumerable<T> output = _dbConnect.Query<T>(storedProcedureName, param: parms, commandTimeout: 0, commandType: CommandType.StoredProcedure);
            return output;
        }
        public List<List<T>> QueryMultiple(string storedProcedureName, Dictionary<string, object> parms = null)
        {
            var resultList = new List<List<T>>();
            try
            {
                var reader = _dbConnect.QueryMultiple(storedProcedureName, param: parms, commandTimeout: 0, commandType: CommandType.StoredProcedure);
                while (!reader.IsConsumed)
                {
                    resultList.Add(reader.Read<T>().ToList());
                }
            }
            catch (Exception e)
            {
                _unitOfWork.Connection.Close();
            }
            return resultList;
        }

    }
}
