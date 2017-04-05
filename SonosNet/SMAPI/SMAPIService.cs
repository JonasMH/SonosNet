using System.Collections.Generic;
using System.Threading.Tasks;
using UPnPNet.Soap;

namespace SonosNet.SMAPI
{
	public class SMAPIService
	{

		public async Task<SoapResponse> Search(string url, string category, string term, int index = 0, int count = 100)
		{
			SoapClient client = new SoapClient()
			{
				BaseAddress = new System.Uri(url)
			};


			var request = new SoapRequest()
			{
				Action = "search",
				Arguments = new Dictionary<string, string>
				{
					{"id", category},
					{"term", term},
					{"index", index.ToString()},
					{"count", count.ToString()},
				},
				ControlUrl = "",
				ServiceType = "http://www.sonos.com/Services/1.1"
			};

			return await client.SendAsync(request);
		}
	}
}
