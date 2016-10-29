using System.Collections.Generic;
using System.Threading.Tasks;
using UPnPNet;

namespace SonosNet
{
	public class SonosSpeaker
	{
		private readonly UPnPServiceControl _serviceControl;
		private const int InstanceId = 0;

		public string Name { get; set; }
		public string Uuid { get; set; }

		internal SonosSpeaker(UPnPServiceControl control)
		{
			_serviceControl = control;
		}

		public async Task Play()
		{
			await SendAction("Play", new Dictionary<string, string>() { { "Speed", "1" } });
		}

		public async Task Pause()
		{
			await SendAction("Pause");
		}

		public async Task Stop()
		{
			await SendAction("Stop");
		}

		public async Task Next()
		{
			await SendAction("Next");
		}

		public async Task Previous()
		{
			await SendAction("Previous");
		}

		private async Task SendAction(string command, IDictionary<string, string> additionalArguments = null)
		{
			IDictionary<string, string> arguments = new Dictionary<string, string>() {{"InstanceID", InstanceId.ToString()}};

			if (additionalArguments != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in additionalArguments)
				{
					arguments.Add(keyValuePair);
				}
			}

			await _serviceControl.SendAction(command, arguments);
		}
	}
}