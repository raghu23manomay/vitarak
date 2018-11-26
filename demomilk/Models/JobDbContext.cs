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
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<Vehical> Vehical { get; set; }
        public DbSet<VehicalDetails> VehicalDetails { get; set; }
        public DbSet<SupplierDetails> SupplierDetails { get; set; }

    }
}