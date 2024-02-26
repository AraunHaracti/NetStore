using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;
using ReactiveUI;

namespace NetStore.ViewModels;

public class ProductCategoryListViewModel : ReactiveObject
{
    private string _searchString = String.Empty;
    public string SearchString
    {
        get => _searchString;
        set
        {
            _searchString = value;
            this.RaisePropertyChanged();
        }
    }

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
        throw new NotImplementedException();
    }
    public void EditItem()
    {
        throw new NotImplementedException();
    }

    private List<ProductCategory> _listProduct = new List<ProductCategory>();

    public List<ProductCategory> ListProduct
    {
        get => _listProduct;
        set
        {
            _listProduct = value;
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

    public void PaginateList(PaginationCommand paginationCommand)
    {
        switch (paginationCommand)
        {
            case PaginationCommand.First: 
                CurrentPage = 1;
                ListProduct = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Last:
                CurrentPage = TotalPage;
                ListProduct = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Next: 
                CurrentPage += 1;
                if (CurrentPage > TotalPage)
                {
                    CurrentPage = TotalPage;
                }
                ListProduct = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Previous: 
                CurrentPage -= 1;
                if (CurrentPage < 1)
                {
                    CurrentPage = 1;
                }
                ListProduct = Database.ProductCategoryDatabase.GetProductCategories(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
        }
    }
}