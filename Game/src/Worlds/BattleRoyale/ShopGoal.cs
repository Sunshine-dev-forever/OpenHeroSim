using Interactable;
using Item;
using Pawn.Action;
using Pawn.Components;
using Pawn.Targeting;
using Pawn.Tasks;
using Serilog;

namespace Pawn.Goal;

public class ShopGoal : IPawnGoal {
    public ITask GetTask(IPawnController pawnController, SensesStruct sensesStruct) {
        if (sensesStruct.nearbyShops.Count == 0) {
            return new InvalidTask();
        }
        
        Shop shop = sensesStruct.nearbyShops[0];
        //atm this will always only contain a health 
        Consumable consumable = (Consumable)shop.Items[0];
        //can we afford this?
        if ((int)(consumable.Value * 0.25) > pawnController.PawnInventory.Money) {
            //we cannot afford this
            return new InvalidTask();
        }

        void executable() {
            while ((int)(consumable.Value * 0.25) <= pawnController.PawnInventory.Money) {
                pawnController.PawnInventory.RemoveMoney((int)(consumable.Value * 0.25));
                pawnController.PawnInventory.AddItem(consumable.Copy());
                Log.Information("a potion was bought in the shop");
            }
        }

        IAction action = ActionBuilder
            .Start(pawnController, executable)
            .Animation(AnimationName.Interact)
            .Finish();

        ITargeting targeting = new InteractableTargeting(shop);
        return new Task(targeting, action, "buying a potion from a shop");
    }
}
