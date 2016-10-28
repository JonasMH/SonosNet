using System.Collections.Generic;
using UPnPNet;

namespace SonosNet
{
	public class SonosSpeaker
	{
		private readonly UPnPServiceControl _serviceControl;
		private const int InstanceId = 0;

		public string Name { get; set; }

		internal SonosSpeaker(UPnPServiceControl control)
		{
			_serviceControl = control;
		}

		public async void Play()
		{
			await _serviceControl.SendAction("Play", new Dictionary<string, string>() { { "InstanceID", InstanceId.ToString() }, { "Speed", "1" } });
		}

		public async void Pause()
		{
			await _serviceControl.SendAction("Pause", new Dictionary<string, string>() { { "InstanceID", InstanceId.ToString() } });
		}

		public async void Stop()
		{
			await _serviceControl.SendAction("Stop", new Dictionary<string, string>() { { "InstanceID", InstanceId.ToString() } });
		}

		public async void Next()
		{
			await _serviceControl.SendAction("Next", new Dictionary<string, string>() { { "InstanceID", InstanceId.ToString() } });
		}
	}
}