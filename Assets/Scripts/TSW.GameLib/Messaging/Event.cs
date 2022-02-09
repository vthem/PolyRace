namespace TSW.Messaging
{
	public class Event
	{
	}

	public class Event<T1> : Event
	{
		public T1 Value1 { get; private set; }
		public Event<T1> SetValue1(T1 data)
		{
			Value1 = data;
			return this;
		}
	}

	public class Event<T1, T2> : Event
	{
		public T1 Value1 { get; private set; }
		public Event<T1, T2> SetValue1(T1 data)
		{
			Value1 = data;
			return this;
		}
		public T2 Value2 { get; private set; }
		public Event<T1, T2> SetValue2(T2 data)
		{
			Value2 = data;
			return this;
		}
	}

}
