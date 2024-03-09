using System.Collections.Generic;
using System.Linq;
using NetStore.Models;
using ReactiveUI;

namespace NetStore.ViewModels;

public class WorkWithProductItemViewModel : ReactiveObject
{
    private Product _currentProduct = new Product();

    public Product CurrentProduct
    {
        get => _currentProduct;
        set
        {
            _currentProduct = value;
            this.RaisePropertyChanged();
        }
    }

    public string Name
    {
        get => CurrentProduct.Name;
        set
        {
            CurrentProduct.Name = value;
            this.RaisePropertyChanged();
        }
    }

    public string Description
    {
        get => CurrentProduct.Description;
        set
        {
            CurrentProduct.Description = value;
            this.RaisePropertyChanged();
        }
    }

    public decimal Price
    {
        get => CurrentProduct.Price;
        set
        {
            CurrentProduct.Price = value;
            this.RaisePropertyChanged();
        }
    }

    public int Value
    {
        get => CurrentProduct.Quantity;
        set
        {
            CurrentProduct.Quantity = value;
            this.RaisePropertyChanged();
        }
    }

    public ProductCategory SelectedCategory
    {
        get
        {
            return ListCategories.Where(it => it.CategoryId == CurrentProduct.CategoryId).ToList()[0];
        }
        set
        {
            CurrentProduct.CategoryId = value.CategoryId;
            this.RaisePropertyChanged();
        }
    }

    public ProductCategory SelectedManufacturer
    {
        get
        {
            return ListCategories.Where(it => it.CategoryId == CurrentProduct.CategoryId).ToList()[0];
        }
        set
        {
            CurrentProduct.CategoryId = value.CategoryId;
            this.RaisePropertyChanged();
        }
    }

    public List<ProductCategory> ListCategories { get; set; } = new List<ProductCategory>();

    public List<ProductManufacturer> ListManufacturers { get; set; } = new List<ProductManufacturer>();

    public WorkWithProductItemViewModel()
    {
        // ListCategories;
        // ListManufacturers;
    }
    
    public void SaveItem()
    {
        if (CurrentProduct.ProductId != 0)
        {
            bool isUpdate = Database.ProductDatabase.ChangeProduct(CurrentProduct);
            if (!isUpdate)
                return;
        }
        else
        {
            bool isInsert = Database.ProductDatabase.SetProduct(CurrentProduct);
            if (!isInsert)
                return;
        }
        
        CloseView();
    }

    public void CloseView()
    {
        Config.SubWindow();
    }
}