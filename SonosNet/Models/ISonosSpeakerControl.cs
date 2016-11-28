using System;
using System.Threading.Tasks;
using UPnPNet.Gena;

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

		void SubscribeToEvents(string notifyUrl, GenaSubscriptionHandler handler, int timeout = 3600);
		event EventHandler<int> OnVolumeUpdate;
	}
}
