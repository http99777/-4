using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileApp
{
    public interface IDisplayable
    {
        void Display();
    }

    public interface ICalculatable
    {
        double CalculateAverage();
    }

    public interface IGradeManager
    {
        void AddGrade(int grade);
        bool HasExcellentGrades();
    }

    // Многоадресный делегат для работы со студентами
    public delegate void StudentDelegate(Student student);

    public class Student : IDisplayable, ICalculatable, IGradeManager
    {
        public string LastName;
        public List<int> Grades = new List<int>();

        public Student(string lastName, List<int> grades)
        {
            LastName = lastName;
            Grades = grades ?? new List<int>();
        }

        public void Display()
        {
            Console.WriteLine($"Студент: {LastName}");
            Console.WriteLine($"Оценки: {string.Join(", ", Grades)}");
            Console.WriteLine($"Средний балл: {CalculateAverage():F2}");
            Console.WriteLine($"Отличник: {(HasExcellentGrades() ? "Да" : "Нет")}");
            Console.WriteLine("---");
        }

        public double CalculateAverage()
        {
            return Grades.Count > 0 ? Grades.Average() : 0;
        }

        // Реализация интерфейса IGradeManager
        public void AddGrade(int grade)
        {
            if (grade >= 2 && grade <= 5)
            {
                Grades.Add(grade);
                Console.WriteLine($"Добавлена оценка {grade} студенту {LastName}");
            }
            else
            {
                Console.WriteLine($"Ошибка: оценка {grade} некорректна!");
            }
        }

        public bool HasExcellentGrades()
        {
            return Grades.Count > 0 && Grades.All(grade => grade == 5);
        }

        public bool HasFailingGrades()
        {
            return Grades.Count > 0 && Grades.Any(grade => grade == 2);
        }
    }

    public class Group : IDisplayable
    {
        public string Name;
        public List<Student> Students = new List<Student>();

        public Group(string name, List<Student> students)
        {
            Name = name;
            Students = students ?? new List<Student>();
        }

        public void Display()
        {
            Console.WriteLine($"Группа: {Name}");
            Console.WriteLine($"Количество студентов: {Students.Count}");
            Console.WriteLine($"Средний балл группы: {CalculateGroupAverage():F2}");
            Console.WriteLine($"Отличников: {Students.Count(s => s.HasExcellentGrades())}");
            Console.WriteLine($"С двойками: {Students.Count(s => s.HasFailingGrades())}");
            Console.WriteLine("---");
        }

        public double CalculateGroupAverage()
        {
            return Students.Count > 0 ? Students.Average(s => s.CalculateAverage()) : 0;
        }

        // Метод для применения делегата ко всем студентам группы
        public void ApplyDelegateToAllStudents(StudentDelegate studentDelegate)
        {
            if (studentDelegate == null) return;

            Console.WriteLine($"Применяем делегат к группе {Name}:");
            foreach (var student in Students)
            {
                studentDelegate(student);
            }
        }
    }

    public class Course : IDisplayable
    {
        public int Number;
        public List<Group> Groups = new List<Group>();

        public Course(int number, List<Group> groups)
        {
            Number = number;
            Groups = groups ?? new List<Group>();
        }

        public void Display()
        {
            Console.WriteLine($"Курс: {Number}");
            Console.WriteLine($"Количество групп: {Groups.Count}");
            Console.WriteLine($"Общее количество студентов: {Groups.Sum(g => g.Students.Count)}");
            Console.WriteLine($"Средний балл курса: {CalculateCourseAverage():F2}");
            Console.WriteLine("---");
        }

        public double CalculateCourseAverage()
        {
            var allStudents = Groups.SelectMany(g => g.Students).ToList();
            return allStudents.Count > 0 ? allStudents.Average(s => s.CalculateAverage()) : 0;
        }
    }

    public class Institute : IDisplayable
    {
        public string Name;
        public List<Course> Courses = new List<Course>();

        public Institute(string name, List<Course> courses)
        {
            Name = name;
            Courses = courses ?? new List<Course>();
        }

        public void Display()
        {
            Console.WriteLine($"Институт: {Name}");
            Console.WriteLine($"Количество курсов: {Courses.Count}");
            Console.WriteLine($"Общее количество студентов: {Courses.Sum(c => c.Groups.Sum(g => g.Students.Count))}");
            Console.WriteLine($"Средний балл института: {CalculateInstituteAverage():F2}");
            Console.WriteLine("==========");
        }

        public double CalculateInstituteAverage()
        {
            var allStudents = Courses.SelectMany(c => c.Groups.SelectMany(g => g.Students)).ToList();
            return allStudents.Count > 0 ? allStudents.Average(s => s.CalculateAverage()) : 0;
        }
    }

    class Program
    {
        static List<Institute> institutes = new List<Institute>();

        static void Main()
        {
            InitializeData();

            while (true)
            {
                Console.WriteLine("\n1. Показать все данные");
                Console.WriteLine("2. Показать отличников");
                Console.WriteLine("3. Тест интерфейсов и делегатов");
                Console.WriteLine("4. Найти институты и курсы со средним баллом >3.5");
                Console.WriteLine("5. Отличники 1-2 курсов");
                Console.WriteLine("6. Сохранить все данные в файл");
                Console.WriteLine("7. Выход");
                Console.Write("Выберите пункт: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ShowAllData(); break;
                    case "2": ShowExcellentStudents(); break;
                    case "3": TestInterfacesAndDelegates(); break;
                    case "4": FindInstitutesWithHighAverage(); break;
                    case "5": ShowFirstSecondYearExcellentStudents(); break;
                    case "6": SaveAllDataToFile(); break;
                    case "7": return;
                    default: Console.WriteLine("Нет такого пункта!"); break;
                }
            }
        }

        static void InitializeData()
        {
            var studentsIT1 = new List<Student>
            {
                new Student("Иванов", new List<int> {5, 5, 5, 5}),
                new Student("Петров", new List<int> {4, 5, 4, 4}),
                new Student("Сидоров", new List<int> {3, 4, 3, 4})
            };

            var studentsFIZ1 = new List<Student>
            {
                new Student("Николаев", new List<int> {5, 5, 5, 5}),
                new Student("Орлов", new List<int> {4, 4, 4, 4}),
                new Student("Андреев", new List<int> {2, 3, 3, 2})
            };

            var studentsIT2 = new List<Student>
            {
                new Student("Федоров", new List<int> {5, 5, 4, 5}),
                new Student("Жуков", new List<int> {5, 5, 5, 5}),
                new Student("Виноградов", new List<int> {3, 3, 4, 4})
            };

            var groupsCourse1 = new List<Group>
            {
                new Group("ИТ-101", studentsIT1),
                new Group("ФИЗ-101", studentsFIZ1)
            };

            var groupsCourse2 = new List<Group>
            {
                new Group("ИТ-201", studentsIT2)
            };

            var coursesIT = new List<Course>
            {
                new Course(1, groupsCourse1),
                new Course(2, groupsCourse2)
            };

            var coursesPhysics = new List<Course>
            {
                new Course(1, new List<Group> { groupsCourse1[1] })
            };

            var itInstitute = new Institute("Институт информационных технологий", coursesIT);
            var physicsInstitute = new Institute("Физический институт", coursesPhysics);

            institutes.Add(itInstitute);
            institutes.Add(physicsInstitute);

            Console.WriteLine("Данные успешно инициализированы!");
        }

        static void TestInterfacesAndDelegates()
        {
            Console.WriteLine("\n=== ТЕСТ ИНТЕРФЕЙСОВ И ДЕЛЕГАТОВ ===");

          
            Console.WriteLine("\n1. ТЕСТ ИНТЕРФЕЙСОВ:");

            var testStudent = institutes[0].Courses[0].Groups[0].Students[0];
            Console.WriteLine("Тестируем класс Student (реализует IDisplayable, ICalculatable, IGradeManager):");

     
            Console.WriteLine("\n--- IDisplayable ---");
            testStudent.Display();

            Console.WriteLine("\n--- ICalculatable ---");
            Console.WriteLine($"Средний балл: {testStudent.CalculateAverage()}");

            Console.WriteLine("\n--- IGradeManager ---");
            Console.WriteLine($"Отличник: {testStudent.HasExcellentGrades()}");
            testStudent.AddGrade(5);
            testStudent.Display();

            Console.WriteLine("\n--- IDisplayable в других классах ---");
            institutes[0].Display();
            institutes[0].Courses[0].Display();
            institutes[0].Courses[0].Groups[0].Display();

            Console.WriteLine("\n2. ТЕСТ МНОГОАДРЕСНОГО ДЕЛЕГАТА:");

            StudentDelegate multiDelegate = DisplayStudentBasicInfo;
            multiDelegate += CheckStudentPerformance;
            multiDelegate += AddBonusIfExcellent;

            var testGroup = institutes[0].Courses[0].Groups[0];
            testGroup.ApplyDelegateToAllStudents(multiDelegate);

            Console.WriteLine("\n--- Тест делегата на отдельном студенте ---");
            var singleStudent = institutes[1].Courses[0].Groups[0].Students[1];
            multiDelegate(singleStudent);
        }

        static void DisplayStudentBasicInfo(Student student)
        {
            Console.WriteLine($"Студент: {student.LastName}, Средний балл: {student.CalculateAverage():F2}");
        }

        static void CheckStudentPerformance(Student student)
        {
            string status = student.HasExcellentGrades() ? "ОТЛИЧНИК" :
                           student.HasFailingGrades() ? "ЕСТЬ ДВОЙКИ" : "СТАНДАРТ";
            Console.WriteLine($"Успеваемость: {student.LastName} - {status}");
        }

        static void AddBonusIfExcellent(Student student)
        {
            if (student.HasExcellentGrades())
            {
                student.AddGrade(5);
                Console.WriteLine($"Начислена бонусная оценка отличнику {student.LastName}!");
            }
        }

        static void ShowAllData()
        {
            if (institutes.Count == 0)
            {
                Console.WriteLine("Данных нет!");
                return;
            }

            foreach (var institute in institutes)
            {
                institute.Display();
                foreach (var course in institute.Courses)
                {
                    course.Display();
                    foreach (var group in course.Groups)
                    {
                        group.Display();
                        foreach (var student in group.Students)
                        {
                            student.Display();
                        }
                    }
                }
            }
        }

        static void ShowExcellentStudents()
        {
            Console.WriteLine("ОТЛИЧНИКИ:");
            Console.WriteLine("==========");

            bool found = false;
            foreach (var institute in institutes)
            {
                foreach (var course in institute.Courses)
                {
                    foreach (var group in course.Groups)
                    {
                        var excellentStudents = group.Students.Where(s => s.HasExcellentGrades());
                        foreach (var student in excellentStudents)
                        {
                            student.Display();
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("Отличников не найдено.");
            }
        }

        static void FindInstitutesWithHighAverage()
        {
            Console.WriteLine("Институты и курсы со средним баллом > 3.5:");
            Console.WriteLine("===========================================");

            bool found = false;
            foreach (var institute in institutes)
            {
                foreach (var course in institute.Courses)
                {
                    double courseAverage = course.CalculateCourseAverage();
                    if (courseAverage > 3.5)
                    {
                        Console.WriteLine($"{institute.Name}, курс {course.Number}, средний балл: {courseAverage:F2}");
                        found = true;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("Таких институтов и курсов не найдено.");
            }
        }

        static void ShowFirstSecondYearExcellentStudents()
        {
            Console.WriteLine("Отличники 1-2 курсов:");
            Console.WriteLine("=====================");

            bool found = false;
            foreach (var institute in institutes)
            {
                foreach (var course in institute.Courses.Where(c => c.Number == 1 || c.Number == 2))
                {
                    foreach (var group in course.Groups)
                    {
                        var excellentStudents = group.Students.Where(s => s.HasExcellentGrades());
                        foreach (var student in excellentStudents)
                        {
                            Console.WriteLine($"{institute.Name}, курс {course.Number}, группа {group.Name}");
                            student.Display();
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("Отличников на 1-2 курсах не найдено.");
            }
        }

        static void SaveAllDataToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("alldata.txt", false, System.Text.Encoding.Default))
                {
                    foreach (var institute in institutes)
                    {
                        writer.WriteLine($"Институт: {institute.Name}");
                        foreach (var course in institute.Courses)
                        {
                            writer.WriteLine($" Курс: {course.Number}");
                            foreach (var group in course.Groups)
                            {
                                writer.WriteLine($" Группа: {group.Name} (средний балл: {group.CalculateGroupAverage():F2})");
                                foreach (var student in group.Students)
                                {
                                    writer.WriteLine($"  Студент: {student.LastName}");
                                    writer.WriteLine($"   Оценки: {string.Join(", ", student.Grades)}");
                                    writer.WriteLine($"   Средний балл: {student.CalculateAverage():F2}");
                                    writer.WriteLine($"   Отличник: {student.HasExcellentGrades()}");
                                }
                            }
                        }
                        writer.WriteLine();
                    }
                }
                Console.WriteLine("Все данные сохранены в файл 'alldata.txt'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}