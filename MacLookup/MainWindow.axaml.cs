using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MacLookup;

public partial class MainWindow : Window
{
    SnmpClient? snmpClient;

    public List<Switch> Switches =
            [
                new Switch("157.26.120.12", [47, 48, 50], AuthProvider.Types.SHA512, "BD12"),
                new Switch("157.26.120.2", [49, 50], AuthProvider.Types.SHA1, "BD18"),
                new Switch("157.26.120.3", [50], AuthProvider.Types.SHA1, "BD21"),
                new Switch("157.26.120.4", [49, 50], AuthProvider.Types.SHA1, "BD24"),
                new Switch("157.26.120.5", [50], AuthProvider.Types.SHA1, "BD52"),
                new Switch("157.26.120.6", [50], AuthProvider.Types.SHA1, "BD53"),
                new Switch("157.26.120.1", [13, 14, 15, 16, 17, 18, 19, 20, 21, 22], AuthProvider.Types.SHA512, "BD59-01"),
                new Switch("157.26.120.11", [19, 20, 21, 22, 23, 24], AuthProvider.Types.SHA1, "BD59-02"),
                new Switch("157.26.120.7", [50], AuthProvider.Types.SHA1, "BD77-01"),
                new Switch("157.26.120.8", [26], AuthProvider.Types.SHA1, "BD77-02"),
                new Switch("157.26.120.9", [50], AuthProvider.Types.SHA512, "BD83"),
                new Switch("157.26.120.10", [50], AuthProvider.Types.SHA1, "BD86"),
                new Switch("157.26.120.47", [49, 50], AuthProvider.Types.SHA512, "BE27"),
            ];

    int CurrentSwitch = 6;
    bool preventLoop = false;
    int nullCounter = 0;

    public MainWindow()
    {
        InitializeComponent();
        LookupButton.Click += LookupMacAddress;
    }

    public async void LookupMacAddress(object? sender, RoutedEventArgs args)
    {
        ErrorField.Text = null;
        CompanyNameField.Text = null;

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
                }
                else
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
                    }
                    else if (CurrentSwitch == 1)
                    {
                        if (port == 49) CurrentSwitch = 0;
                        else CurrentSwitch = 6;
                        continue;
                    }
                    else if (CurrentSwitch == 3)
                    {
                        if (port == 49) CurrentSwitch = 12;
                        else CurrentSwitch = 6;
                        continue;
                    }
                    else if (CurrentSwitch == 12)
                    {
                        CurrentSwitch = 3;
                        continue;
                    }
                }
            } else if (port == null)
            {
                if (nullCounter >= 20)
                {
                    Result.Text = Switches[CurrentSwitch].Name + " - Port: Inconnu";
                    nullCounter = 0;
                    break;
                }

                nullCounter++;
                CurrentSwitch++;
                if (CurrentSwitch >= Switches.Count)
                {
                    CurrentSwitch = 0;
                    if (preventLoop)
                    {
                        nullCounter = 0;
                        Result.Text = "Aucune machine trouvée";
                        break;
                    }

                    preventLoop = true;
                }
                continue;
            }

            Result.Text = Switches[CurrentSwitch].Name + " - Port: " + port.ToString();
            GetMacAddressCompany(mac.Substring(0, mac.Length - 8));

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

    private async void GetMacAddressCompany(string mac)
    {
        string url = $"https://api.maclookup.app/v2/macs/{mac}";

        using HttpClient client = new HttpClient();

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<MacLookupResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            string macAddressCompany = data?.Company ?? "Unknown";

            CompanyNameField.Text = "Mac Address Company: " + macAddressCompany;
        }
        catch (HttpRequestException)
        {
            CompanyNameField.Text = "Error Using the API";
        }
    }
    public class MacLookupResponse
    {
        public string Company { get; set; }
    }

}
