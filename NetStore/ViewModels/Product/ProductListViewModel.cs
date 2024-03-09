using System;
using System.Collections.Generic;
using MySqlConnector;
using NetStore.Models;
using NetStore.Views;
using NetStore.Views.Product;
using ReactiveUI;

namespace NetStore.ViewModels;

public class ProductListViewModel : ReactiveObject
{
    private decimal? _priceFrom = null;
    private decimal? _priceTo = null;
    private int? _quantityFrom = null;
    private int? _quantityTo = null;

    public decimal? PriceFrom
    {
        get => _priceFrom;
        set
        {
            _priceFrom = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal? PriceTo
    {
        get => _priceTo;
        set
        {
            _priceTo = value;
            this.RaisePropertyChanged();
        }
    }

    public int? QuantityFrom
    {
        get => _quantityFrom;
        set
        {
            _quantityFrom = value;
            this.RaisePropertyChanged();
        }
    }

    public int? QuantityTo
    {
        get => _quantityTo;
        set
        {
            _quantityTo = value;
            this.RaisePropertyChanged();
        }
    }

    public void Filter()
    {
        GetProductsFromDb();
    }

    public ProductListViewModel()
    {
        GetProductsFromDb();
    }

    private List<Product> _listProduct = new List<Product>();

    public List<Product> ListProduct
    {
        get => _listProduct;
        set
        {
            _listProduct = value;
            this.RaisePropertyChanged();
        }
    }

    private MySqlCommand _sqlCommand;

    private void GetProductsFromDb()
    {
        _sqlCommand = Database.ProductDatabase.GetQuerySelect(searchQuery: SearchString, priceFrom: PriceFrom, priceTo: PriceTo, quantityFrom: QuantityFrom, quantityTo: QuantityTo, category: SelectedProductCategoryItem);
        TotalPage = (int) Math.Ceiling(Database.ProductDatabase.GetTotalProducts(_sqlCommand.Clone()) / (decimal)_pageSize);
        PaginateList(PaginationCommand.First);
    }
    
    
    public string SearchString { get; set; } = String.Empty;

    public void Search()
    {
        PriceFrom = null;
        PriceTo = null;
        QuantityFrom = null;
        QuantityTo = null;
        GetProductsFromDb();
    }
    
    public List<ProductManufacturer> ManufacturersList { get; set; }
    public List<ProductCategory> CategoriesList { get; set; }

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
                ListProduct = Database.ProductDatabase.GetProducts(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Last:
                CurrentPage = TotalPage;
                ListProduct = Database.ProductDatabase.GetProducts(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Next: 
                CurrentPage += 1;
                if (CurrentPage > TotalPage)
                {
                    CurrentPage = TotalPage;
                }
                ListProduct = Database.ProductDatabase.GetProducts(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
            case PaginationCommand.Previous: 
                CurrentPage -= 1;
                if (CurrentPage < 1)
                {
                    CurrentPage = 1;
                }
                ListProduct = Database.ProductDatabase.GetProducts(CurrentPage, _pageSize, _sqlCommand.Clone());
                break;
        }
    }

    public void CloseView()
    {
        Config.SubWindow();
    }

    public Product SelectedProductItem { get; set; }
    
    public void AddItem()
    {
        Config.AddWindow(new WorkWithProductItem());
    }
    
    public void EditItem()
    {
        Config.AddWindow(new WorkWithProductItem(SelectedProductItem));
    }

    private ProductCategory _selectedProductCategoryItem = null;

    public ProductCategory SelectedProductCategoryItem
    {
        get => _selectedProductCategoryItem;
        set
        {
            _selectedProductCategoryItem = value;
            this.RaisePropertyChanged();
        }
    }

    public void SelectProductCategory()
    {
        var window = new ProductCategoryList();
        (window.DataContext as ProductCategoryListViewModel).ElementSelected +=
            (sender, category) =>
            {
                SelectedProductCategoryItem = category;
                GetProductsFromDb();
            };
        Config.AddWindow(window);
    }
    
}