using NetStore.Views;

namespace NetStore.ViewModels;

public class MainMenuAppViewModel
{
    public void OpenProducts()
    {
        Config.AddWindow(new ProductList());
    }
    
    public void OpenProductManufacturers()
    {
        Config.AddWindow(new ProductManufacturerList());
    }
    
    public void OpenProductCategories()
    {
        Config.AddWindow(new ProductCategoryList());
    }
}