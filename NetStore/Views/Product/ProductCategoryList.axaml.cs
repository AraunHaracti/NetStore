using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NetStore.Views;

public partial class ProductCategoryList : UserControl
{
    public ProductCategoryList()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}