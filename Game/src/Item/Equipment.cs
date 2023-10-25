using GUI.DebugInspector.Display;

namespace Item;

// represents an item that can be worn or held
public class Equipment : IItem
{
    public double BaseDamage { get; init; } = 0;
    public double BaseDefense { get; init; } = 0;

    public double Damage { get { return BaseDamage * UpgradeLevel; } }
    public double Defense { get { return BaseDamage * UpgradeLevel; } }

    //Level 1 is the lowest level
    private int UpgradeLevel { get; set; } = 1;

    // determines which 'slot' an equipments occupies
    // for example a pawn can only equipt 1 head-piece at a tume
    public EquipmentType EquipmentType { get; set; }

    public string Name { get; set; } = "equipment";

    public IDisplay Display => ConstructDisplay();

    public Equipment(EquipmentType equipmentType, string name)
    {
        Name = name;
        EquipmentType = equipmentType;
    }

    //Upgrades the equipment
    public void Upgrade()
    {
        UpgradeLevel += 1;
    }

    public void QueueFree() { }

    IDisplay ConstructDisplay()
    {
        // TODO: Item containers should have proper ID generation.... one day
        Display root = new(Name);
        root.AddDetail("BaseDefense: " + BaseDefense);
        root.AddDetail("BaseDamage: " + BaseDamage);
        root.AddDetail("UpgradeLevel: " + UpgradeLevel);
        root.AddDetail("EquipmentType: " + EquipmentType);
        return root;
    }
}

public enum EquipmentType
{
    HEAD,
    CHEST,
    LEGS,
    FEET,
    HANDS,
    HELD
}
