using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetStore.Models;
using NetStore.ViewModels;

namespace NetStore.Views;

public partial class WorkWithProductManufacturerItem : UserControl
{
    public WorkWithProductManufacturerItem()
    {
        InitializeComponent();
    }

    public WorkWithProductManufacturerItem(ProductManufacturer productManufacturer) : this()
    {
        (this.DataContext as WorkWithProductManufacturerItemViewModel).CurrentProductManufacturer = productManufacturer;
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}