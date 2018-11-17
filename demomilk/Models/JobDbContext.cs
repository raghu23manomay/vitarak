using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace demomilk.Models
{
    public class JobDbContext : DbContext
    {
        static JobDbContext()
        {
            Database.SetInitializer<JobDbContext>(null);
        }
        public JobDbContext() : base("Name=JobDbContext")
        {
        }
        public DbSet<Route> Route { get; set; }
        public DbSet<RouteDetails> RouteDetails { get; set; }

    }
}