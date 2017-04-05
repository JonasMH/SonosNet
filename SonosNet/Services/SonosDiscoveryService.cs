using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SonosNet.Contants;
using SonosNet.Models;
using UPnPNet.Discovery;
using UPnPNet.Discovery.SearchTargets;
using UPnPNet.Models;

namespace SonosNet.Services
{
	public class SonosDiscoveryService
	{
		private readonly Regex _nameRegex = new Regex("(.*) -.*");
		
		public async Task<IList<SonosSpeaker>> FindSpeakers()
		{
			UPnPDiscovery discovery = new UPnPDiscovery { SearchTarget = DiscoverySearchTargetFactory.ServiceTypeSearch("AVTransport", "1") };
			IList<UPnPDevice> devices = await discovery.Search();

			IEnumerable<UPnPDevice> sonosDevices = devices.Where(x => x.Properties["friendlyName"].ToLower().Contains("sonos"));
			IList<SonosSpeaker> speakers = new List<SonosSpeaker>();



			foreach (UPnPDevice sonosDevice in sonosDevices)
			{
				UPnPDevice device = sonosDevice
				.SubDevices
				.FirstOrDefault(x => x.Services.Any(y => y.Type == UPnPSonosServiceTypes.AvService));


				speakers.Add(new SonosSpeaker
				{
					Name = GetName(device.Properties["friendlyName"]),
					Uuid = sonosDevice.Properties["UDN"].Replace("uuid:", ""),
					Control = new SonosSpeakerControlService(sonosDevice)
				});
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
