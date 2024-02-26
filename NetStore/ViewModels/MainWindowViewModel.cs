using System;
using Avalonia.Controls;
using ReactiveUI;

namespace NetStore.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        Config.WindowsListChanged += HandlerUpdatePresenter;
    }

    private UserControl _presenter = null;
    public UserControl Presenter
    {
        get => _presenter;
        set
        {
            _presenter = value;
            this.RaisePropertyChanged();
        }
    }

    private void HandlerUpdatePresenter(object sender, EventArgs e)
    {
        Presenter = (UserControl) Config.GetLastWindow();
    }
}