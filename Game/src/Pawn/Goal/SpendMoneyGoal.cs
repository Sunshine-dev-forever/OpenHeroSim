using Godot;
using Interactable;
using Item;
using Pawn.Action;
using Pawn.Components;
using Pawn.Targeting;
using Pawn.Tasks;
using Serilog;
using System.Collections.Generic;

namespace Pawn.Goal;

public class SpendMoneyGoal : IPawnGoal
{
    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct)
    {
        Building? building = GetNearestBuilding(sensesStruct.nearbyBuildings);

        if (building == null || !HasMoney(pawnController))
        {
            return new InvalidTask();
        }

        void executable()
        {
            List<IItem> items = pawnController.PawnInventory.GetAllItemsInBag();
            foreach (IItem item in items)
            {
                if (item is IStackable && item.Name.Equals(StackItem.MONEY))
                {
                    IStackable money = (IStackable)item;
                    if (money.Count == 0)
                    {
                        //probably should not be error checking here, but for now ill leave it
                        Log.Error("Pawn has a stackable with 0 count!");
                        return;
                    }

                    pawnController.PawnInventory.RemoveItem(item);
                    GD.Print("donated :" + money.Count + " monies!");
                    return;
                }
            }
        }

        IAction action = ActionBuilder
            .Start(pawnController, executable)
            .Animation(AnimationName.Interact)
            .Finish();

        ITargeting targeting = new InteractableTargeting(building);
        return new Task(targeting, action, "Donating money to the blacksmith");
    }


    Building? GetNearestBuilding(List<Building> buildings)
    {
        if (buildings.Count > 0)
        {
            return buildings[0];
        }
        return null;
    }

    bool HasMoney(IPawnController pawnController)
    {
        List<IItem> items = pawnController.PawnInventory.GetAllItemsInBag();
        foreach (IItem item in items)
        {
            if (item is IStackable && item.Name.Equals(StackItem.MONEY))
            {
                IStackable money = (IStackable)item;
                if (money.Count == 0)
                {
                    //probably should not be error checking here, but for now ill leave it
                    Log.Error("Pawn has a stackable with 0 count!");
                    return false;
                }
                return true;
            }
        }
        return false;
    }


}
