using System;
using System.Collections.Generic;
using System.Linq;

namespace SonosNet.Cli
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IList<SonosSpeaker> speakers = new SonosDiscovery().FindSpeakers().Result;

			foreach (SonosSpeaker sonosSpeaker in speakers)
			{
				Console.WriteLine(sonosSpeaker.Name + " (" + sonosSpeaker.Uuid + ")");
			}

			Console.WriteLine("Controlling jonas's");

			SonosSpeaker jonasSonosSpeaker = speakers.FirstOrDefault(x => x.Name.Contains("Jonas"));

			while (true)
			{
				ConsoleKeyInfo key = Console.ReadKey();

				switch (key.Key)
				{
					case ConsoleKey.A:
						jonasSonosSpeaker.Play();
						break;
					case ConsoleKey.S:

						jonasSonosSpeaker.Pause();
						break;
				}
			}
		}
	}
}
