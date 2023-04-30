using System.Diagnostics;

namespace ProjectCarrot
{
    public static class PythonHelper
    {
        public static string CallPythonFile(string file, string args)
        {
            Debug.WriteLine("Calling python file !");

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python39_64\python.exe";
            start.Arguments = file + $" {args}";

            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            string result = Process.Start(start).StandardOutput.ReadToEnd();
            Console.WriteLine(result);
            return result;
        }
    }
}