using System.Collections.Generic;
using System.Threading;
using Serilog;
using Pawn.Controller;
namespace Pawn.Action {
	public class WaitAction : IAction {
		private PawnController? ownerPawnController;
		public int CooldownMilliseconds {get {return 0;} }

		public string Name {get {return "WaitAction";}}
		public float MaxRange {get {return 2;}}
		public List<ActionTags> Tags {get {return new List<ActionTags>();}}

		private int waitTimeMilliseconds;

		public WaitAction(){}
		public WaitAction(PawnController _ownerPawnController, int _waitTimeMilliseconds){
			ownerPawnController = _ownerPawnController;
			waitTimeMilliseconds = _waitTimeMilliseconds;
		}

		//TODO: duplicate should really only have to be used for the combat actions
		public IAction Duplicate(PawnController _ownerPawnController, PawnController _otherPawnController) {
			Log.Warning("Duplicate was called on WaitAction, unintended events may follow");
			int THOUSAND_MILLISECONDS = 1000;
			return new WaitAction(_ownerPawnController, THOUSAND_MILLISECONDS);
		}

		//@param waitTimeMilliseconds - amount of time to wait
		public void execute() {
			ownerPawnController.VisualController.SetAnimation(AnimationName.Idle, true);
			Thread.Sleep(waitTimeMilliseconds);
		}
	}
}