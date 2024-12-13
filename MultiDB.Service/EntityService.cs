using MultiDB.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using MultiDB.Services;
using MultiDB.Models.ViewModel;
using Microsoft.AspNetCore.Http;

namespace MultiDB.Service
{
  public class EntityService<T> : IEntityService<T> where T : class
  {
    //private static readonly SqlConnection conn = new SqlConnection(_connectionString);
    //DalSession _dalSession = new DalSession(conn);
    //public EntityService()
    //{
    //    _dalSession = new DalSession(conn);
    //}

    public IDbConnection _connection;
    public UnitOfWork _unitOfWork;
    public readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EntityService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
     
      _httpContextAccessor = httpContextAccessor;
      _connectionString = configuration.GetConnectionString("DB");
      var clientId = _httpContextAccessor.HttpContext?.User?.FindFirst("client_id")?.Value;
      if (clientId != null)
      {
        _connectionString = GetClientConnectionString(clientId);
      }
      else
      {
        _connectionString = configuration.GetConnectionString("DB");
      }
      
    }

    public void Insert(T entity)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        _connection.Open();
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          _unitOfWork.Begin();
          try
          {
            GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
            _repository.Insert(entity);
            //_unitOfWork.Commit();
          }
          catch (SqlException sqlEx)
          {
            // Handle SQL exceptions here
            for (int i = 0; i < sqlEx.Errors.Count; i++)
            {
              Console.WriteLine($"Error {i}: {sqlEx.Errors[i]}");
              // You can also log these errors or extract more specific information from them
            }

            _unitOfWork.Rollback();
            _connection.Close();
          }
          catch (Exception ex)
          {
            // Handle other exceptions here
            _unitOfWork.Rollback();
            _connection.Close();
          }
        }
      }
    }

    public void InsertBulk(List<T> entity)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        _connection.Open();
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          _unitOfWork.Begin();
          try
          {
            GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);

            foreach (var item in entity)
            {
              _repository.Insert(item);
            }
            //_unitOfWork.Commit();

          }
          catch (SqlException sqlEx)
          {
            // Handle SQL exceptions here
            for (int i = 0; i < sqlEx.Errors.Count; i++)
            {
              Console.WriteLine($"Error {i}: {sqlEx.Errors[i]}");
              // You can also log these errors or extract more specific information from them
            }

            _unitOfWork.Rollback();
            _connection.Close();
          }
          catch (Exception ex)
          {
            // Handle other exceptions here
            _unitOfWork.Rollback();
            _connection.Close();
          }
        }
      }
    }
    public long InsertWithReturn(T entity)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        _connection.Open();
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          _unitOfWork.Begin();
          try
          {
            GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
            var ID = _repository.InsertWithReturn(entity);
            //_unitOfWork.Commit();
            return ID;
          }
          catch (SqlException sqlEx)
          {
            // Handle SQL exceptions here
            for (int i = 0; i < sqlEx.Errors.Count; i++)
            {
              Console.WriteLine($"Error {i}: {sqlEx.Errors[i]}");
              // You can also log these errors or extract more specific information from them
            }

            _unitOfWork.Rollback();

            _connection.Close();
            return 0;
          }
          catch (Exception ex)
          {
            // Handle other exceptions here
            _unitOfWork.Rollback();
            _connection.Close();
            return 0;
          }
        }
      }
    }
    public virtual IEnumerable<T> GetModel()
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.GetAll();
        }
      }
    }
    public virtual IEnumerable<T> GetModelQuery(string query)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.GetAllQuery(query);
        }
      }
    }

    public virtual T Get(int id)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.Get(id);
        }

      }
    }

    public virtual T Find(Expression<Func<T, object>> predicate, object value)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.Find(predicate, value);
        }
      }
    }

    public virtual T GetDataQuery(string query, Dictionary<string, object> parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.GetFromQuery(query, parms);
        }
      }
    }

    public virtual bool UpdateEntity(T entity)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.Update(entity);
        }
      }
    }
    public bool Delete(T entity)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.Delete(entity);
        }
      }
    }
    //public int Delete(string where, IUnitOfWork unitOfWork, Dictionary<string, object> parms = null)
    //{
    //    GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
    //    return _repository.Delete(where,parms);
    //}

    public void ExecuteQuery(string query, Dictionary<string, object> parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          _repository.ExecuteQuery(query, parms);
        }
      }
    }

    public T ExecuteQueryScalar(string query, Dictionary<string, object> parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.ExecuteScalar(query, parms);
        }
      }
    }
    public List<long> ExecuteQueryScalarList(string query, Dictionary<string, object> parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.ExecuteScalarList(query, parms);
        }
      }
    }
    public void ExecuteSP(string storedProcedureName, Dictionary<string, object> parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          _repository.ExecuteSP(storedProcedureName, parms);
        }
      }
    }

    public void ExecuteSPobject(string storedProcedureName, object parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          _repository.ExecuteSPobject(storedProcedureName, parms);
        }
      }
    }

    public T ExecuteSPSingle(string storedProcedureName, Dictionary<string, object> parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.ExecuteSPSingle(storedProcedureName, parms);
        }
      }
    }
    public IEnumerable<T> ExecuteSPList(string storedProcedureName, object parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.ExecuteSPList(storedProcedureName, parms);
        }
      }
    }
    public IEnumerable<T> ExecuteIPAList(string storedProcedureName, object parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.ExecuteIPAList(storedProcedureName, parms);
        }
      }
    }
    public List<List<T>> QueryMultiple(string storedProcedureName, Dictionary<string, object> parms = null)
    {
      using (_connection = new NpgsqlConnection(_connectionString))
      {
        using (_unitOfWork = new UnitOfWork(_connection))
        {
          GenericRepository<T> _repository = new GenericRepository<T>(_unitOfWork);
          return _repository.QueryMultiple(storedProcedureName, parms);
        }
      }
    }
    public string GetClientConnectionString(string sClientId)
    {
      try
      {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
          conn.Open();

          // Use parameterized queries to prevent SQL injection
          string query = "SELECT CONNECTION_STRING FROM PUBLIC.CLIENTS WHERE id = @ClientId";

          using (var cmd = new NpgsqlCommand(query, conn))
          {
            // Add parameter to the query
            cmd.Parameters.AddWithValue("@ClientId", Convert.ToInt32(sClientId));

            using (var reader = cmd.ExecuteReader())
            {
              // If data is found, return the connection string
              if (reader.Read())
              {
                return reader.GetString(0); // Get the connection string from the first column
              }
              else
              {
                Console.WriteLine("No data found.");
                return string.Empty;  // Return an empty string if no data found
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        // Handle exceptions (e.g., log the error)
        Console.WriteLine($"Error: {ex.Message}");
        throw;  // Rethrow the exception if needed
      }
    }

  }
}
