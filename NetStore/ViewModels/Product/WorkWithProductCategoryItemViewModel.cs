using NetStore.Database;
using NetStore.Models;
using ReactiveUI;

namespace NetStore.ViewModels;

public class WorkWithProductCategoryItemViewModel : ReactiveObject
{
    private ProductCategory _currentProductCategory = new ProductCategory();

    public ProductCategory CurrentProductCategory
    {
        get => _currentProductCategory;
        set
        {
            _currentProductCategory = value;
            this.RaisePropertyChanged();
        }
    }
    
    public void SaveItem()
    {
        if (CurrentProductCategory.CategoryId != 0)
        {
            bool isUpdate = Database.ProductCategoryDatabase.ChangeProductCategory(CurrentProductCategory);
            if (!isUpdate)
                return;
        }
        else
        {
            bool isInsert = Database.ProductCategoryDatabase.SetProductCategory(CurrentProductCategory);
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