using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using DKP.Core;
using DKP.Core.Data;
using DKP.Core.Infrastructure;
using System.Data.Entity.Infrastructure;

namespace DKP.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields

        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(int id)
        {
            //see some suggested performance optimization (not tested)
            //http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189
            return this.Entities.Find(id);
        }

        /// <summary>
        /// 
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                var user = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
                if (user != null)
                {
                    entity.CreateTime = DateTime.Now;
                    entity.CreatorId = user.UserId;
                    entity.CreatorName = user.RealName;
                }

                this.Entities.Add(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += string.Format("Property: {0} Error: {1}",
                                             validationError.PropertyName,
                                             validationError.ErrorMessage)
                               + Environment.NewLine;
                    }
                }
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                var user = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;


                var enumerable = entities as T[] ?? entities.ToArray();
                var count = enumerable.Count();
                for (int i = 0; i < count; i++)
                {
                    T entity = enumerable[i];
                    if (user != null)
                    {
                        entity.CreateTime = DateTime.Now;
                        entity.CreatorId = user.UserId;
                        entity.CreatorName = user.RealName;
                    }
                    this.Entities.Add(entity);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += string.Format("Property: {0} Error: {1}",
                                             validationError.PropertyName,
                                             validationError.ErrorMessage)
                               + Environment.NewLine;
                    }
                }

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                var user = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
                if (user != null)
                {
                    entity.UpdateTime = DateTime.Now;
                    entity.UpdaterId = user.UserId;
                    entity.UpdaterName = user.RealName;
                }

                this._context.Modified(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine
                               + string.Format("Property: {0} Error: {1}",
                                               validationError.PropertyName,
                                               validationError.ErrorMessage);
                    }
                }

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Update(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                var user = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;

                var enumerable = entities as T[] ?? entities.ToArray();
                var count = enumerable.Count();
                for (int i = 0; i < count; i++)
                {
                    T entity = enumerable[i];
                    if (user != null)
                    {
                        entity.UpdateTime = DateTime.Now;
                        entity.UpdaterId = user.UserId;
                        entity.UpdaterName = user.RealName;
                    }
                    this._context.Modified(entity);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine
                               + string.Format("Property: {0} Error: {1}",
                                               validationError.PropertyName,
                                               validationError.ErrorMessage);
                    }
                }

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public void Delete(int id)
        {
            try
            {
                T entity = this.Entities.Find(id);
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine
                               + string.Format("Property: {0} Error: {1}",
                                               validationError.PropertyName,
                                               validationError.ErrorMessage);
                    }
                }

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Remove(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine
                               + string.Format("Property: {0} Error: {1}",
                                               validationError.PropertyName,
                                               validationError.ErrorMessage);
                    }
                }

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");


                var enumerable = entities as T[] ?? entities.ToArray();
                var count = enumerable.Count();
                for (int i = 0; i < count; i++)
                {
                    T entity = enumerable[i];
                    this.Entities.Remove(entity);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine
                               + string.Format("Property: {0} Error: {1}",
                                               validationError.PropertyName,
                                               validationError.ErrorMessage);
                    }
                }
                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }

        /// <summary>
        /// save
        /// </summary>
        public void Save()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    this._context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        #endregion
    }
}