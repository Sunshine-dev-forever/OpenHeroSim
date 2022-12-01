using System.Collections.Generic;
using System.Threading;
using Serilog;
namespace Pawn.Actions {
	public class WaitAction : IAction {

		public int CooldownMilliseconds {get {return 0;} }

		public string Name {get {return "WaitAction";}}
		public float MaxRange {get {return 2;}}
		public List<ActionTags> Tags {get {return new List<ActionTags>();}}

		//@param waitTimeMilliseconds - amount of time to wait
		public void execute(object argsStruct, VisualController visualController) {
			Godot.AnimationPlayer animationPlayer = visualController.GetAnimationPlayer();
			animationPlayer.GetAnimation("Idle").Loop = true;
			animationPlayer.Play("Idle");
			Thread.Sleep(((WaitActionArgs) argsStruct).waitTimeMilliseconds);
		}

		public struct WaitActionArgs{
			public WaitActionArgs (int _waitTimeMilliseconds) {
				waitTimeMilliseconds = _waitTimeMilliseconds;
			}
			public int waitTimeMilliseconds;
		}
	}
}