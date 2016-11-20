using System;
using System.Collections.Generic;
using System.Linq;
using SonosNet.Models;
using SonosNet.Services;

namespace SonosNet.Cli
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IList<SonosSpeaker> speakers = new SonosDiscoveryService().FindSpeakers().Result;

			foreach (SonosSpeaker sonosSpeaker in speakers)
			{
				Console.WriteLine(sonosSpeaker.Name + " (" + sonosSpeaker.Uuid + ")");
			}


			SonosSpeaker speaker = speakers.FirstOrDefault();
			Console.WriteLine("Controlling " + speaker.Name);

			while (true)
			{
				string line = Console.ReadLine();

				if (line.StartsWith("a"))
				{
					speaker.Control.Play();
				}
				else if (line.StartsWith("s"))
				{
					speaker.Control.Pause();
				}
				else if (line.StartsWith("v"))
				{
					speaker.Control.SetVolume(int.Parse(line.Remove(0, 1).Trim()));
				}
				else if (line.StartsWith("b"))
				{
					Console.WriteLine(speaker.Control.GetVolume().Result);
				}
			}
		}
	}
}
