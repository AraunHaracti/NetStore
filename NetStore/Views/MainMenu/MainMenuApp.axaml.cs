using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NetStore.Views;

public partial class MainMenuApp : UserControl
{
    public MainMenuApp()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}