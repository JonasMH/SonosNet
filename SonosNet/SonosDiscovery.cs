using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UPnPNet;
using UPnPNet.Discovery;
using UPnPNet.Discovery.SearchTargets;

namespace SonosNet
{
	public class SonosDiscovery
	{
		private readonly Regex _nameRegex;

		public SonosDiscovery()
		{
			_nameRegex = new Regex("(.*) -.*");
		}

		public async Task<IList<SonosSpeaker>> FindSpeakers()
		{
			UPnPDiscovery discovery = new UPnPDiscovery { SearchTarget = DiscoverySearchTargets.ServiceTypeSearch("AVTransport", "1") };
			IList<UPnPDevice> devices = await discovery.Search();

			IEnumerable<UPnPDevice> sonosDevices = devices.Where(x => x.Properties["friendlyName"].ToLower().Contains("sonos"));
			IList<SonosSpeaker> speakers = new List<SonosSpeaker>();
			const string serviceType = "urn:schemas-upnp-org:service:AVTransport:1";

			foreach (UPnPDevice sonosDevice in sonosDevices)
			{
				UPnPDevice subDevice =
					sonosDevice.SubDevices.FirstOrDefault(
						x => x.Services.Any(y => y.Type == serviceType));
				
				UPnPService avService = sonosDevice.SubDevices.SelectMany(x => x.Services).FirstOrDefault(x => x.Type == serviceType);
				SonosSpeaker speaker = new SonosSpeaker(new UPnPServiceControl(avService))
				{
					Name = GetName(subDevice.Properties["friendlyName"]),
					Uuid = subDevice.Properties["UDN"].Replace("uuid:", "")
				};
				
				speakers.Add(speaker);
			}

			return speakers;
		}

		private string GetName(string friendlyName)
		{
			Match match = _nameRegex.Match(friendlyName);
			return match.Groups[1].Value;
		}
	}
}
