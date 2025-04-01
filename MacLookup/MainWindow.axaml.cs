using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MacLookup;

public partial class MainWindow : Window
{
    SnmpClient snmpClient;

    public List<Switch> Switches =
            [
                new Switch("157.26.120.1", [13, 14, 15, 16, 17, 18, 19, 20, 21, 22], AuthProvider.Types.SHA512),
                new Switch("157.26.120.2", [49, 50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.3", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.4", [49, 50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.5", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.6", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.7", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.8", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.9", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.10", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.11", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.12", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.13", [50], AuthProvider.Types.SHA1),
                new Switch("157.26.120.47", [50], AuthProvider.Types.SHA1),
            ];

    public MainWindow()
    {
        InitializeComponent();

        LookupButton.Click += LookupMacAddress;
        snmpClient = new SnmpClient("157.26.120.2", "guest", "Pa$$w0rd", AuthProvider.Types.SHA1);
    }

    public void LookupMacAddress(object? sender, RoutedEventArgs args)
    {
        int? port = snmpClient.getPortFromMac(MacAddressField.Text);

        Result.Text = port.ToString();
    }


}