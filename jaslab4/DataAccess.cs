using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace jaslab4
{
    public interface IGenericDAO<T>
    {
        void SaveOrUpdate(T item);

        T GetById(int id);

        IList<T> GetAll();

        void Delete(T item);

    }
    
    public class GenericDAO<T> : IGenericDAO<T>
    { 
        protected ISession session;

        public GenericDAO() { }

        public GenericDAO(ISession session)
        {
            this.session = session;
        }

        public void SaveOrUpdate(T item)
        {
            var transaction = session.BeginTransaction();
            session.SaveOrUpdate(item);
            transaction.Commit();
            session.Flush();
        }

        public T GetById(int id)
        {
            return session.Get<T>(id);
        }

        public IList<T> GetAll()
        {
            return session
                .CreateCriteria(typeof(T))
                .List<T>();
        }

        public void Delete(T item)
        {
            var transaction = session.BeginTransaction();
            session.Delete(item);
            transaction.Commit();
            session.Flush();
        }
    }

    public interface ITripDAO : IGenericDAO<Trip>
    {
        Trip GetTripByName(string name);
        void RemoveTripByName(string name);
    }
    
    public class TripDao : GenericDAO<Trip>, ITripDAO
    {
        public TripDao(ISession session) : base(session)
        {
            
        }
        
        public Trip GetTripByName(string name)
        {
            return session.CreateSQLQuery(@"SELECT * FROM trips WHERE trip_name LIKE :trip_name")
                .AddEntity("Trip", typeof(Trip))
                .SetParameter("trip_name", name)
                .List<Trip>()
                .FirstOrDefault();
        }

        public void RemoveTripByName(string name)
        {
            Delete(GetTripByName(name));
        }
    }

    public abstract class DAOFactory
    {
        public abstract ITripDAO getTripDAO();
        public abstract IGenericDAO<Passenger> getPassengerDAO();
    }
    
    public class NHibernateDAOFactory : DAOFactory
    {
        /** NHibernate sessionFactory */
        protected ISession session = null;

        public NHibernateDAOFactory(ISession session)
        {
            this.session = session;
        }

        public override ITripDAO getTripDAO() 
        {
            return new TripDao(session);
        }

        public override IGenericDAO<Passenger> getPassengerDAO()
        {
            return new GenericDAO<Passenger>(session);
        }
    }
    
}