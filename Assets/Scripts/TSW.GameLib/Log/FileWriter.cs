using System.IO;

#if UNITY_EDITOR
#endif

namespace TSW.Log
{
	public class FileWriter : IWriter
	{
		private readonly string _logFilename;
		private readonly StreamWriter _streamWriter;

		public FileWriter(string logFilename)
		{
			_logFilename = logFilename;
			_streamWriter = new StreamWriter(_logFilename, false)
			{
				AutoFlush = true
			};
		}

		public void Log(string text)
		{
			_streamWriter.WriteLine(text);
		}

		public void Close()
		{
			_streamWriter.Close();
		}

	}
}
