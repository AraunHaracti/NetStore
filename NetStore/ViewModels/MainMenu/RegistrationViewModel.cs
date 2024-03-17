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
    private bool _isVisibleMassage = false;
    private string _massage = String.Empty;

    public bool IsVisibleMassage { get => _isVisibleMassage;
        set { _isVisibleMassage = value; this.RaisePropertyChanged(); }
    }

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
    
    public void Registration()
    {
        if (!IsValidEmail())
        {
            Massage = "Почта неверна или уже используется!";
        }
        else if (!IsValidName())
        {
            Massage = "Должно быть имя!";
        }
        else if (!IsValidSurname())
        {
            Massage = "Должна быть фамилия!";
        }
        else if (!IsValidPassword())
        {
            Massage = "Слабый пароль!";
        }
        else if (!IsMatchPasswords())
        {
            Massage = "Пароли не совпадают!";
        }
        else if (!IsValidBirthdate())
        {
            Massage = "Неправильная дата!";
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