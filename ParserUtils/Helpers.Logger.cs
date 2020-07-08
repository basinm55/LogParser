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
        public string LoadingFilePath { get; set; }

        public int ReportedLinesCount;
        private bool _isActive { get; set; }

        public ParserLogger(bool isActive = true)
        {
            _isActive = isActive;
        }

        public override void Log(string message, int lineNum = -1) 
        {
            if (!_isActive) return;
            
            using (StreamWriter sw = new StreamWriter(File.Open(TargetPath, FileMode.Append)))
            {
                if (lineNum <= 0)
                    sw.WriteLine(string.Format("{0}", message));
                else
                    sw.WriteLine(string.Format("Ln {0}:\t {1}", lineNum, message));

                sw.Close();
            }
        }

        public void LogStartLoadingStarted()
        {
            if (!_isActive) return;

            if (!string.IsNullOrWhiteSpace(LoadingFilePath))
                Log(string.Format("Load file {0} - started at {1}",
                                Path.GetFileName(Path.GetFileName(LoadingFilePath)),
                                DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff")));
        }

        public void LogLoadingCompleted()
        {
            if (!_isActive) return;

            if (!string.IsNullOrWhiteSpace(LoadingFilePath))
                Log(string.Format("Load file {0} - completed at {1}",
                                Path.GetFileName(Path.GetFileName(LoadingFilePath)),
                                DateTime.Now.ToString("dd/MM/yy HH:mm:ss.fff")));
        }

        public void LogLine(string message, int lineNum)
        {
            if (!_isActive) return;

            Log(message, lineNum);

            ReportedLinesCount++;
        }

        public void LogException(Exception ex, int lineNum = -1)
        {
            if (!_isActive) return;

            if (lineNum <= 0)
                Log(ex.ToString(), lineNum);
            else
                Log(ex.ToString());

            ReportedLinesCount++;
        }
    }
}
