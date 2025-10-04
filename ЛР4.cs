using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public class LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Фамилия не может быть пустой");
        Value = value;
    }

    public override string ToString() => Value;
}

public class Course
{
    public int Value { get; }

    public Course(int value)
    {
        if (value < 1 || value > 6)
            throw new ArgumentException("Курс должен быть от 1 до 6");
        Value = value;
    }

    public override string ToString() => Value.ToString();
}

public class Group
{
    public string Value { get; }

    public Group(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Группа не может быть пустой");
        Value = value;
    }

    public override string ToString() => Value;
}

public class Institute
{
    public string Value { get; }

    public Institute(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Институт не может быть пустым");
        Value = value;
    }

    public override string ToString() => Value;
}

public interface IStudentInfo
{
    string GetInfo();
}

public interface IGrades
{
    double GetAverageGrade();
    bool IsExcellent();
}
\
public class Student : IStudentInfo, IGrades
{
    public LastName StudentLastName { get; set; }
    public Course StudentCourse { get; set; }
    public Group StudentGroup { get; set; }
    public Institute StudentInstitute { get; set; }
    public List<int> Grades { get; set; }

    public Student(LastName lastName, Course course, Group group, Institute institute, List<int> grades)
    {
        StudentLastName = lastName;
        StudentCourse = course;
        StudentGroup = group;
        StudentInstitute = institute;
        Grades = grades;
    }

    public string GetInfo()
    {
        return $"{StudentLastName}, {StudentInstitute}, {StudentGroup}, курс {StudentCourse}";
    }

    public double GetAverageGrade()
    {
        return Grades.Count > 0 ? Grades.Average() : 0;
    }

    public bool IsExcellent()
    {
        return Grades.All(grade => grade == 5);
    }
}

public delegate void StudentDelegate(Student student);

class Program
{
    static List<Student> students = new List<Student>();

    static void InitializeStudents()
    {
        students.Add(new Student(
            new LastName("Иванов"),
            new Course(1),
            new Group("ИТ-101"),
            new Institute("ИТИ"),
            new List<int> { 5, 5, 5, 5 }
        ));

        students.Add(new Student(
            new LastName("Петров"),
            new Course(1),
            new Group("ИТ-101"),
            new Institute("ИТИ"),
            new List<int> { 5, 4, 5, 4 }
        ));

        students.Add(new Student(
            new LastName("Сидоров"),
            new Course(2),
            new Group("ИТ-201"),
            new Institute("ИТИ"),
            new List<int> { 3, 3, 4, 5 }
        ));

        students.Add(new Student(
            new LastName("Кузнецов"),
            new Course(1),
            new Group("ЭК-101"),
            new Institute("ЭКИ"),
            new List<int> { 5, 5, 4, 5 }
        ));
    }

    static void Main()
    {
        InitializeStudents();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("1 - Показать отличников 1-2 курсов");
            Console.WriteLine("2 - Средний балл по группам");
            Console.WriteLine("3 - Показать всех студентов");
            Console.WriteLine("4 - Демонстрация интерфейсов");
            Console.WriteLine("5 - Демонстрация делегата");
            Console.WriteLine("6 - Сохранить в файл");
            Console.WriteLine("7 - Выход");
            Console.Write("Выберите: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ShowExcellentStudents(); break;
                case "2": ShowGroupAverages(); break;
                case "3": ShowAllStudents(); break;
                case "4": DemoInterfaces(); break;
                case "5": DemoDelegate(); break;
                case "6": SaveToFile(); break;
                case "7": return;
            }

            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }

    static void ShowExcellentStudents()
    {
        Console.WriteLine("\n=== Отличники на 1-2 курсах ===");
        var excellent = students.Where(s => s.IsExcellent() && (s.StudentCourse.Value == 1 || s.StudentCourse.Value == 2));

        foreach (var student in excellent)
        {
            Console.WriteLine(student.GetInfo());
        }
    }

    static void ShowGroupAverages()
    {
        Console.WriteLine("\n=== Средний балл по группам (1-2 курсы) ===");
        var groups = students
            .Where(s => s.StudentCourse.Value == 1 || s.StudentCourse.Value == 2)
            .GroupBy(s => new { s.StudentInstitute, s.StudentGroup, s.StudentCourse })
            .Select(g => new {
                Institute = g.Key.StudentInstitute,
                Group = g.Key.StudentGroup,
                Course = g.Key.StudentCourse,
                Average = g.Average(s => s.GetAverageGrade())
            })
            .OrderByDescending(x => x.Average);

        foreach (var group in groups)
        {
            Console.WriteLine($"{group.Institute}, {group.Group}, курс {group.Course}: {group.Average:F2}");
        }
    }

    static void ShowAllStudents()
    {
        Console.WriteLine("\n=== Все студенты ===");
        foreach (var student in students)
        {
            Console.WriteLine($"{student.GetInfo()}, ср.балл: {student.GetAverageGrade():F2}");
        }
    }

    static void DemoInterfaces()
    {
        Console.WriteLine("\n=== Демонстрация интерфейсов ===");

        Console.WriteLine("\n--- IStudentInfo ---");
        foreach (IStudentInfo student in students)
        {
            Console.WriteLine(student.GetInfo());
        }

        Console.WriteLine("\n--- IGrades ---");
        foreach (IGrades student in students)
        {
            Console.WriteLine($"Средний балл: {student.GetAverageGrade():F2}, Отличник: {student.IsExcellent()}");
        }
    }

    static void DemoDelegate()
    {
        Console.WriteLine("\n=== Демонстрация многоадресного делегата ===");

        // Создаем многоадресный делегат
        StudentDelegate multiDelegate = null;

      
        multiDelegate += ShowStudentInfo;
        multiDelegate += ShowStudentGrades;
        multiDelegate += ShowExcellentStatus;

        // Вызываем для всех студентов
        foreach (var student in students)
        {
            Console.WriteLine("---");
            multiDelegate(student);
        }
    }

  
    static void ShowStudentInfo(Student student)
    {
        Console.WriteLine($"Студент: {student.StudentLastName}");
    }

    static void ShowStudentGrades(Student student)
    {
        Console.WriteLine($"Оценки: {string.Join(", ", student.Grades)}");
    }

    static void ShowExcellentStatus(Student student)
    {
        Console.WriteLine($"Отличник: {student.IsExcellent()}");
    }

    static void SaveToFile()
    {
        string filename = "students.txt";

        using (StreamWriter writer = new StreamWriter(filename))
        {
            writer.WriteLine("Список студентов:");
            foreach (var student in students)
            {
                writer.WriteLine($"{student.GetInfo()}, оценки: {string.Join(", ", student.Grades)}");
            }
        }

        Console.WriteLine($"Данные сохранены в файл: {filename}");
    }
}