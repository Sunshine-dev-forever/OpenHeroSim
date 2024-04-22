using Pawn.Action.Ability;
using System.Collections.Generic;
using System.Linq;

namespace Pawn;

public class PawnInformation : IPawnInformation {
    public string Faction { get; set; } = IPawnInformation.NO_FACTION;
    public string Name { get; set; } = "Testy McTesterson";
    public double BaseDamage { get; set; } = 10;
    public double Health { get; set; } = 100;
    public double MaxHealth { get; set; } = 100;
    public float Speed { get; set; } = 10;

    readonly List<IAbility> abilities = new();

    // TODO: we have no duplicate detection for now
    public void AddAbility(IAbility ability) => abilities.Add(ability);

    public List<IAbility> GetAllUsableAbilities(IPawnController ownerPawnController, IPawnController otherPawnController) =>
        // filter out abilities that can not be used in current envirionmental context or are on cool down
        abilities.Where((ability) => ability.CanBeUsed(ownerPawnController)).ToList();

    public List<IAbility> GetAllAbilites() => abilities;

    public bool HasAbility(string abilityName) => abilities.Any((ability) => ability.Name == abilityName);
}
