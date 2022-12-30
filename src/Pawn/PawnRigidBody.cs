using Godot;
using System;
using Serilog;
using Pawn.Controller;
namespace Pawn {
	public class PawnRigidBody : RigidBody
	{
		//This is the default values of where the PawnController Should be
		[Export]
		private NodePath PawnControllerPath = (NodePath) "..";
		public override void _Ready()
		{
			if(PawnControllerPath == null || PawnControllerPath == "") {
				Log.Error("YOU FORGOT TO SET PAWNCONTROLLER PATH IN PawnRigidBody.cs");
			}
		}

		public PawnController GetPawnController() {
			return this.GetNode<PawnController>(PawnControllerPath);
		}
	}
}
