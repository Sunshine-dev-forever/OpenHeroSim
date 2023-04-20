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
	public partial class BattleRoyaleRunner : Node
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

		public override void _Process(double delta){
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
			NavigationRegion3D navigation = GetNode<NavigationRegion3D>("/root/Node3D/NavigationRegion3D");
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
			Node3D NegX = GetNode<Node3D>("NegX");
			Node3D NegZ = GetNode<Node3D>("NegZ");
			Node3D PosX = GetNode<Node3D>("PosX");
			Node3D PosZ = GetNode<Node3D>("PosZ");
			float newDist = (float) FogController.GetFogController().GetFogPosition();
			SetOrigin(NegX, new Vector3(-newDist, 0,0));
			SetOrigin(NegZ, new Vector3(0,0,-newDist));
			SetOrigin(PosX, new Vector3(newDist,0,0));
			SetOrigin(PosZ, new Vector3(0,0,newDist));
		}

		private void SetOrigin(Node3D spatial, Vector3 origin ) {
			spatial.GlobalTransform = new Transform3D(spatial.GlobalTransform.Basis, origin);
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
			location.Y = 0.5f;
			Node3D TreasureChestMesh = CustomResourceLoader.LoadMesh(ResourcePaths.TREASURE_CHEST);
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
			itemContainer.GlobalTransform = new Transform3D(itemContainer.GlobalTransform.Basis, location);
			kdTreeController.AddInteractable(itemContainer);
		}

		private Equipment CreateIronSword() {
		Node3D ironSword = CustomResourceLoader.LoadMesh(ResourcePaths.IRON_SWORD);
		Equipment equipment = new Equipment(ironSword, EquipmentType.HELD);
		equipment.Damage = 7;
		return equipment;
	}

	private Equipment CreateRustedDagger() {
		Node3D rustedDagger = CustomResourceLoader.LoadMesh(ResourcePaths.RUSTED_DAGGER);
		Equipment equipment = new Equipment(rustedDagger, EquipmentType.HELD);
		equipment.Damage = 3;
		return equipment;
	}

	private Equipment CreateLightSaber() {
		Node3D lightSaber = CustomResourceLoader.LoadMesh(ResourcePaths.LIGHTSABER);
		Equipment equipment = new Equipment(lightSaber, EquipmentType.HELD);
		equipment.Damage = 15;
		return equipment;
	}

	private Consumable CreateHealingPotion() {
		Node3D healthPotion = CustomResourceLoader.LoadMesh(ResourcePaths.HEALTH_POTION);
		return new Consumable(40, healthPotion);
	}
	}
}
