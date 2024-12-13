using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MultiDB.Repository
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        readonly IDbConnection _connection = null;
        IDbTransaction _transaction = null;
        Guid _id;
        public UnitOfWork(IDbConnection connection)
        {
            _id = Guid.NewGuid();
            _connection = connection;
        }

        IDbConnection IUnitOfWork.Connection
        {
            get { return _connection; }
        }
        IDbTransaction IUnitOfWork.Transaction
        {
            get { return _transaction; }
        }
        Guid IUnitOfWork.Id
        {
            get { return _id; }
        }

        public void Begin()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            Dispose();
        }

        public void Dispose()
        {
            if (_transaction != null)
                _transaction.Dispose();
            _transaction = null;
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
        //public virtual void Dispose(bool disposing)
        //{
        //    if (!disposed)
        //    {
        //        if (disposing)
        //        {
        //            _connection.Dispose();
        //        }
        //    }
        //    disposed = true;
        //}
    }
}
