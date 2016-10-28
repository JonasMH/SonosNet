using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonosNet.Cli
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IList<SonosSpeaker> speakers = new SonosDiscovery().FindSpeakers().Result;
			SonosSpeaker jonasSonosSpeaker = speakers.FirstOrDefault(x => x.Name.Contains("Jonas"));

			if (jonasSonosSpeaker != null)
			{
				Console.WriteLine("Playing");
				jonasSonosSpeaker.Play();

				Task.Delay(5000).Wait();

				Console.WriteLine("Pausing");
				jonasSonosSpeaker.Pause();
			}

			Console.ReadKey();
		}
	}
}
