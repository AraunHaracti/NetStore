using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;
using NetStore.Views;
using ReactiveUI;

namespace NetStore.ViewModels;

public class ProductManufacturerListViewModel : ReactiveObject
{
    public string SearchString { get; set; } = String.Empty;

    public ProductManufacturerListViewModel()
    {
        GetDataFromDb(); // TODO: Авто обновление при добавлении элемента
    }

    public void Search()
    {
        GetDataFromDb();
    }
    public void AddItem()
    {
        Config.AddWindow(new WorkWithProductManufacturerItem());
    }
    public void EditItem()
    {
        if (SelectedProductManufacturer == null)
            return;
        
        Config.AddWindow(new WorkWithProductManufacturerItem(SelectedProductManufacturer));
    }

    private List<ProductManufacturer> _listProductManufacturers = new List<ProductManufacturer>();

    public List<ProductManufacturer> ListProductManufacturers
    {
        get => _listProductManufacturers;
        set
        {
            _listProductManufacturers = value;
            this.RaisePropertyChanged();
        }
    }

    private MySqlCommand _sqlCommand;

    private void GetDataFromDb()
    {
        _sqlCommand = Database.ProductManufacturerDatabase.GetQuerySelect(searchQuery: SearchString);
        TotalPage = (int) Math.Ceiling(Database.ProductManufacturerDatabase.GetTotalProductManufacturers(_sqlCommand.Clone()) / (decimal)_pageSize);
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

    public ProductManufacturer SelectedProductManufacturer { get; set; }
    
    public event EventHandler<ProductManufacturer> ElementSelected;

    public void SelectElement()
    {
        ElementSelected?.Invoke(this, SelectedProductManufacturer);
        CloseView();
    }

    public void CloseView()
    {
        Config.SubWindow();
    }
    
    public void PaginateList(PaginationCommand paginationCommand)
    {
        switch (paginationCommand)
        {
            case PaginationCommand.First: 
                CurrentPage = 1;
                ListProductManufacturers = Database.ProductManufacturerDatabase.GetProductManufacturers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Last:
                CurrentPage = TotalPage;
                ListProductManufacturers = Database.ProductManufacturerDatabase.GetProductManufacturers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Next: 
                CurrentPage += 1;
                if (CurrentPage > TotalPage)
                {
                    CurrentPage = TotalPage;
                }
                ListProductManufacturers = Database.ProductManufacturerDatabase.GetProductManufacturers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Previous: 
                CurrentPage -= 1;
                if (CurrentPage < 1)
                {
                    CurrentPage = 1;
                }
                ListProductManufacturers = Database.ProductManufacturerDatabase.GetProductManufacturers(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
        }

        if (TotalPage == 0)
            CurrentPage = 0;
    }
}