using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SonosNet.Models;
using System.Linq;
using SonosNet.Contants;
using UPnPNet.Gena;
using UPnPNet.Models;
using UPnPNet.Services.AvTransport;
using UPnPNet.Services.RenderingControl;

namespace SonosNet.Services
{
	public class SonosSpeakerControlService : ISonosSpeakerControl
	{
		private const int InstanceId = 0;
		private readonly AvTransportServiceControl _avService;
		private readonly RenderingControlServiceControl _renderingControlService;

		public SonosSpeakerControlService(UPnPDevice speakerDevice)
		{
			UPnPService avService = speakerDevice.Services.FirstOrDefault(x => x.Type == UPnPSonosServiceType.AvService);
			UPnPService renderingControl = speakerDevice.Services.FirstOrDefault(x => x.Type == UPnPSonosServiceType.RenderingControlService);

			_avService = new AvTransportServiceControl(avService);
			_renderingControlService = new RenderingControlServiceControl(renderingControl);
		}

		public void SubscribeToEvents(string notifyUrl, GenaSubscriptionHandler handler, int timeout = 3600)
		{
			_avService.OnLastChangeEvent += AvServiceOnLastChangeEvent;
			_renderingControlService.OnLastChangeEvent += RenderingControlOnLastChangeEvent;

			_avService.SubscribeToEvents(handler, notifyUrl, timeout).Wait();
			_renderingControlService.SubscribeToEvents(handler, notifyUrl, timeout).Wait();
		}

		private void RenderingControlOnLastChangeEvent(object sender, RenderingControlEvent e)
		{
			OnUpdate?.Invoke(this, new KeyValuePair<string, string>("volume", e.Volumes[RenderingControlChannel.Master].ToString()));
		}

		private void AvServiceOnLastChangeEvent(object sender, AvTransportEvent e)
		{
			OnUpdate?.Invoke(this, new KeyValuePair<string, string>("play-state", e.TransportState.Value));
		}

		public async Task Play()
		{
			await _avService.Play(InstanceId, 1);
		}

		public async Task Pause()
		{
			await _avService.Pause(InstanceId);
		}

		public async Task Stop()
		{
			await _avService.Stop(InstanceId);
		}

		public async Task Next()
		{
			await _avService.Next(InstanceId);
		}

		public async Task Previous()
		{
			await _avService.Previous(InstanceId);
		}

		public async Task SetVolume(int value)
		{
			if (value < 0 || value > 100)
			{
				return;
			}

			await _renderingControlService.SetVolume(InstanceId, RenderingControlChannel.Master, value);
		}

		public async Task<int> GetVolume()
		{
			return await _renderingControlService.GetVolume(InstanceId, RenderingControlChannel.Master);
		}

		public event EventHandler<KeyValuePair<string, string>> OnUpdate;
	}
}
