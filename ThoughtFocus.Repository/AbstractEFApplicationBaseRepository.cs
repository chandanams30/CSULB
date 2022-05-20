using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Repository
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class AbstractEFApplicationBaseRepository<T> : IEFApplicationBaseRepository<T>
        where T : class
    {
        #region Fields

        private DbContext context;

        #endregion Fields

        #region Constructors

        public AbstractEFApplicationBaseRepository(DbContext context)
        {
            this.context = context;
        }

        #endregion Constructors

        #region Methods

        public virtual void Add(T entity)
        {
            this.context.Set<T>().Add(entity);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return this.context.Set<T>().Where(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return this.context.Set<T>().Where(predicate).FirstOrDefault();
        }

        public virtual IQueryable<T> GetAll()
        {
            return this.context.Set<T>();
        }

        public virtual void
            Update(T entity)
        {
            this.context.Entry(entity).State = EntityState.Modified;
        }

        #endregion Methods
    }
}
