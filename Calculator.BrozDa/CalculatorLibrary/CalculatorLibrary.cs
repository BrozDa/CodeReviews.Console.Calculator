﻿using System.Diagnostics;
using System.Dynamic;
using Newtonsoft.Json;
namespace CalculatorLibrary
{
    public class Calculator
    {
        private int _numberOfUses;
        private JsonWriter _writer;
        private List<string> _calculationHistory;
        private List<double> _calculationHistoryResults;
        public bool GettingResultFromHistory { get; set; }
        public Calculator()
        {
            StreamWriter logFile = File.CreateText("calculatorlog.json");
            logFile.AutoFlush = true;
            _writer = new JsonTextWriter(logFile);
            _writer.Formatting = Formatting.Indented;
            InitializeJSONLog();
            _numberOfUses = GetNumberOfUses();
            _calculationHistory = new List<string>();
            _calculationHistoryResults = new List<double>();
            GettingResultFromHistory = false;

        }
        private int GetNumberOfUses()
        {
            StreamReader readNumberOfUses = new StreamReader("NumberOfUses.txt");
            int numberOfUses = Convert.ToInt32(readNumberOfUses.ReadLine());
            readNumberOfUses.Close();
            return numberOfUses;
        }
        private void WriteNumberOfUses(int numberOfUses)
        {
            StreamWriter writeNumberOfUses = new StreamWriter("NumberOfUses.txt");
            writeNumberOfUses.Write(numberOfUses);
            writeNumberOfUses.Close();
        }
        private void InitializeJSONLog()
        {
            _writer.WriteStartObject();
            _writer.WritePropertyName("Operations");
            _writer.WriteStartArray();
        }

        private void AddOperandsToJSON(double num1, double num2)
        {
            _writer.WriteStartObject();
            _writer.WritePropertyName("Operand1");
            _writer.WriteValue(num1);
            _writer.WritePropertyName("Operand2");
            _writer.WriteValue(num2);
            _writer.WritePropertyName("Operation");
        }
        private void AddResultToJSON(double result)
        {
            _writer.WritePropertyName("Result");
            _writer.WriteValue(result);
            _writer.WriteEndObject();
        }

        public double DoOperation(double num1, double num2, string op)
        {
            double result = double.NaN; // Default value is "not-a-number" if an operation, such as division, could result in an error.
            AddOperandsToJSON(num1, num2);
            // Use a switch statement to do the math.
            switch (op)
            {
                case "a":
                    result = num1 + num2;
                    _writer.WriteValue("Add");
                    _calculationHistory.Add($"{num1} + {num2} = {result}");
                    break;
                case "s":
                    result = num1 - num2;
                    _writer.WriteValue("Subtract");
                    _calculationHistory.Add($"{num1} - {num2} = {result}");
                    break;
                case "m":
                    result = num1 * num2;
                    _writer.WriteValue("Multiply");
                    _calculationHistory.Add($"{num1} * {num2} = {result}");
                    break;
                case "d":
                    // Ask the user to enter a non-zero divisor.
                    if (num2 != 0)
                    {
                        result = num1 / num2;
                    }
                    _calculationHistory.Add($"{num1} / {num2} = {result}");
                    _writer.WriteValue("Divide");
                    break;
                // Return text for an incorrect option entry.
                default:
                    break;
            }
            _calculationHistoryResults.Add(result); 
            AddResultToJSON(result);
            _numberOfUses++;
            return result;
        }
        public void PrintHistory()
        {
            Console.WriteLine("Calculation history:");
            if (_calculationHistory == null || _calculationHistory.Count == 0)
            {
                Console.WriteLine("No records in history");
            }
            else
            {
                for(int i = 0; i < _calculationHistory.Count; i++)
                {
                    Console.WriteLine($"{i + 1}: {_calculationHistory[i]}");
                }
                
            }
            Console.WriteLine();
        }
        public void Finish()
        {
            _writer.WriteEndArray();
            _writer.WriteEndObject();
            _writer.Close();
            WriteNumberOfUses(_numberOfUses);
        }
        public void PrintTitle()
        {
            Console.WriteLine("Welsome to Console Calculator in C#\r");
            Console.WriteLine($"Calculator was used to solve {_numberOfUses} problems so far");
            Console.WriteLine("------------------------\n");
        }
        public void OptionsMenu()
        {
            PrintOptionsMenu();
            ProcessOptionMenu();
        }
        private void PrintOptionsMenu()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            PrintTitle();
            Console.WriteLine("Press coresponding number and [Enter] for selection: ");
            Console.WriteLine("1. Show History");
            Console.WriteLine("2. Delete History");
            Console.WriteLine("3. Return to calculations");
            Console.WriteLine("------------------------");
        }
        private int GetAndValidateMenuInput()
        {
            string? selection;
            int numericSelection;
            Console.Write("Please enter your choice: ");
            selection = Console.ReadLine();

            while (int.TryParse(selection, out numericSelection) == false || (numericSelection < 1 || numericSelection > 3))
            {
                Console.Write("Please enter valid choice: ");
                selection = Console.ReadLine();
            }


            return numericSelection;

        }

        private void ProcessOptionMenu()
        {
            bool wantToStayInMenu = true;
            
            while (wantToStayInMenu) 
            {
                int selection = GetAndValidateMenuInput();

                switch (selection)
                {
                    case 1:
                        PrintHistory();
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        PrintOptionsMenu();
                        break;
                    case 2:
                        _calculationHistory.Clear();
                        _calculationHistoryResults.Clear();
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        PrintOptionsMenu();
                        break;
                    case 3:
                        wantToStayInMenu = false;
                        break;

                }
                
            }
            
        }
        public double GetNumber(int position)
        {
            string? numInput;
            double cleanNum = 0;

            if (position == 1)
            {
                Console.Write("Enter fisrt number folowed by an [Enter] key: ");
            }
            else if (position == 2)
            {
                Console.Write("Enter second number folowed by an [Enter] key: ");
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid number position passed to GetNumber()");
            }

            numInput = Console.ReadLine();
            while (!double.TryParse(numInput, out cleanNum))
            {
                Console.Write("This is not valid input. Please enter a numeric value: ");
                numInput = Console.ReadLine();
            }

            return cleanNum;
        }
        public double GetNumberFromResult()
        {
            string? numInput;
            int index;

            Console.Write("Enter index number of result you'd like to use: ");
            numInput = Console.ReadLine();
            while (!int.TryParse(numInput, out index) || index-1 < 0 || index-1 > _calculationHistoryResults.Count)
            {
                Console.Write("This is not valid input. Please enter a valid numeric value: ");
                numInput = Console.ReadLine();
            }
            Console.WriteLine("First Number for calculation: " + _calculationHistoryResults[index - 1]);


            return _calculationHistoryResults[index-1];
        }

    }
}
