using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NetStore.Views;

public partial class ProductList : UserControl
{
    public ProductList()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}