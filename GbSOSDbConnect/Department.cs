using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployyeManageSys
{
    internal class Department
    {
        public int DepId { get; set; }
        public string DepName { get; set; }
        public int Floor { get; set; }

        public static List<Department> department 
            = new List<Department>();

        public Department(int id, string name, int floor)
        {
            DepId = id;
            DepName = name;
            Floor = floor;
        }
    }
}
