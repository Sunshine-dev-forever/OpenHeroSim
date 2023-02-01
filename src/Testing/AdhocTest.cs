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

//TODO: I need more examples on this
[System.Diagnostics.CodeAnalysis.SuppressMessage("Non-nullable", "CA8618:must contain non-null value exiting constructor", Justification = "Not production code.")]
public class AdhocTest : Node
{

	private KdTreeController kdTreeController = null!; 
	public override void _Ready()
	{
		kdTreeController = new KdTreeController();
		this.AddChild(kdTreeController);
	}
	
	public override void _Input(InputEvent input) {
		 if(input.IsActionPressed("mouse_left_click")) {
			//Adhoc();
		 } else if(input.IsActionPressed("ui_left")) {
			//Adhoc2();
		 } else if (input.IsActionPressed("ui_right")) {
			CreateHealingPotionTester();
		 }
	}
	private float TimeSinceLastPawnCreation = 4;
	public override void _Process(float delta)
	{
		TimeSinceLastPawnCreation += delta;
		if(TimeSinceLastPawnCreation > 5) {
			TimeSinceLastPawnCreation = 0;
			CreatePawn();
		}
		
	}

	private PawnController CreatePawn(){
		Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
		
		Vector3 location = GenerateRandomVector();
		
		return PawnControllerBuilder.Start(this, kdTreeController, navigation)
							.AddGoal(new HealGoal())
							.AddGoal(new DefendSelfGoal())
							.AddGoal(new LootGoal())
							.AddGoal(new WanderGoal())
							.AddAbility(new StabAbility())
							.WearEquipment(GetRandomWeapon())
							.WearEquipment(GetHelmet())
							.Location(location)
							.Finish();		
	}

	private Equipment GetHelmet() {
		Spatial boxHelm = (Spatial) GD.Load<PackedScene>("res://scenes/world_objects/box_helm.tscn").Instance();
		Equipment equipment = new Equipment(boxHelm, EquipmentType.HEAD);
		equipment.Defense = 5;
		return equipment;
	}

	private Vector3 GenerateRandomVector() {
		Random rand = new Random();
		int rng = rand.Next(0,3);
		if(rng == 0) {
			return new Vector3(23,5,23);
		} else if (rng == 1) {
			return new Vector3(-23,5,23);
		} else if (rng == 2) {
			return new Vector3(-23, 5, -23);
		} else if (rng == 3) {
			return new Vector3(23, 5, -23);
		}
		return new Vector3();
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
	private void Adhoc2(){

	}

	private PawnController CreateHealingPotionTester() {
		Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
		return PawnControllerBuilder.Start(this, kdTreeController, navigation)
							.AddGoal(new HealGoal())
							.AddGoal(new WanderGoal())
							.Location(new Vector3(0,5,0))
							.WearEquipment(CreateLightSaber())
							.AddConsumable(CreateHealingPotion()).AddConsumable(CreateHealingPotion())
							.DealDamage(50)
							.Finish();
	}

	private void CreateItemContainer() {
		Spatial TreasureChestMesh = (Spatial) GD.Load<PackedScene>("res://scenes/world_objects/treasure_chest.tscn").Instance();
		//The iron sword gets leaked when created like this
		List<IItem> items = new List<IItem>();
		items.Add(CreateHealingPotion());
		items.Add(CreateIronSword());
		ItemContainer itemContainer = new ItemContainer(items, TreasureChestMesh);
		this.AddChild(itemContainer);
		itemContainer.GlobalTransform = new Transform(itemContainer.GlobalTransform.basis, new Vector3(0,1,0));
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
