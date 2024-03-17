using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NetStore.Models;

namespace NetStore.Views;

public partial class ProductList : UserControl
{
    public ProductList()
    {
        InitializeComponent();
        
        var element = this.FindControl<StackPanel>("PermissionControl");
        element.IsVisible = Config.CurrentUser.RoleId == UserRoleEnum.Merchandiser;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void FilterControl_Btn_OnClick(object? sender, RoutedEventArgs e)
    {
        var element = this.FindControl<ContentControl>("FilterControl");
        element.IsVisible = !element.IsVisible;
    }
}