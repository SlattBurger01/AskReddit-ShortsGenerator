using System.Diagnostics;

namespace ProjectCarrot
{
    public static class PythonHelper
    {
        /// <summary> Starts new process of 'file' (path) with 'args' </summary>
        /// <param name="file"> Path to python file that is going to be executed </param>
        /// <returns> Whatever file printed into console </returns>
        public static string CallPythonFile(string file, string args)
        {
            Debug.WriteLine($"Calling python file {file}!");

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = LocalPaths.pythonPath;
            start.Arguments = file + $" {args}";

            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            string result = Process.Start(start).StandardOutput.ReadToEnd();
            return result;
        }
    }
}