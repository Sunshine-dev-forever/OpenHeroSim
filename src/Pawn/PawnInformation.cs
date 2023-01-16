using System.Collections.Generic;
using Godot;
using System;
using Serilog;
using Pawn.Controller;
using Pawn.Item;
using Pawn.Action.Ability;
using Pawn.Action;
using System.Linq;

namespace Pawn {

	public class PawnInformation
	{
		struct AbilityStruct {
			public AbilityStruct(IAbility _ability, DateTime _timeLastUsed) {
				ability = _ability;
				timeLastUsed = _timeLastUsed;
			}
			public IAbility ability;
			public DateTime timeLastUsed;
		}

		private Dictionary<string, AbilityStruct> abilitiesDict = new Dictionary<string, AbilityStruct>();

		public PawnInformation() {
			//TODO: this should be in the PawnControllerBuilder
			AddAbility(new StabAbility());
		}

		public void AddAbility(IAbility ability) {
			if(!abilitiesDict.ContainsKey(ability.Name)) {
				abilitiesDict.Add(ability.Name, new AbilityStruct(ability, DateTime.MinValue));
			}
		}

		public List<IAbility> GetAllAbilitiesWithTags(List<ActionTags> actionTags, bool canBeOnCooldown) {
			//Convert to dictionary values to IEnumerable<ActionStruct>
			IEnumerable<AbilityStruct> actionStructs = abilitiesDict.Values.AsEnumerable();
			//Get all actions with the specified tags
			actionStructs = actionStructs.Where( (actionStruct) => actionStruct.ability.Tags.Intersect(actionTags).Count() == actionTags.Count);
			//filter out on CD stuff
			if(!canBeOnCooldown) {
				actionStructs = actionStructs.Where( (actionStruct) =>  
													(DateTime.Now - actionStruct.timeLastUsed).TotalMilliseconds 
													> actionStruct.ability.CooldownMilliseconds);
			}
			return actionStructs.Select((actionStruct) => actionStruct.ability).ToList();
		}

		public bool HasAbility(string abilityName) {
			return abilitiesDict.ContainsKey(abilityName);
		}

		public void UpdateAbilityLastUsed(string abilityName, DateTime dateTime) {
			abilitiesDict[abilityName] = new AbilityStruct(abilitiesDict[abilityName].ability, dateTime);
		}

		public const String NO_FACTION = "none";
		//TODO: any way to get around using strings for factions?
		public string Faction {get; set;} = NO_FACTION;
		public string Name {get; set;} = "Testy McTesterson";
		public double BaseDamage {get; set;} = 10;
		public double Health {get; set;} = 100;
		public double MaxHealth {get; set;} = 100;	
		//TODO: this should be used
		public float Speed {get; set;} = 10;	
	}
}
