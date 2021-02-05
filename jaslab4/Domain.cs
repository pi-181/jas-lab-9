﻿using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace jaslab4
{
    public class Cabin
    {
        public virtual int CabinId { get; set; }
        public virtual string CabinName { get; set; }
        public virtual int Square { get; set; }
        public virtual string ClassName { get; set; }
        public virtual List<Passenger> Passengers { get; set; }
    }

    public class CabinMap : ClassMap<Cabin>
    {
        public CabinMap()
        {
            // Указание имени таблицы для зачетной книжки
            Table("cabins");
            
            // Отображение идентификатора на колонку таблицы
            Id(x => x.CabinId, "cabin_id").GeneratedBy.Native();
            
            // Отображение обычного поля на колонку таблицы
            Map(x => x.CabinName, "cabin_name");
            Map(x => x.Square, "square");
            Map(x => x.ClassName, "class_name");

            HasMany(x => x.Passengers).KeyColumns
                .Add("cabin_id")
                .Inverse()
                .Cascade
                .All();
        }
    }
    
    public class Passenger
    {
        public virtual int PassengerId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Sex { get; set; }
        public virtual Cabin Cabin { get; set; }
    }
    
    public class PassengerMap : ClassMap<Passenger>
    {
        public PassengerMap()
        {
            // Указание имени таблицы для зачетной книжки
            Table("passengers");
            
            // Отображение идентификатора на колонку таблицы
            Id(x => x.PassengerId, "pass_id").GeneratedBy.Native();
            
            // Отображение обычного поля на колонку таблицы
            Map(x => x.FirstName, "first_name");
            Map(x => x.LastName, "last_name");
            Map(x => x.Sex, "sex");
            
            References(x => x.Cabin).Column("cabin_id").Cascade.All();
        }
    }
}