using Item;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Pawn;

public class PawnInventory : IPawnInventory {
    // for now all pawns can only carry 5 items
    static readonly int INVENTORY_SPACE = 5;

    // Held items should be seperate, but that is a later issue
    Dictionary<EquipmentType, Equipment> wornGear;
    List<IItem> bag;

    public PawnInventory() {
        bag = new List<IItem>();
        wornGear = new Dictionary<EquipmentType, Equipment>();
    }

    public List<IItem> GetAllEquippedItems() => new(wornGear.Values);

    public List<IItem> GetAllItemsInBag() => new(bag);

    public int Money => GetAllItemsInBag().Where(item => item is StackItem stackItem && stackItem.IsMoney).Select(item => item.Value).Sum();

    public void RemoveMoney(int amount) {
        StackItem money = (StackItem)GetAllItemsInBag().Where(item => item is StackItem stackItem && stackItem.IsMoney).First();
        if (money.Value < amount) {
            Log.Error("Tried to remove more money than pawn has");
            money.Count = 0;
        }
        else {
            money.Count -= amount;
        }
    }

    public List<IItem> GetAllItems() {
        List<IItem> rtn = new();
        rtn.AddRange(bag);
        rtn.AddRange(wornGear.Values);
        return rtn;
    }

    // returns true if the item was successfully added
    public bool AddItem(IItem item) {
        if (item is IStackable stackable && AddStackableToStack(stackable)) {
            return true;
        }
        // if out of inventory space then fail
        if (bag.Count >= INVENTORY_SPACE) {
            return false;
        }
        else {
            bag.Add(item);
            return true;
        }
    }

    private bool AddStackableToStack(IStackable stackable) {
        List<IStackable> bagStackablesWithSameName = bag.FindAll((bagItem) => bagItem is IStackable && bagItem.Name.Equals(stackable.Name))
            .ConvertAll((bagItem) => (IStackable)bagItem);

        if (bagStackablesWithSameName.Count > 1) {
            //we have multiple stackables with the same name... they should have already been stacked together
            Log.Error("mulitple stackables with the same name");
            return false;
        }
        else if (bagStackablesWithSameName.Count == 1) {
            IStackable bagStackable = bagStackablesWithSameName.First();
            bagStackable.Count += ((IStackable)stackable).Count;
            return true;
        }
        
        return false;
    }

    // Tries to remove items from the bag first
    // Returns true if the item was successfully removed, otherwise false
    public bool RemoveItem(IItem item) {
        // Try to remove items from bag first
        if (bag.Contains(item)) {
            return bag.Remove(item);
        }
        else if (item is Equipment equipment) // Then we will remove equipped gear
        {
            if (wornGear.ContainsValue(equipment)) {
                return wornGear.Remove(equipment.EquipmentType);
            }
        }
        // failed to remove anything
        return false;
    }

    // Returns all items in the pawn's inventory, including equiptment
    // Removes the reference of all items from the pawns inventory
    public List<IItem> EmptyAllItems() {
        List<IItem> rtn = GetAllItems();
        bag = new List<IItem>();
        wornGear = new Dictionary<EquipmentType, Equipment>();
        return rtn;
    }

    // returns null if no equiptment of that type is worn
    public Equipment? GetWornEquipment(EquipmentType type) => wornGear.ContainsKey(type) ? wornGear[type] : null;

    public void WearEquipment(Equipment equipment) {
        // if we already were wearing something
        if (wornGear.ContainsKey(equipment.EquipmentType)) {
            Equipment oldEquipment = wornGear[equipment.EquipmentType];
            // store it for later
            bag.Add(oldEquipment);
        }

        wornGear[equipment.EquipmentType] = equipment;
    }

    // TODO: somehow combine GetTotalEquiptmentDefense and GetTotalEquiptmentDamage and GetTotal<whateverElse> into one function
    public double GetTotalEquiptmentDefense() {
        double total = 0;

        foreach (Equipment equipment in wornGear.Values) {
            total += equipment.Defense;
        }

        return total;
    }

    public double GetTotalEquiptmentDamage() {
        double total = 0;

        foreach (Equipment equipment in wornGear.Values) {
            total += equipment.Damage;
        }

        return total;
    }

    public void QueueFree() {
        foreach (Equipment equipment in wornGear.Values) {
            equipment.QueueFree();
        }

        foreach (IItem item in bag) {
            item.QueueFree();
        }
    }
}
