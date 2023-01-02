using Godot;
using System;
using System.Collections.Generic;
using Serilog;
using Pawn.Action;
using System.Threading.Tasks;
using Pawn;
using Pawn.Controller;
using Pawn.Goal;

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
			Adhoc2();
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

	private void CreatePawn(){
		Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
		
		Vector3 location = GenerateRandomVector();
		
		PawnControllerBuilder.Start(this, kdTreeController, navigation)
							.AddGoal(new DefendSelfGoal())
							.AddGoal(new LootGoal())
							.AddGoal(new WanderGoal())
							.Location(location)
							.Weapon(GetRandomWeapon())
							.Finish();		
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

	private Weapon GetRandomWeapon() {
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
		CreateItemContainer();
	}

	private void CreateHealingPotionTester() {
				Navigation navigation = GetNode<Navigation>("/root/Spatial/Navigation");
		PawnControllerBuilder.Start(this, kdTreeController, navigation)
							.AddGoal(new HealGoal())
							.AddGoal(new WanderGoal())
							.Location(new Vector3(0,5,0))
							.Weapon(CreateOPLightSaber())
							.AddItem(CreateHealingPotion()).AddItem(CreateHealingPotion())
							.DealDamage(50)
							.Finish();
	}

	private void CreateItemContainer() {
		Spatial TreasureChestMesh = (Spatial) GD.Load<PackedScene>("res://scenes/world_objects/treasure_chest.tscn").Instance();
		//The iron sword gets leaked when created like this
		ItemContainer itemContainer = new ItemContainer(CreateIronSword(), TreasureChestMesh);
		this.AddChild(itemContainer);
		itemContainer.GlobalTransform = new Transform(itemContainer.GlobalTransform.basis, new Vector3(0,1,0));
		kdTreeController.AddInteractable(itemContainer);
	}

	private Weapon CreateIronSword() {
		Spatial ironSword = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/iron_sword.tscn").Instance();
		return new Weapon(5, ironSword);
	}

	private Weapon CreateRustedDagger() {
		Spatial rustedDagger = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/rusted_dagger.tscn").Instance();
		return new Weapon(4, rustedDagger);
	}

	private Weapon CreateLightSaber() {
		Spatial lightSaber = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/light_saber.tscn").Instance();
		return new Weapon(6, lightSaber);
	}

	private Weapon CreateOPLightSaber() {
		Spatial lightSaber = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/light_saber.tscn").Instance();
		return new Weapon(50, lightSaber);
	}

	private Item CreateHealingPotion() {
		Spatial healthPotion = (Spatial) GD.Load<PackedScene>("res://scenes/weapons/health_potion.tscn").Instance();
		return new Item(40, healthPotion);
	}
}
