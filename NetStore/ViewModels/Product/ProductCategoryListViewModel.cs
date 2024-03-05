using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;
using NetStore.Views;
using ReactiveUI;

namespace NetStore.ViewModels;

public class ProductCategoryListViewModel : ReactiveObject
{
    public string SearchString { get; set; } = String.Empty;

    public ProductCategoryListViewModel()
    {
        GetDataFromDb();
    }

    public void Search()
    {
        GetDataFromDb();
    }
    public void AddItem()
    {
        Config.AddWindow(new WorkWithProductCategoryItem());
    }
    public void EditItem()
    {
        if (SelectedProductCategory == null)
            return;
        
        Config.AddWindow(new WorkWithProductCategoryItem(SelectedProductCategory));
    }

    private List<ProductCategory> _listProductCategories = new List<ProductCategory>();

    public List<ProductCategory> ListProductCategories
    {
        get => _listProductCategories;
        set
        {
            _listProductCategories = value;
            this.RaisePropertyChanged();
        }
    }

    private MySqlCommand _sqlCommand;

    private void GetDataFromDb()
    {
        _sqlCommand = Database.ProductCategoryDatabase.GetQuerySelect(searchQuery: SearchString);
        TotalPage = (int) Math.Ceiling(Database.ProductCategoryDatabase.GetTotalProductCategories(_sqlCommand.Clone()) / (decimal)_pageSize);
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

    public ProductCategory SelectedProductCategory { get; set; }
    
    public void PaginateList(PaginationCommand paginationCommand)
    {
        switch (paginationCommand)
        {
            case PaginationCommand.First: 
                CurrentPage = 1;
                ListProductCategories = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Last:
                CurrentPage = TotalPage;
                ListProductCategories = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Next: 
                CurrentPage += 1;
                if (CurrentPage > TotalPage)
                {
                    CurrentPage = TotalPage;
                }
                ListProductCategories = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Previous: 
                CurrentPage -= 1;
                if (CurrentPage < 1)
                {
                    CurrentPage = 1;
                }
                ListProductCategories = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
        }

        if (TotalPage == 0)
            CurrentPage = 0;
    }
}