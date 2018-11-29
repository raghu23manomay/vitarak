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
        public DbSet<CustomerList> CustomerList { get; set; }
        public DbSet<EmployeeList> EmployeeList { get; set; }
        
        public DbSet<ProductMaster> ProductMaster { get; set; }
        public DbSet<SupplierMaster> SupplierMaster { get; set; }
        
=======

        public DbSet<Customer>  Customer { get; set; }
        public DbSet<CustomerDetails> CustomerDetails { get; set; }

        public DbSet<ProductMaster> ProductMaster { get; set; }
        public DbSet<SupplierMaster> SupplierMaster { get; set; }
        


>>>>>>> 2751fcccea2f842c32d41a72280d7185d404de45
    }
}