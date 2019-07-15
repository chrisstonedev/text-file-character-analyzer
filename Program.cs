using System;
using System.IO;
using TextFileCharacterAnalyzer.Resources;

namespace TextFileCharacterAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Allow the ability to pass in an application argument of a file path.
            string pathFromArgs = null;
            if (args.Length >= 1)
            {
                pathFromArgs = args[0];
            }

            // Put the entire application in a while loop.
            // It keeps going until the user enters a blank file path.
            while (true)
            {
                StreamReader sr;
                // Create an infinite loop to give the user multiple chances to enter a valid file name.
                while (true)
                {
                    // Allow a user to set the path for the first attempt from an application argument.
                    string filePath = pathFromArgs;
                    if (filePath == null)
                    {
                        // Prompt the user to enter a file path.
                        Console.WriteLine(Strings.EnterFullFilePath);
                        filePath = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(filePath))
                        {
                            // Nothing was entered. It must be safe to close the application.
                            return;
                        }
                    }
                    try
                    {
                        // Try to create a new StreamReader.
                        // If this file path came from an application argument, let the exception throw.
                        // If this file path came from user entry, show a nicer error and let them try again.
                        sr = new StreamReader(filePath);
                        break;
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine(Strings.FileNotFound);
                        if (pathFromArgs != null) throw; else continue;
                    }
                    catch (IOException)
                    {
                        Console.WriteLine(Strings.IOAccessException);
                        if (pathFromArgs != null) throw; else continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine(Strings.UnauthorizedAccess);
                        if (pathFromArgs != null) throw; else continue;
                    }
                }

                // We are going to keep track of the line and column in which a "bad" character is found.
                // Initialize this integer at zero so, when we increment it each time, it actually starts at 1.
                int activeLine = 0;
                // Loop until the StreamReader reaches the end of the stream (file).
                while (!sr.EndOfStream)
                {
                    // Read the file one line at a time.
                    string line = sr.ReadLine();
                    activeLine++;

                    // This variable will help us if a "bad" character is found multiple times consecutively.
                    int firstColumnInRange = 0;

                    // Loop through all characters, using its "column" position instead of the index.
                    for (int activeColumn = 1; activeColumn <= line.Length; activeColumn++)
                    {
                        char currentChar = line[activeColumn - 1];
                        // ' ' (32) and '~' (126) are considered the lower and upper bounds of what is
                        // considered a "good" character in this application.
                        if (currentChar < ' ' || currentChar > '~')
                        {
                            // Inside this if statement, we know the character in focus now is a "bad" one.
                            // If this value is 0, then it means this is the character preceding it on this line was differen.t
                            if (firstColumnInRange == 0)
                                firstColumnInRange = activeColumn;

                            // If this is the last character on the line or if we know that the next one is different, print the details.
                            // If the next character is the same one, hold off on reporting it because it will be a range..
                            if (activeColumn == line.Length || line[activeColumn] != currentChar)
                            {
                                // Display the column(s) conditionally as a single value or as a range.
                                string columnsText = firstColumnInRange != activeColumn
                                    ? $"Cols {firstColumnInRange}-{activeColumn}"
                                    : $"Col {activeColumn}";
                                Console.WriteLine($"{currentChar} ({(int)currentChar}) {Strings.CharacterFound}: Ln {activeLine}, {columnsText}");

                                // Reset this value to 0 for the next one.
                                firstColumnInRange = 0;
                            }
                        }
                    }
                }

                // Report successful analysis and dispose of StreamReader as it extends IDisposable.
                Console.WriteLine(Strings.AnalysisComplete + "\r\n");
                sr.Dispose();

                // If a command-line argument was used, do not loop back to the beginning.
                if (pathFromArgs != null)
                    return;
            }
        }
    }
}
