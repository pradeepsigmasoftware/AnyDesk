using System.Security.Cryptography;

namespace RemoteDesktop.Shared;

public static class DeviceIdentity
{
    public static string CreateId()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }

    public static string CreatePin()
    {
        return RandomNumberGenerator.GetInt32(0, 10_000).ToString("D4");
    }
}
