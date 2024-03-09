using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetStore.ViewModels;

namespace NetStore.Views.Product;

public partial class WorkWithProductItem : UserControl
{
    public WorkWithProductItem()
    {
        InitializeComponent();
    }
    
    public WorkWithProductItem(Models.Product product) : this()
    {
        (this.DataContext as WorkWithProductItemViewModel).CurrentProduct = product;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}