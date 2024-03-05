using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetStore.Models;

namespace NetStore.Views;

public partial class WorkWithUserItem : UserControl
{
    public WorkWithUserItem()
    {
        InitializeComponent();
    }

    public WorkWithUserItem(User user) : this()
    {
        
    }
}