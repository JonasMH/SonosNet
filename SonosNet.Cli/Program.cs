using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SonosNet.Models;
using SonosNet.Services;
using UPnPNet;

namespace SonosNet.Cli
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Run();
		}

		private static void Run()
		{
			IList<SonosSpeaker> speakers = new SonosDiscoveryService().FindSpeakers().Result;

			foreach (SonosSpeaker sonosSpeaker in speakers)
			{
				Console.WriteLine(sonosSpeaker.Name + " (" + sonosSpeaker.Uuid + ")");
			}

			SonosSpeaker speaker = speakers.FirstOrDefault(x => x.Name.StartsWith("J"));

			speaker.Control.OnUpdate += Control_OnUpdate;
			Console.WriteLine("Controlling " + speaker.Name);


			UPnPServer server = new UPnPServer();
			server.Start(new IPEndPoint(IPAddress.Parse("172.16.1.30"), 24452));

			speaker.Control.SubscribeToEvents(server);
			var queue = speaker.Control.GetQueue().Result;

			while (true)
			{
				string line = Console.ReadLine();

				if (line.StartsWith("a"))
				{
					speaker.Control.Play().Wait();
				}
				else if (line.StartsWith("s"))
				{
					speaker.Control.Pause().Wait();
				}
				else if (line.StartsWith("v"))
				{
					speaker.Control.SetVolume(int.Parse(line.Remove(0, 1).Trim())).Wait();
				}
				else if (line.StartsWith("b"))
				{
					Console.WriteLine(speaker.Control.GetVolume().Result);
				}
				else if (line.StartsWith("u"))
				{
					var result =
						speaker.Control.AddURITOQueue(
							"x-sonos-spotify:spotify%3atrack%3a456lFrF5OrYuCffSHSaYfs?sid=9&amp;flags=8224&amp;sn=1", "", 0, true).Result;
					Console.WriteLine("TRACK NR: " + result.FirstTrackNumberEnqueued);
					speaker.Control.Seek(SeekUnitType.TrackNumber, result.FirstTrackNumberEnqueued.ToString()).Wait(); 
					speaker.Control.Play().Wait(); ;
				}
				else if (line.StartsWith("q"))
				{
					break;
				}
			}
		}

		private static void Control_OnUpdate(object sender, KeyValuePair<string, string> e)
		{
			Console.WriteLine(e.Key + " : " + e.Value);
		}
	}
}
