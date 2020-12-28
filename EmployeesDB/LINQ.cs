using EmployeesDB.Data.Models;
using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace EmployeesDB
{
    public class LINQ
    {
        private static EmployeesContext _context = new EmployeesContext();
        static string Task1()
        {
            var employees = (
                from e in _context.Employees
                where e.Salary > 48000
                orderby e.LastName
                select e
            ).ToList();

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.AddressId}");
            }
            return sb.ToString().TrimEnd();
        }

        static string Task2()
        {
            var town = new Towns() { Name = "Lalaland" };
            _context.Towns.Add(town);
            _context.SaveChanges();

            var address = new Addresses() 
            { 
                AddressText = "18 Solomatina",
                Town = town 
            };
            _context.Addresses.Add(address);
            _context.SaveChanges();

            var browns = (
                from employee in _context.Employees
                where employee.LastName == "Brown"
                select employee
            ).ToList();

            browns.ForEach(e => e.Address = address);
            var sb = new StringBuilder();
            foreach (var b in browns)
            {
                sb.AppendLine($"{b.FirstName} {b.LastName} {b.MiddleName} {b.JobTitle} {b.DepartmentId} {b.ManagerId} {b.Address.AddressText}");
            }
            return sb.ToString().TrimEnd();
        }

        static string Task3()
        {
            var employees = (
                   from e in _context.Employees
                   where (
                       from ep in e.EmployeesProjects
                       select ep.ProjectId
                   ).Any(pid => (
                           from p in _context.Projects
                           where 2002 <= p.StartDate.Year && p.StartDate.Year <= 2005
                           select p.ProjectId
                       ).Contains(pid)
                   )
                   select e
               )
               .Include(e => e.EmployeesProjects)
               .ThenInclude(ep => ep.Project)
               .Include(e => e.Manager)
               .Take(5)
               .ToList();

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                var projects = (
                    from ep in e.EmployeesProjects
                    select ep.Project
                ).ToList();

                foreach (var p in projects)
                {
                    if (p.EndDate == null)
                        sb.AppendLine($"Employee: {e.FirstName} {e.LastName}  Manager: {e.Manager.FirstName} {e.Manager.LastName} \n Name project: {p.Name} Start date: {p.StartDate} НЕ ЗАВЕРШЁН \n ");
                    else
                        sb.AppendLine($"Employee: {e.FirstName} {e.LastName}  Manager: {e.Manager.FirstName} {e.Manager.LastName} \n Name project: {p.Name} Start date: {p.StartDate} End date: {p.EndDate} \n ");

                }

            }
            return sb.ToString().TrimEnd();
        }

        static string Task4()
        {
            int id = int.Parse(Console.ReadLine());

            var employees = (
                from e in _context.Employees
                where e.EmployeeId == id
                select e
            ).FirstOrDefault();

            var sb = new StringBuilder();

            if (employees == null)
            {
                sb.AppendLine("Сотрудник не найден");
                return sb.ToString();
            }

            Console.WriteLine($"{employees.LastName} {employees.FirstName} {employees.MiddleName} - {employees.JobTitle}");

            var projects = (
                from ep in _context.EmployeesProjects
                where ep.EmployeeId == id
                select ep.Project
            ).ToList();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
            }
            return sb.ToString().TrimEnd();
        }

        static string Task5()
        {
            var departments = (
                from department in _context.Departments
                where department.Employees.Count < 5
                select department.Name
            ).ToList();

            return string.Join("; ", departments);
        }

        static string Task6()
        {
            string name = Console.ReadLine();
            int percent = int.Parse(Console.ReadLine());

            var department = (
                from d in _context.Departments
                where d.Name == name
                select d
            ).First();

            var sb = new StringBuilder();
            foreach (var employee in department.Employees)
            {
                employee.Salary *= (decimal)((100 + percent) / 100f);
            }

            _context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        static void Task7()
        {
            int id = int.Parse(Console.ReadLine());

            var department = (
                from d in _context.Departments
                where d.DepartmentId == id
                select d
            ).First();

            if (department == null)
            {
                Console.WriteLine("Отдел не найден");
                return;
            }

            department.Employees.Clear();
            _context.SaveChanges();
        }

        static void Task8()
        {
            string town = Console.ReadLine();

            var towns = (
                from t in _context.Towns
                where t.Name == town
                select t
            ).First();

            _context.Entry(towns).Collection(t => t.Addresses).Load();
            _context.Towns.Remove(towns);
            _context.SaveChanges();
        }
    }
}