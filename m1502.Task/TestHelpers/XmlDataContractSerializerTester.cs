using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace m1502.Task.TestHelpers
{
	public class XmlDataContractSerializerTester<T> : SerializationTester<T, XmlObjectSerializer>
	{
		public XmlDataContractSerializerTester(
			XmlObjectSerializer serializer, bool showResult = false) : base(serializer, showResult)
		{ }

		internal override T Deserialization(MemoryStream stream)
		{
			return (T)serializer.ReadObject(stream);
		}

		internal override void Serialization(T data, MemoryStream stream)
		{
			serializer.WriteObject(stream, data);
		}
	}
}
