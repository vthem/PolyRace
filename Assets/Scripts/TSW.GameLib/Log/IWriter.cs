namespace TSW
{
	namespace Log
	{
		public interface IWriter
		{
			void Log(string text);
			void Close();
		}
	}
}

