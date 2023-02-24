using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Controller;
using Pawn.Goal;
using Pawn.Item;
using Pawn.Action.Ability;
using Util;

namespace Worlds.BattleRoyale {
	public class BattleRoyaleRunner : Node
	{
		private static int NUMBER_OF_PAWNS_TO_SPAWN = 100;
		private static int NUMBER_OF_CHESTS_TO_SPAWN = 50;
		private List<PawnController> pawns = new List<PawnController>();

		private KdTreeController kdTreeController = null!; 
		public override void _Ready()
		{
			kdTreeController = new KdTreeController();
			this.AddChild(kdTreeController);
		}
		
		public override void _Input(InputEvent input) {
			if(input.IsActionPressed("mouse_left_click")) {
				//Do nothing... for now
			} else if(input.IsActionPressed("ui_left")) {
				//spawns pawns in random locations
				for(int x = 0; x < NUMBER_OF_PAWNS_TO_SPAWN; x++) {
					pawns.Add(CreatePawn(GetRandomLocationInArena()));
				}
			} else if (input.IsActionPressed("ui_right")) {
				for(int x = 0; x < NUMBER_OF_CHESTS_TO_SPAWN; x++) {
					CreateItemChest(GetRandomLocationInArena());
				}
			} else if(input.IsActionPressed("ui_up")) {
				Log.Information("starting fog!");
				FogController.GetFogController().StartFog();
			} else if(input.IsActionPressed("ui_down")){
				Log.Information("distnace to set: " + FogController.GetFogController().GetFogPosition());
			}
		}

		public override void _Process(float  delta){
			//iterate through all pawns, deal damage those that are outside the bounds
			for(int i = pawns.Count - 1; i >= 0; i--) {
				PawnController pawn = pawns[i];
				if(!IsInstanceValid(pawn)) {
					pawns.RemoveAt(i);
					break;
				}
				//TODO: do damage to pawn if they are outside bounds
			}
			UpdateBarriers();
		}


		private Vector3 GetRandomLocationInArena() {
			Random rand = new Random();
			int x = rand.Next(-249, 249);
			int z = rand.Next(-249, 249);
			int HEIGHT_DEFAULT = 2;
			return new Vector3(x, HEIGHT_DEFAULT, z);
		}

		private PawnController CreatePawn(Vector3 location){
			Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
			return PawnControllerBuilder.Start(this, kdTreeController, navigation)
								.AddGoal(new HealGoal())
								.AddGoal(new DefendSelfGoal())
								.AddGoal(new LootGoal())
								.AddGoal(new BattleRoyaleWanderGoal())
								.AddAbility(new StabAbility())
								.Location(location)
								.Finish();		
		}

		private void UpdateBarriers() {
			Spatial NegX = GetNode<Spatial>("NegX");
			Spatial NegZ = GetNode<Spatial>("NegZ");
			Spatial PosX = GetNode<Spatial>("PosX");
			Spatial PosZ = GetNode<Spatial>("PosZ");
			float newDist = (float) FogController.GetFogController().GetFogPosition();
			SetOrigin(NegX, new Vector3(-newDist, 0,0));
			SetOrigin(NegZ, new Vector3(0,0,-newDist));
			SetOrigin(PosX, new Vector3(newDist,0,0));
			SetOrigin(PosZ, new Vector3(0,0,newDist));
		}

		private void SetOrigin(Spatial spatial, Vector3 origin ) {
			spatial.GlobalTransform = new Transform(spatial.GlobalTransform.basis, origin);
		}

		private Equipment? GetRandomWeapon() {
			Random rand = new Random();
			int rng = rand.Next(0, 100);

			if(rng > 40) {
				return null;
			}
			if(rng > 15) {
				return CreateRustedDagger();
			}
			if( rng > 4) {
				return CreateIronSword();
			}
			return CreateLightSaber();
		}

		private void CreateItemChest(Vector3 location) {
			//gonna override the height here
			//TODO: not sure if this is mutable
			location.y = 0.5f;
			Spatial TreasureChestMesh = CustomResourceLoader.LoadMesh(ResourcePaths.TREASURE_CHEST);
			//The iron sword gets leaked when created like this
			List<IItem> items = new List<IItem>();
			items.Add(CreateHealingPotion());
			Equipment? equipment = GetRandomWeapon();
			if(equipment != null) {
				items.Add(equipment);
			}
			//items.Add(CreateIronSword());
			ItemContainer itemContainer = new ItemContainer(items, TreasureChestMesh);
			this.AddChild(itemContainer);
			itemContainer.GlobalTransform = new Transform(itemContainer.GlobalTransform.basis, location);
			kdTreeController.AddInteractable(itemContainer);
		}

		private Equipment CreateIronSword() {
		Spatial ironSword = CustomResourceLoader.LoadMesh(ResourcePaths.IRON_SWORD);
		Equipment equipment = new Equipment(ironSword, EquipmentType.HELD);
		equipment.Damage = 7;
		return equipment;
	}

	private Equipment CreateRustedDagger() {
		Spatial rustedDagger = CustomResourceLoader.LoadMesh(ResourcePaths.RUSTED_DAGGER);
		Equipment equipment = new Equipment(rustedDagger, EquipmentType.HELD);
		equipment.Damage = 3;
		return equipment;
	}

	private Equipment CreateLightSaber() {
		Spatial lightSaber = CustomResourceLoader.LoadMesh(ResourcePaths.LIGHTSABER);
		Equipment equipment = new Equipment(lightSaber, EquipmentType.HELD);
		equipment.Damage = 15;
		return equipment;
	}

	private Consumable CreateHealingPotion() {
		Spatial healthPotion = CustomResourceLoader.LoadMesh(ResourcePaths.HEALTH_POTION);
		return new Consumable(40, healthPotion);
	}
	}
}
