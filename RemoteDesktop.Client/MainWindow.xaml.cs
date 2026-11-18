using System.IO;
using System.Linq;
using System.Windows;
using RemoteDesktop.Shared;

namespace RemoteDesktop.Client;

public partial class MainWindow : Window
{
    private readonly string _pinPath;

    public MainWindow()
    {
        InitializeComponent();
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SysConnector");
        Directory.CreateDirectory(appData);
        var idPath = Path.Combine(appData, "user-id.txt");
        var id = File.Exists(idPath) ? File.ReadAllText(idPath).Trim() : DeviceIdentity.CreateId();
        if (!IsSixDigitUserId(id)) id = DeviceIdentity.CreateId();
        if (!File.Exists(idPath) || File.ReadAllText(idPath).Trim() != id) File.WriteAllText(idPath, id);
        UserIdBox.Text = id;
        _pinPath = Path.Combine(appData, "access-pin.txt");
        var savedPin = File.Exists(_pinPath) ? File.ReadAllText(_pinPath).Trim() : string.Empty;
        PinBox.Text = IsFourDigitPin(savedPin) ? savedPin : DeviceIdentity.CreatePin();
        if (!IsFourDigitPin(savedPin)) File.WriteAllText(_pinPath, PinBox.Text);
    }

    private void CopyId_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(UserIdBox.Text);
        ConnectionMessage.Text = "User ID copied.";
    }

    private void RefreshPin_Click(object sender, RoutedEventArgs e)
    {
        PinBox.Text = DeviceIdentity.CreatePin();
        File.WriteAllText(_pinPath, PinBox.Text);
        PinMessage.Text = "A new four digit PIN has been saved.";
    }

    private void SavePin_Click(object sender, RoutedEventArgs e)
    {
        var pin = PinBox.Text.Trim();
        if (!IsFourDigitPin(pin))
        {
            PinMessage.Text = "PIN must contain exactly four digits.";
            return;
        }

        File.WriteAllText(_pinPath, pin);
        PinBox.Text = pin;
        PinMessage.Text = "Your PIN has been saved.";
    }

    private void Connect_Click(object sender, RoutedEventArgs e)
    {
        var remoteId = RemoteIdBox.Text.Trim();
        ConnectionMessage.Text = !IsSixDigitUserId(remoteId)
            ? "Enter a valid six digit user ID."
            : "Authentication is still not implemented in this MVP.";
    }

    private static bool IsSixDigitUserId(string userId) => userId.Length == 6 && userId.All(char.IsAsciiDigit);

    private static bool IsFourDigitPin(string pin) => pin.Length == 4 && pin.All(char.IsAsciiDigit);
}

