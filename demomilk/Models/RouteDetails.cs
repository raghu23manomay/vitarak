using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace demomilk.Models
{
    public class RouteDetails
    {

        [Key] public int AreaID { get; set; }
        public string Area { get; set; }
        IEnumerable<RouteDetails> RouteDetail { get; set; }
        public int? TotalRows { get; set; }

    }
}