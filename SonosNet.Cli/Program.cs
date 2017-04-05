using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SonosNet.Models;
using SonosNet.Services;
using SonosNet.SMAPI;
using UPnPNet.Server;

namespace SonosNet.Cli
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Run().Wait();
		}

		private static async Task Run()
		{
			var service = new SMAPIService();
			var result1 = await service.Search("https://spotify-v4.ws.sonos.com/smapi", "artist", "Foo Figters");


			return;
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

			while (true)
			{
				string line = Console.ReadLine();

				if (line.StartsWith("a"))
				{
					await speaker.Control.Play();
				}
				else if (line.StartsWith("s"))
				{
					await speaker.Control.Pause();
				}
				else if (line.StartsWith("v"))
				{
					await speaker.Control.SetVolume(int.Parse(line.Remove(0, 1).Trim()));
				}
				else if (line.StartsWith("b"))
				{
					Console.WriteLine(speaker.Control.GetVolume().Result);
				}
				else if (line.StartsWith("u"))
				{
					var result = await speaker.Control.AddURITOQueue("x-sonos-spotify:spotify%3atrack%3a456lFrF5OrYuCffSHSaYfs?sid=9&amp;flags=8224&amp;sn=1", "", 0, true);
					Console.WriteLine("TRACK NR: " + result.FirstTrackNumberEnqueued);
					await speaker.Control.Seek(SeekUnitType.TrackNumber, result.FirstTrackNumberEnqueued.ToString());
					await speaker.Control.Play();
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
