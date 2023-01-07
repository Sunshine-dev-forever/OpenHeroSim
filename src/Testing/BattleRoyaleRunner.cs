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

public class BattleRoyaleRunner : Node
{
	private static int NUMBER_OF_PAWNS_TO_SPAWN = 100;
	private static int NUMBER_OF_CHESTS_TO_SPAWN = 50;

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
				CreatePawn(GetRandomLocationInArena());
			}
		 } else if (input.IsActionPressed("ui_right")) {
			for(int x = 0; x < NUMBER_OF_CHESTS_TO_SPAWN; x++) {
				CreateItemChest(GetRandomLocationInArena());
			}
		 }
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
							.AddGoal(new WanderGoal())
							.WearEquiptment(GetRandomWeapon())
							.Location(location)
							.Finish();		
	}

	private Equipment GetRandomWeapon() {
		Random rand = new Random();
		int rng = rand.Next(0,3);
		switch (rng) {
			case 0: return CreateLightSaber();
			case 1: return CreateRustedDagger();
			case 2: return CreateIronSword();
			default:
			return CreateRustedDagger();
		}
	}

	private void CreateItemChest(Vector3 location) {
		//gonna override the height here
		//TODO: not sure if this is mutable
		location.y = 0.5f;
		Spatial TreasureChestMesh = (Spatial) GD.Load<PackedScene>("res://scenes/world_objects/treasure_chest.tscn").Instance();
		//The iron sword gets leaked when created like this
		List<IItem> items = new List<IItem>();
		items.Add(CreateHealingPotion());
		//items.Add(CreateIronSword());
		ItemContainer itemContainer = new ItemContainer(items, TreasureChestMesh);
		this.AddChild(itemContainer);
		itemContainer.GlobalTransform = new Transform(itemContainer.GlobalTransform.basis, location);
		kdTreeController.AddInteractable(itemContainer);
	}

	private Equipment CreateIronSword() {
		Spatial ironSword = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/iron_sword.tscn").Instance();
		Equipment equipment = new Equipment(ironSword, EquipmentType.HELD);
		equipment.Damage = 5;
		return equipment;
	}

	private Equipment CreateRustedDagger() {
		Spatial rustedDagger = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/rusted_dagger.tscn").Instance();
		Equipment equipment = new Equipment(rustedDagger, EquipmentType.HELD);
		equipment.Damage = 3;
		return equipment;
	}

	private Equipment CreateLightSaber() {
		Spatial lightSaber = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/light_saber.tscn").Instance();
		Equipment equipment = new Equipment(lightSaber, EquipmentType.HELD);
		equipment.Damage = 10;
		return equipment;
	}

	private Consumable CreateHealingPotion() {
		Spatial healthPotion = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/health_potion.tscn").Instance();
		return new Consumable(40, healthPotion);
	}
}
