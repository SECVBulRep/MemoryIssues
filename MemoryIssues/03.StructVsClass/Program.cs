using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
// стр 360
public class LocationClass
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public struct LocationStruct
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class PersonDataClass
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime BirthDate { get; set; }
    public int EmployeeId { get; set; }
}

public readonly  struct PersonDataStruct
{
    public string Firstname { get; init; }
    public string Lastname { get; init; }
    public DateTime BirthDate { get; init; }
    public int EmployeeId { get; init; }
}

public class EmployeeClass
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public ref  struct EmployeeStruct
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class Service
{
    public List<PersonDataClass> GetPersonsInBatchClasses(int amount)
    {
        var result = new List<PersonDataClass>(amount);
        for (int i = 0; i < amount; i++)
        {
            result.Add(new PersonDataClass
            {
                Firstname = $"Firstname{i}",
                Lastname = $"Lastname{i}",
                BirthDate = DateTime.Now.AddYears(-20).AddDays(i),
                EmployeeId = i
            });
        }
        return result;
    }

    public List<PersonDataStruct> GetPersonsInBatchStructs(int amount)
    {
        var result = new List<PersonDataStruct>(amount);
        for (int i = 0; i < amount; i++)
        {
            result.Add(new PersonDataStruct
            {
                Firstname = $"Firstname{i}",
                Lastname = $"Lastname{i}",
                BirthDate = DateTime.Now.AddYears(-20).AddDays(i),
                EmployeeId = i
            });
        }
        return result;
    }

    public EmployeeClass GetEmployeeClass(int employeeId)
    {
        double baseLat = 52.0, baseLon = 13.0;
        return new EmployeeClass
        {
            Latitude = baseLat + employeeId * 0.001,
            Longitude = baseLon + employeeId * 0.001
        };
    }

    public EmployeeStruct GetEmployeeStruct(int employeeId)
    {
        double baseLat = 52.0, baseLon = 13.0;
        return new EmployeeStruct
        {
            Latitude = baseLat + employeeId * 0.001,
            Longitude = baseLon + employeeId * 0.001
        };
    }
}

public static class LocationService
{
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double dx = lat1 - lat2;
        double dy = lon1 - lon2;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}

[SimpleJob]
[MemoryDiagnoser]
public class Benchy
{
    private readonly Service service = new Service();

    
    [Params(10,100,1000,10000)]
    public int Amount { get; set; }

    [Params(52.0)] 
    public double Latitude { get; set; }

    [Params(13.0)] 
    public double Longitude { get; set; }

    [Benchmark]
    public List<string> FilterPeopleWithinLocation_Classes()
    {
        var locationLat = Latitude;
        var locationLon = Longitude;
        var now = DateTime.Now;

        var input = service.GetPersonsInBatchClasses(Amount);
        var result = new List<string>(input.Count);

        foreach (var person in input)
        {
            if ((now - person.BirthDate).TotalDays <= 18 * 365) continue;

            var employee = service.GetEmployeeClass(person.EmployeeId);

            if (LocationService.CalculateDistance(locationLat, locationLon, employee.Latitude, employee.Longitude) < 10.0)
            {
                result.Add($"{person.Firstname} {person.Lastname}");
            }
        }

        return result;
    }

    [Benchmark]
    public List<string> FilterPeopleWithinLocation_Structs()
    {
        var locationLat = Latitude;
        var locationLon = Longitude;
        var now = DateTime.Now;

        var input = service.GetPersonsInBatchStructs(Amount).ToArray(); // Преобразуем список в массив
        var result = new List<string>(input.Length);

        foreach (ref var person in input.AsSpan())
        {
            if ((now - person.BirthDate).TotalDays <= 18 * 365) continue;

            var employee = service.GetEmployeeStruct(person.EmployeeId);

            if (LocationService.CalculateDistance(locationLat, locationLon, employee.Latitude, employee.Longitude) < 10.0)
            {
                result.Add($"{person.Firstname} {person.Lastname}");
            }
        }

        return result;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchy>();
    }
}
