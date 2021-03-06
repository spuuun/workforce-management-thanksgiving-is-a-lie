﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration; 
using BangazonWorkforceMVC.Models;
using BangazonWorkforceMVC.Models.ViewModels;



namespace BangazonWorkforceMVC.Controllers
{
    public class DepartmentsController : Controller
    {
        private string _connectionString;
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public DepartmentsController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id as DepartmentId, d.Budget as Budget, d.Name as Department,
                                      COUNT(e.Id) as TotalEmployees
                                          FROM Department d Left JOIN Employee e on e.DepartmentId = d.Id
                                          GROUP BY d.Id, d.Name, d.Budget";

                    var reader = cmd.ExecuteReader();
                    var departments = new List<Department>();



                    while (reader.Read())
                    {
                        departments.Add(
                            new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Department")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees"))
                            });
                    }

                    reader.Close();
                    return View(departments);
                }
            }
        }

        // GET: Departments/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id as DepartmentId, d.Name as Department, d.Budget as Budget, e.Id as EmployeeId, 
                                               e.FirstName + ' ' + e.LastName as Employee
                                          FROM Department d LEFT JOIN Employee e ON d.Id = e.DepartmentId
                                         WHERE d.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Department> department = new Dictionary<int, Department>();

                    while (reader.Read())
                    {
                        int departmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                        if (!department.ContainsKey(departmentId))
                        {
                            Department newDepartment = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Department")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };
                            department.Add(departmentId, newDepartment);
                        }
                        Department fromDictionary = department[departmentId];

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("Employee")),


                            };
                            fromDictionary.employees.Add(employee);
                        }
                    }
                    reader.Close();
                    return View(department.Values.First());
                }
            }

        }



        // GET: Department/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department newDepartment)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Department (Name, Budget)
                                                VALUES (@name, @budget);";
                    cmd.Parameters.Add(new SqlParameter("@name", newDepartment.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", newDepartment.Budget));


                    cmd.ExecuteNonQuery();
                    return RedirectToAction(nameof(Index));
                }
            }

        }
    }
}


        
      