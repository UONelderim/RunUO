using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Spells.Necromancy;
using Server.Spells.Spellweaving;

namespace Server.Nelderim;

public class NelderimRegion
{
    internal string Name { get; set; }
    internal string Parent { get; set; }
    internal NelderimRegionSchools BannedSchools { get; set; }
    internal double Female { get; set; } = double.NaN;
    internal Dictionary<Race, double> Population { get; set; }
    internal Dictionary<Race, double> Intolerance { get; set; }
    internal Dictionary<GuardType, NelderimRegionGuard> Guards { get; set; }
    internal Dictionary<CraftResource, double> Resources { get; set; }

    public bool Validate()
    {
        var populationSum = Population.Values.Sum();
        if (Math.Abs(populationSum - 1.0) > 0.001)
        {
            Console.WriteLine($"Population sum for region {Name} is incorrect. Expected: 1.0. Acutal: {populationSum}");
        }
        foreach (var kvp in Guards)
        {
            var guardType = kvp.Key;
            var guardDef = kvp.Value;
            var guardDefPopulationSum = guardDef.Population.Values.Sum();
            if (Math.Abs(guardDefPopulationSum - 1.0) > 0.001)
            {
                Console.WriteLine($"Guard population sum for region {Name} for type {guardType} is incorrect. Expected: 1.0. Acutal: {guardDefPopulationSum}");
            }
        }
        //TODO: Check Population, Resources and Guards
        return true;
    }

    private NelderimRegion GetParent => NelderimRegionSystem.GetRegion(Parent);

    public double FemaleChance()
    {
        if (!double.IsNaN(Female))
        {
            return Female;
        }

        var parentResult = GetParent?.FemaleChance();
        if (parentResult.HasValue)
        {
            return parentResult.Value;
        }
        Console.WriteLine($"Unable to get female for region {Name}");
        return 0.5;
    }

    public Race RandomRace()
    {
        if (Population is { Count: > 0 })
        {
            return Utility.RandomWeigthed(Population);
        }

        var parentResult = GetParent?.RandomRace();
        if (parentResult != null)
        {
            return parentResult;
        }
        Console.WriteLine($"Unable to get race for region {Name}");
        return None.Instance;
    }

    public NelderimGuardProfile GuardProfile(GuardType guardType)
    {
        if (Guards is { Count: > 0 })
        {
            if(Guards.TryGetValue(guardType, out var profile))
            {
                return NelderimRegionSystem.GetGuardProfile(profile.Name);
            }
        }
        var parentResult = GetParent?.GuardProfile(guardType);
        if (parentResult != null)
        {
            return parentResult;
        }

        Console.WriteLine($"Unable to get guard profile for {guardType} for region {Name}");
        return default;
    }
    
    public bool CastIsBanned(Spell spell)
    {
        if (BannedSchools != null)
        {
            return spell switch
            {
                MagerySpell => BannedSchools.Magery,
                NecromancerSpell => BannedSchools.Necromancy,
                PaladinSpell => BannedSchools.Chivalry,
                ArcanistSpell => BannedSchools.Spellweaving,
                _ => false
            };
        }
        var parentResult = GetParent?.CastIsBanned(spell);
        if (parentResult.HasValue)
        {
            return parentResult.Value;
        }
        Console.WriteLine($"Unable to get banned cast for {spell.GetType().Name} for region {Name}");
        return default;
    }
    
    public bool PetIsBanned(BaseCreature pet)
    {
        if (BannedSchools != null)
        {
            return pet switch
            {
                BaseFamiliar => BannedSchools.Necromancy,
                SummonedAirElemental or SummonedEarthElemental or SummonedFireElemental or SummonedFireElemental or 
                    SummonedPoisonElemental or SummonedDaemon=> BannedSchools.Magery,
                _ => false
            };
        }
        var parentResult = GetParent?.PetIsBanned(pet);
        if (parentResult.HasValue)
        {
            return parentResult.Value;
        }
        Console.WriteLine($"Unable to get banned pet for {pet.GetType().Name} for region {Name}");
        return default;
    }

    public void MakeGuard(BaseNelderimGuard guard)
    {
        //We need Race and Gender first
        if (Guards.TryGetValue(guard.Type, out var guardDefinition))
        {
            guard.Race = Utility.RandomWeigthed(guardDefinition.Population);
            guard.Female = Utility.RandomDouble() < guardDefinition.Female;
        }
        else
        {
            Console.WriteLine($"Unable to get guard definition for {guard.Type} for region {Name}");
        }
        GuardProfile(guard.Type).Make(guard);
    }

    public Dictionary<CraftResource, double> ResourceVeins()
    {
        if (Resources is { Count: > 0 })
        {
            return Resources;
        }

        var parentResult = GetParent?.ResourceVeins();
        if (parentResult != null)
        {
            return parentResult;
        }
        return new Dictionary<CraftResource, double>();
    }
    
    protected bool Equals(NelderimRegion other)
    {
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }
}