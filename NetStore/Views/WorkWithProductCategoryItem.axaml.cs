using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetStore.Models;
using NetStore.ViewModels;

namespace NetStore.Views;

public partial class WorkWithProductCategoryItem : UserControl
{
    public WorkWithProductCategoryItem()
    {
        InitializeComponent();
    }

    public WorkWithProductCategoryItem(ProductCategory productCategory) : this()
    {
        (this.DataContext as WorkWithProductCategoryItemViewModel).CurrentProductCategory = productCategory;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}