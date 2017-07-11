//  ==============================================================
//    Copyright (c) 2016 PranWorks Co., Ltd
//    This program is protected by copyright laws.
//    Unauthorized reproduction or distribution of this
//    program or any portion of it is Prohibited.
// ==============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RiskGame.Repository.Common
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);
        List<T> AddList(List<T> list);
        T Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T Find(int id);
        T Get(Expression<Func<T, bool>> where);
        T GetWith(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetManyWith(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetManyWithRequired(IEnumerable<Expression<Func<T, bool>>> where, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllWith(params Expression<Func<T, object>>[] includes);
        int Count(Expression<Func<T, bool>> predicate = null);
        IEnumerable<T> GetWithSize(int skip = 0, int take = 0, Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetWithSize(int skip = 0, int take = 0, Expression<Func<T, bool>> where = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        int GetAllWithCount(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includes);
        bool CheckAny(Expression<Func<T, bool>> where);
        T GetLastOrderBy(Expression<Func<T, bool>> where, Func<T, object> keySelector);
        int SumInt(Expression<Func<T, bool>> where, Func<T, int> keySelector);
        decimal SumDecimal(Expression<Func<T, bool>> where, Func<T, int> keySelector);
        void LogAudit(T oldData, T newData, params string[] ignore);

        T GetWithCulture(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetManyWithCulture(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
    }
}