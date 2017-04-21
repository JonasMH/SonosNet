using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SonosNet.Contants;
using UPnPNet;

namespace SonosNet.Services
{
	public class SonosQueueItem
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Creator { get; set; }	
		public string Album { get; set; }
		public string AlbumArtURI { get; set; }
		public string Duration { get; set; }

		public override string ToString()
		{
			return Title;
		}
	}

	public static class SonosQueueControl
	{

		public static async Task<List<SonosQueueItem>> GetQueue(this SonosSpeakerControlService service, int startIndex = 0, int maxItems = 100)
		{
			UPnPServiceControl serviceControl = new UPnPServiceControl(service.GetMediaServerService(UPnPSonosServiceTypes.ContentDirectory));
			IDictionary<string, string> actionResult = await serviceControl.SendAction("Browse",
					new Dictionary<string, string>()
					{
						{"ObjectID", "Q:0"},
						{"BrowseFlag", "BrowseDirectChildren"},
						{"Filter", "*"},
						{"StartingIndex", startIndex.ToString()},
						{"RequestedCount", maxItems.ToString()},
						{"SortCriteria", ""}
					});
			
			return new DidlParser().Parse(actionResult.FirstOrDefault(x => x.Key == "Result").Value);
		}
	}

	public class DidlParser
	{
		public List<SonosQueueItem> Parse(string xml)
		{
			List<SonosQueueItem> resultList = new List<SonosQueueItem>();

			XDocument doc = XDocument.Parse(xml);

			foreach (XElement element in doc.Root.Elements())
			{
				SonosQueueItem item = new SonosQueueItem
				{
					Id = element.Attributes().FirstOrDefault(x => x.Name.LocalName == "id").Value,
					Title = element.Elements().FirstOrDefault(x => x.Name.LocalName == "title").Value,
					Creator = element.Elements().FirstOrDefault(x => x.Name.LocalName == "creator").Value,
					Album = element.Elements().FirstOrDefault(x => x.Name.LocalName == "album").Value,
					AlbumArtURI = element.Elements().FirstOrDefault(x => x.Name.LocalName == "albumArtURI").Value,
					Duration =
						element.Elements()
							.FirstOrDefault(x => x.Name.LocalName == "res")
							.Attributes()
							.FirstOrDefault(x => x.Name.LocalName == "duration")
							.Value
				};

				resultList.Add(item);
			}

			return resultList;
		}
	}
}
