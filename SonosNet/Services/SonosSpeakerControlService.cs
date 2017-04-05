using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SonosNet.Contants;
using UPnPNet;
using UPnPNet.Models;
using UPnPNet.Services.AvTransport;
using UPnPNet.Services.RenderingControl;

namespace SonosNet.Services
{
	public class SonosSpeakerControlService
	{
		internal readonly UPnPDevice SpeakerDevice;
		private const int InstanceId = 0;
		private readonly AvTransportServiceControl _avService;
		private readonly RenderingControlServiceControl _renderingControlService;

		public SonosSpeakerControlService(UPnPDevice speakerDevice)
		{
			SpeakerDevice = speakerDevice;

			UPnPService avService = this.GetMediaRendererService(UPnPSonosServiceTypes.AvService);
			UPnPService renderingControl = this.GetMediaRendererService(UPnPSonosServiceTypes.RenderingControlService);

			_avService = new AvTransportServiceControl(avService);
			_renderingControlService = new RenderingControlServiceControl(renderingControl);
		}

		public void SubscribeToEvents(UPnPServer server)
		{
			_avService.OnLastChangeEvent += AvServiceOnLastChangeEvent;
			_renderingControlService.OnLastChangeEvent += RenderingControlOnLastChangeEvent;

			server.SubscribeToControl(_avService);
			server.SubscribeToControl(_renderingControlService);
		}

		private void RenderingControlOnLastChangeEvent(object sender, RenderingControlEvent e)
		{
			OnUpdate?.Invoke(this, new KeyValuePair<string, string>("volume", e.Volumes[RenderingControlChannel.Master].ToString()));
		}

		private void AvServiceOnLastChangeEvent(object sender, AvTransportEvent e)
		{
			OnUpdate?.Invoke(this, new KeyValuePair<string, string>("play-state", e.TransportState.Value));
		}

		public async Task Play() => await _avService.Play(InstanceId, 1);
		public async Task Pause() => await _avService.Pause(InstanceId);
		public async Task Stop() => await _avService.Stop(InstanceId);
		public async Task Next() => await _avService.Next(InstanceId);
		public async Task Previous() => await _avService.Previous(InstanceId);
		public async Task SetVolume(int value)
		{
			if (value < 0 || value > 100)
			{
				return;
			}

			await _renderingControlService.SetVolume(InstanceId, RenderingControlChannel.Master, value);
		}

		public async Task Seek(SeekUnitType unit, string target)
		{
			string unitType = "";

			switch (unit)
			{
				case SeekUnitType.RelativeTime:
					unitType = "REL_TIME";
					break;
				case SeekUnitType.Section:
					unitType = "SECTION";
					break;
				case SeekUnitType.TimeDelta:
					unitType = "TIME_DELTA";
					break;
				case SeekUnitType.TrackNumber:
					unitType = "TRACK_NR";
					break;
			}

			await _avService.SendAction("Seek", new Dictionary<string, string>
			{
				{"InstanceID", InstanceId.ToString()},
				{"Unit", unitType},
				{"Target", target},
			});
		}

		public async Task<AddUriQueueResponse> AddURITOQueue(string uri, string uriMetadata, int desiredTrackNumber = 0, bool enqueueAsNext = false)
		{
			var result = await _avService.SendAction("AddURIToQueue", new Dictionary<string, string>
			{
				{"InstanceID", InstanceId.ToString()},
				{"EnqueuedURI", uri},
				{"EnqueuedURIMetaData", uriMetadata},
				{"DesiredFirstTrackNumberEnqueued", desiredTrackNumber.ToString()},
				{"EnqueueAsNext", enqueueAsNext ? "1" : "0"}
			});

			return new AddUriQueueResponse
			{
				FirstTrackNumberEnqueued = int.Parse(result["FirstTrackNumberEnqueued"]),
				NumTracksAdded = int.Parse(result["NumTracksAdded"]),
				NewQueueLength = int.Parse(result["NewQueueLength"])
			};
		}

		public async Task<int> GetVolume()
		{
			return await _renderingControlService.GetVolume(InstanceId, RenderingControlChannel.Master);
		}

		public event EventHandler<KeyValuePair<string, string>> OnUpdate;
	}

	public static class SonosSpeakerControlExtensions
	{
		public static UPnPService GetMediaRendererService(this SonosSpeakerControlService service, string serviceName)
		{
			return service.SpeakerDevice
				.SubDevices
				.FirstOrDefault(x => x.Services.Any(y => y.Type == UPnPSonosServiceTypes.AvService))
				.Services.FirstOrDefault(x => x.Type == serviceName);
		}
		public static UPnPService GetMediaServerService(this SonosSpeakerControlService service, string serviceName)
		{
			return service.SpeakerDevice
				.SubDevices
				.FirstOrDefault(x => x.Services.Any(y => y.Type == UPnPSonosServiceTypes.ContentDirectory))
				.Services.FirstOrDefault(x => x.Type == serviceName);
		}
	}

	public class AddUriQueueResponse
	{
		public int FirstTrackNumberEnqueued { get; set; }
		public int NumTracksAdded { get; set; }
		public int NewQueueLength { get; set; }
	}

	public enum SeekUnitType
	{
		TrackNumber,
		RelativeTime,
		TimeDelta,
		Section
	}
}
