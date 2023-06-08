using System.Collections.Generic;
using Godot;
using System;
using Serilog;
using Pawn;
using Item;
using Pawn.Action.Ability;
using Pawn.Action;
using System.Linq;

namespace Pawn {

	public class PawnInformation : IPawnInformation
	{
		private List<IAbility> abilities = new List<IAbility>();

		public void AddAbility(IAbility ability) {
			//TODO: we have no duplicate detection for now
			abilities.Add(ability);
		}

		public List<IAbility> GetAllUsableAbilities(IPawnController ownerPawnController, IPawnController otherPawnController) {
			//filter out abilities that can not be used in current envirionmental context or are on cool down
			return abilities.Where<IAbility>( (ability) => (ability.CanBeUsed(ownerPawnController))).ToList();;
		}

		public bool HasAbility(string abilityName) {
			return abilities.Any<IAbility>( (ability) => ability.Name == abilityName);
		}
		public string Faction {get; set;} = IPawnInformation.NO_FACTION;
		public string Name {get; set;} = "Testy McTesterson";
		public double BaseDamage {get; set;} = 10;
		public double Health {get; set;} = 100;
		public double MaxHealth {get; set;} = 100;	
		public float Speed {get; set;} = 10;	
	}
}
