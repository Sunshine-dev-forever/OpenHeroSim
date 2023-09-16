using Pawn.Action.Ability;
using System.Collections.Generic;

namespace Pawn;

public interface IPawnInformation
{
    //Pawns will attack all others of a different faction
    public const string NO_FACTION = "none";
    string Faction { get; set; }
    string Name { get; set; }
    double BaseDamage { get; set; }
    double Health { get; set; }
    double MaxHealth { get; set; }
    float Speed { get; set; }

    void AddAbility(IAbility ability);
    List<IAbility> GetAllUsableAbilities(IPawnController ownerPawnController, IPawnController otherPawnController);
    bool HasAbility(string abilityName);
    public List<IAbility> GetAllAbilites();

}