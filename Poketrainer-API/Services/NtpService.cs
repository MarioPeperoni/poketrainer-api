using Yort.Ntp;

namespace Poketrainer_API.Services;

public interface INtpService
{
    Task<DateTime> GetNetworkTimeAsync();
}

public class NtpService : INtpService
{
    private const string NtpServer = "time.google.com";

    public async Task<DateTime> GetNetworkTimeAsync()
    {
        try
        {
            var client = new NtpClient(NtpServer);
            var time = await client.RequestTimeAsync();
            return time.NtpTime;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get NTP time. {e.Message}", e);
        }
    }
}