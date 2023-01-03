using System.Collections.Generic;
using System.Threading;
using Serilog;
using Pawn.Controller;

//TODO: this could be generalized to a 'UseItemClass' where the animation depends on the item
namespace Pawn.Action {
	public class LootAction : IAction {
		private PawnController? ownerPawnController;
		public int CooldownMilliseconds {get {return 0;} }

		public string Name {get {return "LootAction";}}
		public float MaxRange {get {return 2;}}
		public List<ActionTags> Tags {get {return new List<ActionTags>();}}
		public Godot.Spatial? HeldItemMesh {get; set;}

		private int waitTimeMilliseconds;

		private ItemContainer itemContainer;

		public LootAction(){}
		public LootAction(PawnController _ownerPawnController, ItemContainer _itemContainer){
			ownerPawnController = _ownerPawnController;
			itemContainer = _itemContainer;
			HeldItemMesh = null;
		}

		//TODO: duplicate should really only have to be used for the combat actions
		//TODO: HOLY SHIT this has to be refactored. I need an action builder
		public IAction Duplicate(PawnController _ownerPawnController, PawnController _otherPawnController) {
			Log.Warning("Duplicate was called on LootAction, unintended events may follow");
			int THOUSAND_MILLISECONDS = 1000;
			return new WaitAction(_ownerPawnController, THOUSAND_MILLISECONDS);
		}

		public void execute() {
			//TODO: TakeDamage should be called 'change health'
			Weapon nextWeapon = itemContainer.ContainedWeapon;

			if( (ownerPawnController.Weapon == null) || (nextWeapon.Damage > ownerPawnController.Weapon.Damage) ){
				//changing things in a multithreaded environment!
				//awesome
				Log.Information("Setting Weapon");
				ownerPawnController.SetWeapon(nextWeapon);
			}
			ownerPawnController.VisualController.SetAnimation(AnimationName.Interact, false);
			Thread.Sleep( (int) ownerPawnController.VisualController.getAnimationLengthMilliseconds(AnimationName.Interact));
			itemContainer.Delete();
		}
	}
}