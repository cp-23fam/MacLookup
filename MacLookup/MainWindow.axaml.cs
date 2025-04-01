using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MacLookup;

public partial class MainWindow : Window
{
    SnmpClient? snmpClient;

    public List<Switch> Switches =
            [
                new Switch("157.26.120.12", [50], AuthProvider.Types.SHA1, "BD12"),
                new Switch("157.26.120.2", [49, 50], AuthProvider.Types.SHA1, "BD18"),
                new Switch("157.26.120.3", [50], AuthProvider.Types.SHA1, "BD21"),
                new Switch("157.26.120.4", [49, 50], AuthProvider.Types.SHA1, "BD24"),
                new Switch("157.26.120.5", [50], AuthProvider.Types.SHA1, "BD52"),
                new Switch("157.26.120.6", [50], AuthProvider.Types.SHA1, "BD53"),
                new Switch("157.26.120.1", [13, 14, 15, 16, 17, 18, 19, 20, 21, 22], AuthProvider.Types.SHA512, "BD59-01"),
                new Switch("157.26.120.11", [24], AuthProvider.Types.SHA1, "BD59-02"),
                new Switch("157.26.120.7", [50], AuthProvider.Types.SHA1, "BD77-01"),
                new Switch("157.26.120.8", [26], AuthProvider.Types.SHA1, "BD77-02"),
                new Switch("157.26.120.9", [50], AuthProvider.Types.SHA1, "BD83"),
                new Switch("157.26.120.10", [50], AuthProvider.Types.SHA1, "BD86"),
                new Switch("157.26.120.47", [50], AuthProvider.Types.SHA1, "BE27"),
                //new Switch("157.26.120.13", [50], AuthProvider.Types.SHA1),
            ];

    int CurrentSwitch = 6;

    public MainWindow()
    {
        InitializeComponent();
        LookupButton.Click += LookupMacAddress;
    }

    public void LookupMacAddress(object? sender, RoutedEventArgs args)
    {
        CurrentSwitch = ClassSelectionField.SelectedIndex;
        ErrorField.Text = null;

        while (true)
        {
            snmpClient = new SnmpClient(Switches[CurrentSwitch].Ip, "guest", "Pa$$w0rd", Switches[CurrentSwitch].ConnexionType);

            string? mac = CheckMacAddress(MacAddressField.Text?.Trim());
            if (mac == null)
            {
                ErrorField.Text = "Please enter a valid MAC address";
                return;
            }

            int? port = snmpClient.getPortFromMac(mac);

            if (port != null && Switches[CurrentSwitch].ConnexionPorts.Contains((int)port))
            {
                if (CurrentSwitch > 1 && CurrentSwitch != 6 && CurrentSwitch != 12 && CurrentSwitch != 3)
                {
                    CurrentSwitch = 6;
                    continue;
                } else
                {
                    if (CurrentSwitch == 6)
                    {
                        switch (port)
                        {
                            case 13:
                                CurrentSwitch = 8;
                                break;
                            case 14:
                                CurrentSwitch = 10;
                                break;
                            case 15:
                                CurrentSwitch = 9;
                                break;
                            case 16:
                                CurrentSwitch = 4;
                                break;
                            case 17:
                                CurrentSwitch = 5;
                                break;
                            case 18:
                                CurrentSwitch = 11;
                                break;
                            case 19:
                                CurrentSwitch = 7;
                                break;
                            case 20:
                                CurrentSwitch = 1;
                                break;
                            case 21:
                                CurrentSwitch = 2;
                                break;
                            case 22:
                                CurrentSwitch = 3;
                                break;

                        }

                        continue;
                    }
                    else if (CurrentSwitch == 0)
                    {
                        CurrentSwitch++;
                        continue;
                    } else if ( CurrentSwitch == 1)
                    {
                        if (port == 49) CurrentSwitch = 0;
                        else CurrentSwitch = 6;
                        continue;
                    } else if (CurrentSwitch == 3)
                    {
                        if (port == 49) CurrentSwitch = 12;
                        else CurrentSwitch = 6;
                        continue;
                    } else if (CurrentSwitch == 12)
                    {
                        CurrentSwitch = 3;
                        continue;
                    }
                }
            }

            Result.Text = Switches[CurrentSwitch].Name + " - Port: " + port.ToString();
            break;
        }

    }

    private string? CheckMacAddress(string? mac)
    {
        if (mac == null) return null;

        string[] separator = [":", "-", " "];

        for (int i = 0; i < separator.Length; i++)
        {
            if (mac.Split(separator[i]).Length > 0)
            {
                if (mac.Split(separator[i]).Length == 6)
                {
                    return string.Join(":", mac.Split(separator[i]));
                }
            }
        }

        return null;
    }
}