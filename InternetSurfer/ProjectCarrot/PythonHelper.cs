using System.Diagnostics;

namespace ProjectCarrot
{
    public static class PythonHelper
    {
        public static string CallPythonFile(string file, string args)
        {
            Debug.WriteLine($"Calling python file {file}!");

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = Paths.pythonPath;
            start.Arguments = file + $" {args}";

            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            string result = Process.Start(start).StandardOutput.ReadToEnd();
            Console.WriteLine(result);
            return result;
        }
    }
}