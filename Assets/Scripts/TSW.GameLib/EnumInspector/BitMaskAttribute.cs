using UnityEngine;

namespace TSW
{
	public class BitMaskAttribute : PropertyAttribute
	{
		public System.Type propType;
		public BitMaskAttribute(System.Type aType)
		{
			propType = aType;
		}
	}
}