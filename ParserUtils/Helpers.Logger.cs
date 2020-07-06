using System;
using System.IO;

namespace Helpers
{
    public abstract class LogBase
    {
        public abstract void Log(string message, int parsedLineNum = -1);
    }

    public class ParserLogger : LogBase
    {
        public string TargetPath {  get; set; }
        public string LodingFilePath { get; set; }

        public override void Log(string message, int parsedLineNum = -1) 
        {
            using (StreamWriter sw = new StreamWriter(File.Open(TargetPath, FileMode.Append)))
            {
                if (parsedLineNum <= 0)
                    sw.WriteLine(string.Format("{0}", message));
                else
                    sw.WriteLine(string.Format("Ln {0}: {1}", parsedLineNum, message));

                sw.Close();
            }
        }

        public void LogStartLoadingStarted()
        {
            if (!string.IsNullOrWhiteSpace(LodingFilePath))
                Log(string.Format("Load file {0} - started at {1}",
                                Path.GetFileName(Path.GetFileName(LodingFilePath)),
                                DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff")));
        }

        public void LogLoadingCompleted()
        {
            if (!string.IsNullOrWhiteSpace(LodingFilePath))
                Log(string.Format("Load file {0} - completed at {1}",
                                Path.GetFileName(Path.GetFileName(LodingFilePath)),
                                DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff")));
        }

        public void LogLine(string message, int parsedLineNum)
        {
            Log(message, parsedLineNum);
        }
    }
}
