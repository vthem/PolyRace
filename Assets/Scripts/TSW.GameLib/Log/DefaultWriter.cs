using UnityEngine;


namespace TSW.Log
{
	public class DefaultWriter : IWriter
	{

		public void Log(string text)
		{
			Debug.Log(text);
		}

		public void Close()
		{

		}
	}
}
