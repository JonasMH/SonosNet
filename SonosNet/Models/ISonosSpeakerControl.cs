using System.Threading.Tasks;

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
	}
}
