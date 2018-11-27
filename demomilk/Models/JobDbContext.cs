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
        public DbSet<EditRoute> EditRoute { get; set; }
        public DbSet<RouteDetails> RouteDetails { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<Vehical> Vehical { get; set; }
        public DbSet<VehicalDetails> VehicalDetails { get; set; }
        public DbSet<SupplierDetails> SupplierDetails { get; set; }
<<<<<<< HEAD
        public DbSet<Customer>  Customer { get; set; }
        public DbSet<CustomerDetails> CustomerDetails { get; set; }
=======
        public DbSet<ProductMaster> ProductMaster { get; set; }
        public DbSet<SupplierMaster> SupplierMaster { get; set; }
        
>>>>>>> 83bc00c3e460636a2975a893b93b5bf67e3ec9e0

    }
}