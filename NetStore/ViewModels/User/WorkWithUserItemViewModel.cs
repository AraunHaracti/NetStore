using System.Collections.Generic;
using NetStore.Models;
using ReactiveUI;

namespace NetStore.ViewModels;

public class WorkWithUserItemViewModel : ReactiveObject
{
    private User _currentUser = new User();

    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            this.RaisePropertyChanged();
        }
    }

    public List<UserRole> UserRoleList { get; } = new List<UserRole>() { new UserRole() { } };
    private UserRole _role;

    public UserRole Role
    {
        get => _role;
        set
        {
            _role = value;
            this.RaisePropertyChanged();
        }
    }

    public void SaveItem()
    {
        
        
        CloseView();
    }

    public void CloseView()
    {
        Config.SubWindow();
    }
}