using System;
using System.Text.RegularExpressions;
using NetStore.Models;
using ReactiveUI;

namespace NetStore.ViewModels;

public class RegistrationViewModel : ReactiveObject
{
    public string Name { get; set; } = String.Empty;
    public string Surname { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public string PasswordConfirm { get; set; } = String.Empty;
    public DateTimeOffset Birthdate { get; set; } = DateTimeOffset.Now;

    public bool IsVisibleMassage { get => _isVisibleMassage;
        set { _isVisibleMassage = value; this.RaisePropertyChanged(); }
    }

    private bool _isVisibleMassage = false;
    
    public string Massage 
    { 
        get => _massage;
        set
        {
            _massage = value;
            IsVisibleMassage = !string.IsNullOrEmpty(_massage); 
            this.RaisePropertyChanged();
        } 
    }

    private string _massage = String.Empty;
    
    public void Registration()
    {
        if (!IsValidEmail())
        {
            Massage = "Email is invalid!";
        }
        else if (!IsValidName())
        {
            Massage = "Name is invalid!";
        }
        else if (!IsValidSurname())
        {
            Massage = "Surname is invalid!";
        }
        else if (!IsValidPassword())
        {
            Massage = "Password is invalid!";
        }
        else if (!IsMatchPasswords())
        {
            Massage = "Passwords mismatches!";
        }
        else if (!IsValidBirthdate())
        {
            Massage = "Passwords mismatches!";
        }
        else
        {
            User newUser = new User()
            {
                Name = this.Name, 
                Surname = this.Surname, 
                Password = this.Password, 
                Email = this.Email,
                Birthdate = this.Birthdate, 
                RoleId = UserRoleEnum.Client
            };

            if (Database.UserDatabase.SetUser(newUser))
            {
                Close();
            }
            else
            {
                Massage = "Connection error";
            }
        }
    }

    public void Close()
    {
        Config.SubWindow();
    }

    private bool IsValidEmail()
    {
        bool? isEmailUse = Database.UserDatabase.IsEmailUse(Email);
        string regexString = @"^(?![_\-.])[\w\-.]{1,64}(?<![_\-.])@([\w\-]+\.)+[\w\-]{2,}$";
        Regex regex = new Regex(regexString);
        return isEmailUse != null && isEmailUse == false && regex.IsMatch(Email);
    }

    private bool IsValidName()
    {
            
        return !string.IsNullOrWhiteSpace(Name);
    }
    
    private bool IsValidSurname()
    {
        return !string.IsNullOrWhiteSpace(Surname);
    }

    private bool IsValidPassword()
    {
        string regexString = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$";
        Regex regex = new Regex(regexString);
        return regex.IsMatch(Password);
    }

    private bool IsMatchPasswords()
    {
        return Password.Equals(PasswordConfirm);
    }

    private bool IsValidBirthdate()
    {
        return Birthdate.Date < DateTime.Now;
    }
}