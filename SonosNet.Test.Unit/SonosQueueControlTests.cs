using System.IO;
using System.Reflection;
using SonosNet.Services;
using Xunit;

namespace SonosNet.Test.Unit
{
	public class SonosQueueControlTests
	{
		private string LoadXML()
		{
			return File.ReadAllText("./SonosQueue_Browse_Example.xml");
		}

		[Fact]
		public void Parse_ReadXml_ShouldContainTenItems()
		{
			string xml = LoadXML();

			DidlParser parser = new DidlParser();

			var result = parser.Parse(xml);

			Assert.Equal(36, result.Count);
		}

		[Fact]
		public void Parse_ReadXml_FirstSongCorrectCreator()
		{
			string xml = LoadXML();

			DidlParser parser = new DidlParser();

			var result = parser.Parse(xml);

			Assert.Equal("Shockerz ft. Kirsten", result[0].Creator);
		}
	}
}
