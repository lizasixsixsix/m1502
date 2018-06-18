using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m1502.Task.TestHelpers
{
	public abstract class SerializationTester<TData, TSerializer>
	{
		protected TSerializer serializer;
		bool showResult;

		public SerializationTester(TSerializer serializer, bool showResult = false)
		{
			this.serializer = serializer;
			this.showResult = showResult;
		}

		public TData SerializeAndDeserialize(TData data)
		{
			var stream = new MemoryStream();

			Console.WriteLine("Start serialization");
			Serialization(data, stream);
			Console.WriteLine("Serialization finished");

			if (showResult)
			{
				var r = Console.OutputEncoding.GetString(stream.GetBuffer(), 0, (int)stream.Length);
				Console.WriteLine(r);
			}

			stream.Seek(0, SeekOrigin.Begin);
			Console.WriteLine("Start deserialization");
			TData result = Deserialization(stream);
			Console.WriteLine("Deserialization finished");

			return result;
		}

		internal abstract TData Deserialization(MemoryStream stream);
		internal abstract void Serialization(TData data, MemoryStream stream);
	}
}
