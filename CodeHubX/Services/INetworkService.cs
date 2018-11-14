using CodeHubX.Helpers;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CodeHubX.Services
{
	public interface INetworkService
		: System.IDisposable
	{
		ReadOnlyObservableCollection<ulong> Bandwidths { get; }

		ConnectionType ConnectionType { get; }

		IReadOnlyCollection<ConnectionType> ConnectionTypes { get; }

		IConnectivity Connectivity { get; }

		bool IsConnected { get; }

		event ConnectivityChangedEventHandler ConnectivityChanged;

		event ConnectivityTypeChangedEventHandler ConnectionTypeChanged;

		bool IsReachable(string host, int timeoutInMilliSeconds = 5000);

		Task<bool> IsReachableAsync(string host, int timeoutInMilliSeconds = 5000);

		bool IsRemoteReachable(string host, int port = 80, int timeoutInMilliSeconds = 5000);

		Task<bool> IsRemoteReachableAsync(string host, int port = 80, int timeoutInMilliSeconds = 5000);
	}

	public sealed class NetworkService
	    : INetworkService
	{
		private IEnumerable<ulong> _Bandwidths;
		private IEnumerable<ConnectionType> _ConnectionTypes;
		private IConnectivity _Connectivity;
		private bool _IsConnected;

		public ReadOnlyObservableCollection<ulong> Bandwidths
		{
			get => _Bandwidths.ToReadOnlyObservableCollection();
			private set => _Bandwidths = value;
		}

		public ConnectionType ConnectionType
			=> _ConnectionTypes.First();

		public IReadOnlyCollection<ConnectionType> ConnectionTypes
		{
			get => _ConnectionTypes?.ToReadOnlyCollection() ?? null;
			private set => _ConnectionTypes = value;
		}

		public IConnectivity Connectivity
		{
			get => _Connectivity;
			private set => _Connectivity = value;
		}

		public bool IsConnected
		{
			get => _IsConnected;
			private set => _IsConnected = value;
		}

		public event ConnectivityChangedEventHandler ConnectivityChanged;

		public event ConnectivityTypeChangedEventHandler ConnectionTypeChanged;

		private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
			IsConnected = e
				.IsConnected;
			ConnectivityChanged?.Invoke(sender, e);
		}

		private void OnConnectivityTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
		{
			_ConnectionTypes = e.ConnectionTypes;
			ConnectionTypeChanged?.Invoke(sender, e);
		}

		public NetworkService()
		{
			_Connectivity = CrossConnectivity.Current;
			IsConnected = _Connectivity?.IsConnected ?? false;
			if (_Connectivity != null)
			{
				_ConnectionTypes = _Connectivity.ConnectionTypes;
				_Connectivity.ConnectivityChanged += OnConnectivityChanged;
				_Connectivity.ConnectivityTypeChanged += OnConnectivityTypeChanged;
			}
		}

		public bool IsReachable(string host, int timeoutInMilliseconds = 2000)
			=> _Connectivity.IsReachable(
				host,
				timeoutInMilliseconds)
			  .GetAwaiter()
			  .GetResult();

		public Task<bool> IsReachableAsync(string host, int timeoutInMilliseconds = 2000)
			=> _Connectivity.IsReachable(
				host,
				timeoutInMilliseconds);

		public bool IsRemoteReachable(string host, int port = 80, int timeoutInMilliseconds = 2000)
			=> _Connectivity.IsRemoteReachable(
				host,
				port,
				timeoutInMilliseconds)
			   .GetAwaiter()
			   .GetResult();

		public async Task<bool> IsRemoteReachableAsync(string host, int port = 80, int timeoutInMilliseconds = 2000)
			=> await _Connectivity.IsRemoteReachable(
				host,
				port,
				timeoutInMilliseconds);

		#region Disposable Support
		private bool disposedValue = false; // To detect redundant calls

		private void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					if (_Connectivity != null)
					{
						_Connectivity.ConnectivityChanged -= OnConnectivityChanged;
						_Connectivity.ConnectivityTypeChanged -= OnConnectivityTypeChanged;
						_Connectivity.Dispose();
						_Connectivity = null;
						_Bandwidths = null;
						_ConnectionTypes = null;
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~NetworkService() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
			=>
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);// TODO: uncomment the following line if the finalizer is overridden above. //GC.SuppressFinalize(this);
		#endregion
	}
}
