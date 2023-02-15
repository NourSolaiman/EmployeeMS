using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployyeManageSys
{
    internal class Employee
    {
        public int empId { get; set; }
        public string empName { get; set; }
        public string empAddress { get; set; }
        public string empPhoneNumber { get; set; }
        public string empEmail { get; set; }

        public static List<Employee> employee
            = new List<Employee>();

        public Employee(int id, string name, string address, string phoneNumber, string email)
        {
            empId = id;
            empName = name;
            empAddress = address;
            empPhoneNumber = phoneNumber;
            empEmail = email;

        }
    }
}
