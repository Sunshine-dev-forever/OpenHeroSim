using System.Collections.Generic;
using System.Threading;
using Serilog;
using Pawn.Controller;
using Pawn.Item;

//TODO: this could be generalized to a 'UseItemClass' where the animation depends on the item
namespace Pawn.Action {
	public class DrinkPotionAction : IAction {
		private PawnController? ownerPawnController;
		public int CooldownMilliseconds {get {return 0;} }

		public string Name {get {return "DrinkPotionAction";}}
		public float MaxRange {get {return 2;}}
		public List<ActionTags> Tags {get {return new List<ActionTags>();}}
		public Godot.Spatial? HeldItemMesh {get; set;}

		private int waitTimeMilliseconds;

		private Consumable potionToDrink;

		public DrinkPotionAction(){}
		public DrinkPotionAction(PawnController _ownerPawnController, Consumable _potionToDrink){
			ownerPawnController = _ownerPawnController;
			potionToDrink = _potionToDrink;
			HeldItemMesh = _potionToDrink.Mesh;
		}

		//TODO: duplicate should really only have to be used for the combat actions
		public IAction Duplicate(PawnController _ownerPawnController, PawnController _otherPawnController) {
			Log.Warning("Duplicate was called on DrinkPotionAction, unintended events may follow");
			int THOUSAND_MILLISECONDS = 1000;
			return new WaitAction(_ownerPawnController, THOUSAND_MILLISECONDS);
		}

		//@param waitTimeMilliseconds - amount of time to wait
		public void execute() {
			ownerPawnController.ItemList.Remove(potionToDrink);
			//TODO: TakeDamage should be called 'change health'
			ownerPawnController.TakeDamage(potionToDrink.Healing * (-1));
			ownerPawnController.VisualController.SetAnimation(AnimationName.Drink, false);
			Thread.Sleep( (int) ownerPawnController.VisualController.getAnimationLengthMilliseconds(AnimationName.Drink));
		}
	}
}