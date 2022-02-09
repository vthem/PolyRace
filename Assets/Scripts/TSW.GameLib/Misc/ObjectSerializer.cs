using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TSW
{
	public static class ObjectSerializer
	{
		public static byte[] Serialize(object obj)
		{
			MemoryStream ms = new MemoryStream();
			BinaryFormatter serializer = new BinaryFormatter();
			serializer.Serialize(ms, obj);
			return ms.ToArray();
		}

		public static T Deserialize<T>(byte[] data) where T : class
		{
			MemoryStream decodeBuffer = new MemoryStream(data);
			BinaryFormatter serializer = new BinaryFormatter();
			object obj = serializer.Deserialize(decodeBuffer);
			return obj as T;
		}

		public static string SerializeBase64(object obj)
		{
			return Convert.ToBase64String(Serialize(obj));
		}

		public static T DeserializeBase64<T>(string data) where T : class
		{
			return Deserialize<T>(Convert.FromBase64String(data));
		}

		public static string SerializeLZFBase64(object obj)
		{
			byte[] data = Serialize(obj);
			byte[] compress = CompressLZFHelper.CompressBytes(data);
			string base64 = Convert.ToBase64String(compress);
			return base64;
		}

		public static byte[] SerializeLZF(object obj)
		{
			byte[] data = Serialize(obj);
			byte[] compress = CompressLZFHelper.CompressBytes(data);
			return compress;
		}

		public static T DeserializeLZFBase64<T>(string base64) where T : class
		{
			MemoryStream decodeBuffer = new MemoryStream(CompressLZFHelper.DecompressBytes(Convert.FromBase64String(base64)));
			BinaryFormatter serializer = new BinaryFormatter();
			return serializer.Deserialize(decodeBuffer) as T;
		}

		public static T DeserializeLZF<T>(byte[] data) where T : class
		{
			MemoryStream decodeBuffer = new MemoryStream(CompressLZFHelper.DecompressBytes(data));
			BinaryFormatter serializer = new BinaryFormatter();
			return serializer.Deserialize(decodeBuffer) as T;
		}
	}
}