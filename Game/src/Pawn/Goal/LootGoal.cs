using Interactable;
using Item;
using Pawn.Action;
using Pawn.Components;
using Pawn.Targeting;
using Pawn.Tasks;
using System.Collections.Generic;

namespace Pawn.Goal;

public class LootGoal : IPawnGoal
{
    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct)
    {
        ItemContainer? nearbyLoot = GetFirstNonemptyContainer(sensesStruct.nearbyContainers);

        if (nearbyLoot == null)
        {
            return new InvalidTask();
        }

        void executable()
        {
            for (int i = nearbyLoot.Items.Count - 1; i >= 0; i--)
            {
                IItem item = nearbyLoot.Items[i];
                processItem(item, pawnController, nearbyLoot);
            }
        }

        IAction action = ActionBuilder
            .Start(pawnController, executable)
            .Animation(AnimationName.Interact)
            .Finish();

        ITargeting targeting = new InteractableTargeting(nearbyLoot);
        return new Task(targeting, action, "Looting a container");
    }

    void processItem(IItem item, IPawnController pawnController, ItemContainer container)
    {
        container.Items.Remove(item);

        if (item is Equipment newWeapon)
        {
            Equipment? currentWeapon = pawnController.PawnInventory
                .GetWornEquipment(EquipmentType.HELD);

            if (currentWeapon == null || (newWeapon.Damage > currentWeapon.Damage))
            {
                // if we want the weapon, we take the weapon
                pawnController.PawnInventory.WearEquipment(newWeapon);
            }
        }
        else if (item is Consumable)
        {
            bool wasAddSuccessfull = pawnController.PawnInventory.AddItem(item);

            if (!wasAddSuccessfull)
            {
                // I guess the bag is full
                // we delete the item
                item.QueueFree();
            }
        }
        else if (item is StackItem)
        {
            //man this function is not going to scale well HAHA!
            bool wasAddSuccessfull = pawnController.PawnInventory.AddItem(item);
            if (!wasAddSuccessfull)
            {
                // I guess the bag is full
                // we delete the item
                item.QueueFree();
            }
        }
        else
        {
            // Some unknown item, destroy it so no one else can have it
            item.QueueFree();
        }
    }

    ItemContainer? GetFirstNonemptyContainer(List<ItemContainer> nearbyContainers)
    {
        foreach (ItemContainer itemContainer in nearbyContainers)
        {
            if (itemContainer.Items.Count >= 1)
            {
                return itemContainer;
            }
        }

        return null;
    }
}
