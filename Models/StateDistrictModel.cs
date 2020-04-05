using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.Models
{
    public class StateDistrictModel
    {
        public string state { get; set; }
        public List<DistrictData> districtData { get; set; }
    }

    public class Delta2
    {
        public int confirmed { get; set; }
    }

    public class DistrictData
    {
        public string district { get; set; }
        public int confirmed { get; set; }
        public string lastupdatedtime { get; set; }
        public Delta2 delta { get; set; }
    }
}
