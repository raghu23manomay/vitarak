using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace demomilk.Models
{
    public class Route
    {
        public DataTable dtTable;

        [Key]
        public int RouteId { get; set; }
        public int CityId { get; set; }
        public string Area { get; set; }
        
    }
}