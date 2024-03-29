﻿using System;
using NetStore.Models;
using NetStore.Views;
using ReactiveUI;

namespace NetStore.ViewModels;

public class LoginViewModel : ReactiveObject
{
    private string _email = String.Empty;
    private string _password = String.Empty;
    private bool _isVisibleMessage = false;
    private bool _isActionsEnabled = true;
    private string _message = String.Empty;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            this.RaisePropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            this.RaisePropertyChanged();
        }
    }

    public bool IsVisibleMessage { get => _isVisibleMessage;
        set
        {
            _isVisibleMessage = value; 
            this.RaisePropertyChanged();
        }
    }
    
    public bool IsActionsEnabled
    {
        get => _isActionsEnabled;
        set
        {
            _isActionsEnabled = value;
            this.RaisePropertyChanged();
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
    
    public LoginViewModel()
    {
        if (!Database.ConnectionDatabase.CheckConnection())
        {
            IsActionsEnabled = false;
            Message = "Connction error";
        }
        
    }
    
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
        
        FirstState();
    }

    public void Registration()
    {
        Config.AddWindow(new RegistrationForm());
        
        FirstState();
    }

    public void Login()
    {
        User? user = Database.UserDatabase.GetUser(Email, Password);
        if (user != null)
        {
            Config.CurrentUser = user;
            Config.AddWindow(new MainMenuApp());

            FirstState();
        }
        else
        {
            Message = "Почта или пароль неверны!";
        }
    }

    private void FirstState()
    {
        Message = String.Empty;
        Email = String.Empty;
        Password = String.Empty;
    }
}