using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NetStore.Views;

public partial class ProductManufacturerList : UserControl
{
    public ProductManufacturerList()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}