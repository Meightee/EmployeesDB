using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmployeesDB.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesDB
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();

        static void Main(string[] args)
        {
            //Console.WriteLine(Task1());
            //Console.WriteLine(Task2());
            //Console.WriteLine(Task3());
            //Console.WriteLine(Task4());
            //Console.WriteLine(Task5());
            //Console.WriteLine(Task6());
            //Console.WriteLine(Task7());
            Console.WriteLine(Task8());
        }

        static string Task1()
        {
            var employees = _context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.HireDate,
                    e.Salary
                })
                .OrderBy(e => e.LastName)
                .Where(e => e.Salary > 48000)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary} {e.HireDate}");
            }

            return sb.ToString().TrimEnd();
        }

        static string Task2()
        {
            _context.Towns.Add(new Towns { Name = "Lalaland" });
            _context.Addresses.Add(new Addresses
            {
                AddressText = "18 Solomatina",
                TownId = _context.Towns
                .AsEnumerable().LastOrDefault().TownId
            });
            _context.SaveChanges();

            var brownEmployees = _context.Employees
                .Select(e => e)
                .Where(e => e.LastName == "Brown").ToArray();
            var sb = new StringBuilder();
            foreach (var e in brownEmployees)
            {
                e.AddressId = _context.Addresses.AsEnumerable().LastOrDefault().AddressId;
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary} {e.HireDate} {e.AddressId}");
            }


            return sb.ToString().TrimEnd();
        }
        static string Task3()
        {
            var employees = _context.Employees.Join(_context.EmployeesProjects,
                e => e.EmployeeId,
                p => p.EmployeeId,
                (e, p) => new { e.FirstName, e.LastName, e.Manager, p.ProjectId, ProjectName = p.Project.Name, p.Project.StartDate, p.Project.EndDate })
                .Where(p => p.StartDate.Year > 2001 && p.StartDate.Year < 2006)
                .ToArray();
            var sb = new StringBuilder();
            int i = 0;
            int j = 0;
            foreach (var e in employees)
            {
                if ((i == 0) || (e.FirstName != employees[i - 1].FirstName && e.LastName != employees[i - 1].LastName && j > 0 && j < 5))
                {
                    if (e.EndDate == null)
                        sb.AppendLine($"Employee: {e.FirstName} {e.LastName}  Manager: {e.Manager.FirstName} {e.Manager.LastName} \n Name project: {e.ProjectName} Start date: {e.StartDate} НЕ ЗАВЕРШЁН \n ");
                    else
                        sb.AppendLine($"Employee: {e.FirstName} {e.LastName}  Manager: {e.Manager.FirstName} {e.Manager.LastName} \n Name project: {e.ProjectName} Start date: {e.StartDate} End date: {e.EndDate} \n ");
                    j++;
                }
                i++;
            }
            return sb.ToString().TrimEnd();
        }
        static string Task4()
        {
            int id = Convert.ToInt32(Console.ReadLine());
            var employees = _context.Employees.Join(_context.EmployeesProjects,
                e => e.EmployeeId,
                p => p.EmployeeId,
                (e, p) => new { e.EmployeeId, e.FirstName, e.LastName, e.MiddleName, e.JobTitle, e.Manager, p.ProjectId, ProjectName = p.Project.Name, p.Project.StartDate, p.Project.EndDate })
                .Where(e => e.EmployeeId == id)
                .ToArray();
            var sb = new StringBuilder();
            sb.AppendLine($"Employee: {employees[0].FirstName} {employees[0].LastName} - {employees[0].JobTitle} \n");
            foreach (var e in employees)
            {
                sb.AppendLine($"Name project: {e.ProjectName}\n ");
            }
            return sb.ToString().TrimEnd();
        }
        static string Task5()
        {
            var departments = _context.Departments
                .Select(d => d)
                .Where(d => d.Employees.Count < 5).ToArray();
            var sb = new StringBuilder();
            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name}");
            }
            return sb.ToString().TrimEnd();
        }
        static string Task6()
        {
            Console.WriteLine("Напишите название изменяемого отдела: ");
            String depart = Console.ReadLine();
            Console.WriteLine("На сколько процентов увеличить зарплату? ");
            int percent = Convert.ToInt32(Console.ReadLine());

            var depatments = _context.Departments
                .Select(d => d)
                .Where(d => d.Name == depart).ToArray();

            var employees = _context.Employees
               .Where(e => e.Department.Name == depart)
               .ToArray();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                Decimal salary = e.Salary;
                e.Salary = salary + salary * percent / 100;
                sb.AppendLine($"{e.FirstName} {e.LastName} Зарплата: {e.Salary}");
            }
            _context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        static string Task7()
        {
            Console.WriteLine("Напишите айДи отдела, который хотите расформировать ");
            int id = Convert.ToInt32(Console.ReadLine());
            var department = _context.Departments//вот тут я взяла отдел и работников с нужным айди
              .Include(e => e.Employees)
               .FirstOrDefault(e => e.DepartmentId == id);

            var sb = new StringBuilder();
            if (department != null) //проверочка
            {
                _context.Departments.Remove(department); //удаление
                _context.SaveChanges();
                Console.WriteLine("Весь отдел удален");
            }
            return sb.ToString().TrimEnd();
        } //этот код точно должен работать, там бд, а не я, ну честно. Вот ниже работает же((
        
        static string Task8()
        {
            Console.WriteLine("Напишите название города, который хотите удалить");
            String town = Console.ReadLine();
            var towns = _context.Towns
              .Include(e => e.Addresses)
               .FirstOrDefault(e => e.Name == town);

            var sb = new StringBuilder();
            if (towns != null)
            {
                _context.Towns.Remove(towns);
                _context.SaveChanges();
                Console.WriteLine("Город удален");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
