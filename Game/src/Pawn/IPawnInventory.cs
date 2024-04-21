using Item;
using System.Collections.Generic;

namespace Pawn;

public interface IPawnInventory
{
    // Returns all items in the pawn's inventory, including equipment
    List<IItem> GetAllItems();
    List<IItem> GetAllItemsInBag();
    List<IItem> GetAllEquippedItems();
    int Money {get;}
    void removeMoney(int amount);
    bool AddItem(IItem item);
    bool RemoveItem(IItem item);
    List<IItem> EmptyAllItems();
    Equipment? GetWornEquipment(EquipmentType type);
    void WearEquipment(Equipment equipment);
    double GetTotalEquiptmentDefense();
    double GetTotalEquiptmentDamage();
    void QueueFree();
}
