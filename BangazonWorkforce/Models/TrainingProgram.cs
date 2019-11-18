using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceMVC.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }
        [Display(Name = "Employees Enrolled")]
        public Employee Employee { get; set; } = new Employee();
        public List<Employee> EmployeeList { get; set; } = new List<Employee>();
    }
}