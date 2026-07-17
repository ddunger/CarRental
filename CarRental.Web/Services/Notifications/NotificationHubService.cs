using CarRental.Web.Dtos.Notifications;
using CarRental.Web.Services.Auth;
using Microsoft.AspNetCore.SignalR.Client;

namespace CarRental.Web.Services.Notifications
{
    public sealed class NotificationHubService(TokenStorageService tokenStorage) : IAsyncDisposable
    {
        private const string HubUrl = "https://localhost:8081/notifications";

        private HubConnection? _connection;
        private bool _reconnecting;

        public event Action<RentalOverdueNotification>? RentalOverdueReceived;
        public event Action<string>? ConnectionLost;

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public async Task StartAsync()
        {
            // Sentinem pattern: always rebuild — never reuse a stale connection
            if (_connection is not null)
                await StopAsync();

            var accessToken = await tokenStorage.GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(accessToken))
                return;

            _connection = new HubConnectionBuilder()
                .WithUrl($"{HubUrl}?access_token={Uri.EscapeDataString(accessToken)}")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<RentalOverdueNotification>("RentalOverdue", notification =>
            {
                RentalOverdueReceived?.Invoke(notification);
            });

            _connection.Closed += HandleConnectionClosedAsync;

            await _connection.StartAsync();
        }

        public async Task StopAsync()
        {
            var connection = _connection;
            if (connection is null)
                return;

            _connection = null;

            connection.Closed -= HandleConnectionClosedAsync;

            try { await connection.StopAsync(); }
            catch { /* best effort */ }

            await connection.DisposeAsync();
        }

        private async Task HandleConnectionClosedAsync(Exception? exception)
        {
            if (_reconnecting)
                return;
            _reconnecting = true;

            try
            {
                // No refresh-token flow on the client yet: retry with the stored token.
                // When TryRefreshAsync exists, this is where it plugs in (Sentinem pattern).
                var token = await tokenStorage.GetAccessTokenAsync();
                if (string.IsNullOrWhiteSpace(token))
                {
                    ConnectionLost?.Invoke("Connection to the server lost.");
                    return;
                }

                await StartAsync();
            }
            catch
            {
                ConnectionLost?.Invoke("Connection to the server lost.");
            }
            finally
            {
                _reconnecting = false;
            }
        }

        public async ValueTask DisposeAsync() => await StopAsync();
    }
}