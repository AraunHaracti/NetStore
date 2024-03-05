using NetStore.Models;
using ReactiveUI;

namespace NetStore.ViewModels;

public class WorkWithProductManufacturerItemViewModel : ReactiveObject
{
    private ProductManufacturer _currentProductManufacturer = new ProductManufacturer();

    public ProductManufacturer CurrentProductManufacturer
    {
        get => _currentProductManufacturer;
        set
        {
            _currentProductManufacturer = value;
            this.RaisePropertyChanged();
        }
    }
    
    public void SaveItem()
    {
        if (CurrentProductManufacturer.ManufacturerId != 0)
        {
            bool isUpdate = Database.ProductManufacturerDatabase.ChangeProductManufacturer(CurrentProductManufacturer);
            if (!isUpdate)
                return;
        }
        else
        {
            bool isInsert = Database.ProductManufacturerDatabase.SetProductManufacturer(CurrentProductManufacturer);
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