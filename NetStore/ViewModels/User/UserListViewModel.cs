using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;
using NetStore.Views;
using ReactiveUI;

namespace NetStore.ViewModels;

public class UserListViewModel : ReactiveObject
{
    public string SearchString { get; set; } = String.Empty;

    public UserListViewModel()
    {
        GetDataFromDb();
    }

    public void Search()
    {
        GetDataFromDb();
    }
    public void AddItem()
    {
        Config.AddWindow(new WorkWithUserItem());
    }
    public void EditItem()
    {
        if (SelectedUser == null)
            return;
        
        Config.AddWindow(new WorkWithUserItem(SelectedUser));
    }

    private List<User> _listUsers = new List<User>();

    public List<User> ListUsers
    {
        get => _listUsers;
        set
        {
            _listUsers = value;
            this.RaisePropertyChanged();
        }
    }

    private MySqlCommand _sqlCommand;

    private void GetDataFromDb()
    {
        _sqlCommand = Database.UserDatabase.GetQuerySelect(searchQuery: SearchString);
        TotalPage = (int) Math.Ceiling(Database.UserDatabase.GetTotalUsers(_sqlCommand.Clone()) / (decimal)_pageSize);
        PaginateList(PaginationCommand.First);
    }
    
    
    private int _currentPage = 1;

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            this.RaisePropertyChanged();
        }
    }

    private int _totalPage = 1;
    public int TotalPage 
    { 
        get => _totalPage;
        set
        {
            _totalPage = value;
            this.RaisePropertyChanged();
        } 
    }
    private readonly int _pageSize = 4;

    public User SelectedUser { get; set; }
    
    public void PaginateList(PaginationCommand paginationCommand)
    {
        switch (paginationCommand)
        {
            case PaginationCommand.First: 
                CurrentPage = 1;
                ListUsers = Database.UserDatabase.GetUsers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Last:
                CurrentPage = TotalPage;
                ListUsers = Database.UserDatabase.GetUsers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Next: 
                CurrentPage += 1;
                if (CurrentPage > TotalPage)
                {
                    CurrentPage = TotalPage;
                }
                ListUsers = Database.UserDatabase.GetUsers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Previous: 
                CurrentPage -= 1;
                if (CurrentPage < 1)
                {
                    CurrentPage = 1;
                }
                ListUsers = Database.UserDatabase.GetUsers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
        }

        if (TotalPage == 0)
            CurrentPage = 0;
    }
}