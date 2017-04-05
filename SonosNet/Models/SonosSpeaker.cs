using SonosNet.Services;

namespace SonosNet.Models
{
	public class SonosSpeaker
	{
		public string Name { get; set; }
		public string Uuid { get; set; }
		public SonosSpeakerControlService Control { get; set; }
	}
}