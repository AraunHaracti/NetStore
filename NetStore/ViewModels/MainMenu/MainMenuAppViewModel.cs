﻿using NetStore.Views;

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

    public void OpenUsers()
    {
        Config.AddWindow(new UserList());
    }

    public void CloseView()
    {
        Config.SubWindow();
    }
}