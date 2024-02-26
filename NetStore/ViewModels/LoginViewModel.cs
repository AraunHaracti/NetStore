using System;
using NetStore.Models;
using NetStore.Views;
using ReactiveUI;

namespace NetStore.ViewModels;

public class LoginViewModel : ReactiveObject
{
    public string Email { get; set; }
    public string Password { get; set; }

    public bool IsVisibleMessage { get => _isVisibleMessage;
        set { _isVisibleMessage = value; this.RaisePropertyChanged(); }
    }

    private bool _isVisibleMessage = false;


    private bool _isActionsEnabled = true;

    public bool IsActionsEnabled
    {
        get => _isActionsEnabled;
        set
        {
            _isActionsEnabled = value;
            this.RaisePropertyChanged();
        }
    }
    
    public LoginViewModel()
    {
        if (!Database.ConnectionDatabase.CheckConnection())
        {
            IsActionsEnabled = false;
            Message = "Connction error";
        }
        
    }
    
    public string Message 
    { 
        get => _message;
        set
        {
            _message = value;
            IsVisibleMessage = !string.IsNullOrEmpty(_message); 
            this.RaisePropertyChanged();
        } 
    }

    private string _message = String.Empty;

    public void Guest()
    {
        Config.CurrentUser = new User()
        {
            Birthdate = DateTime.Now, 
            Name = "Гость", 
            Surname = "Гостевой", 
            RoleId = UserRoleEnum.Guest 
        };
        
        Config.AddWindow(new ProductList());
    }

    public void Registration()
    {
        Config.AddWindow(new RegistrationForm());
    }

    public void Login()
    {
        if (!(string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)))
        {
            User? user = Database.UserDatabase.GetUser(Email, Password);
            if (user != null)
            {
                Config.CurrentUser = user;
                Config.AddWindow(new ProductList());
            }
            else
            {
                Message = "Email or password is incorrect";
            }
        }
    }
}