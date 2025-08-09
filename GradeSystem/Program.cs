using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // --- Custom Exceptions ---
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // --- Student Class ---
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    // --- Student Result Processor ---
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 1;

                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        var parts = line.Split(',');

                        if (parts.Length != 3)
                            throw new MissingFieldException($"Line {lineNumber}: Missing fields.");

                        if (!int.TryParse(parts[0].Trim(), out int id))
                            throw new FormatException($"Line {lineNumber}: Invalid ID format.");

                        string fullName = parts[1].Trim();

                        if (string.IsNullOrWhiteSpace(fullName))
                            throw new MissingFieldException($"Line {lineNumber}: Missing student name.");

                        if (!int.TryParse(parts[2].Trim(), out int score))
                            throw new InvalidScoreFormatException($"Line {lineNumber}: Score format is invalid.");

                        if (score < 0 || score > 100)
                            throw new InvalidScoreFormatException($"Line {lineNumber}: Score out of valid range (0–100).");

                        students.Add(new Student(id, fullName, score));
                    }
                    catch (MissingFieldException)
                    {
                        throw;
                    }
                    catch (InvalidScoreFormatException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Line {lineNumber}: {ex.Message}");
                    }
                    finally
                    {
                        lineNumber++;
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    // --- Main Program ---
    class Program
    {
        static void Main()
        {
            var processor = new StudentResultProcessor();
            string inputFilePath = "students.txt";       // input file path
            string outputFilePath = "report.txt";        // output file path

            try
            {
                var students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine("Report generated successfully!");
                Console.WriteLine($"Output file: {Path.GetFullPath(outputFilePath)}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: Input file not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }
    }
}

