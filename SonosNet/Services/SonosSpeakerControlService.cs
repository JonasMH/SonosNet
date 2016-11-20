using System.Collections.Generic;
using System.Threading.Tasks;
using SonosNet.Models;
using UPnPNet;
using System.Linq;
using SonosNet.Contants;

namespace SonosNet.Services
{
	public class SonosSpeakerControlService : ISonosSpeakerControl
	{
		private const int InstanceId = 0;
		private readonly UPnPServiceControl _avService;
		private readonly UPnPServiceControl _renderControlService;

		public SonosSpeakerControlService(UPnPDevice speakerDevice)
		{
			UPnPService avService = speakerDevice.Services.FirstOrDefault(x => x.Type == UPnPSonosServiceType.AvService);
			UPnPService renderingControl = speakerDevice.Services.FirstOrDefault(x => x.Type == UPnPSonosServiceType.RenderingControlService);

			_avService = new UPnPServiceControl(avService);
			_renderControlService = new UPnPServiceControl(renderingControl);
		}

		public async Task Play()
		{
			await SendAction(_avService, "Play", new KeyValuePair<string, string>("Speed", "1"));
		}

		public async Task Pause()
		{
			await SendAction(_avService, "Pause");
		}

		public async Task Stop()
		{
			await SendAction(_avService, "Stop");
		}

		public async Task Next()
		{
			await SendAction(_avService, "Next");
		}

		public async Task Previous()
		{
			await SendAction(_avService, "Previous");
		}

		public async Task SetVolume(int value)
		{
			if (value < 0 || value > 100)
			{
				return;
			}

			await SendAction(_renderControlService, "SetVolume", new KeyValuePair<string, string>("Channel", "Master"), new KeyValuePair<string, string>("DesiredVolume", value.ToString()));
		}

		public async Task<int> GetVolume()
		{
			IDictionary<string, string> response = await SendAction(_renderControlService, "GetVolume", new KeyValuePair<string, string>("Channel", "Master"));

			return int.Parse(response.FirstOrDefault(x => x.Key == "CurrentVolume").Value);
		}

		private async Task<IDictionary<string, string>> SendAction(UPnPServiceControl service, string command, params KeyValuePair<string, string>[] additionalArguments)
		{
			IDictionary<string, string> arguments = new Dictionary<string, string>() { { "InstanceID", InstanceId.ToString() } };

			if (additionalArguments != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in additionalArguments)
				{
					arguments.Add(keyValuePair);
				}
			}

			return await service.SendAction(command, arguments);
		}
	}
}
