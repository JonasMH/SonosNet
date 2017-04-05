using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UPnPNet.Server;

namespace SonosNet.Models
{
	public interface ISonosSpeakerControl
	{
		Task Play();
		Task Pause();
		Task Next();
		Task Previous();
		Task Stop();
		Task SetVolume(int value);
		Task<int> GetVolume();

		void SubscribeToEvents(UPnPServer server);
		event EventHandler<KeyValuePair<string, string>> OnUpdate;
		Task AddURITOQueue();
	}
}
