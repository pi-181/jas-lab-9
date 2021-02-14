using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace jaslab4
{
    public abstract class EntityBase
    {
        public virtual int Id { get; set; }
    }


    public class Trip : EntityBase
    {
        public virtual string Name { get; set; } = "";
        public virtual string Source { get; set; } = "";
        public virtual string Target { get; set; } = "";

        public virtual IList<Passenger> Passengers { get; set; }

        public Trip()
        {
            Passengers = new List<Passenger>();
        }
    }

    public class TripMap : ClassMap<Trip>
    {
        public TripMap()
        {
            // Указание имени таблицы для зачетной книжки
            Table("trips");
            
            // Отображение идентификатора на колонку таблицы
            Id(x => x.Id, "trip_id").GeneratedBy.Native();
            
            // Отображение обычного поля на колонку таблицы
            Map(x => x.Name, "trip_name").Unique();
            Map(x => x.Source, "source");
            Map(x => x.Target, "target");

            HasManyToMany(x => x.Passengers)
                .Table("trips_passengers")
                .ParentKeyColumn("trip_id")
                .ChildKeyColumn("pass_id")
                .Cascade.SaveUpdate();
        }
    }
    
    public class Passenger : EntityBase
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Sex { get; set; }
        public virtual IList<Trip> Trips { get; set; }

        public Passenger()
        {
            Trips = new List<Trip>();
        }
    }
    
    public class PassengerMap : ClassMap<Passenger>
    {
        public PassengerMap()
        {
            // Указание имени таблицы для зачетной книжки
            Table("passengers");
            
            // Отображение идентификатора на колонку таблицы
            Id(x => x.Id, "pass_id").GeneratedBy.Native();
            
            // Отображение обычного поля на колонку таблицы
            Map(x => x.FirstName, "first_name");
            Map(x => x.LastName, "last_name");
            Map(x => x.Sex, "sex");
           
            HasManyToMany(x => x.Trips)
                .Table("trips_passengers")
                .ParentKeyColumn("pass_id")
                .ChildKeyColumn("trip_id");
        }
    }
}