using Avalonia.Controls;

namespace MacLookup;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        new SnmpClient("157.26.120.1", "guest", "Pa$$w0rd", AuthProvider.Types.SHA512);
    }
}