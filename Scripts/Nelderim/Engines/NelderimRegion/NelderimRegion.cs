using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Spells.Necromancy;
using Server.Spells.Spellweaving;

namespace Server.Nelderim;

public class NelderimRegion
{
    protected bool Equals(NelderimRegion other)
    {
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }

    internal string Name { get; set; }
    internal string Parent { get; set; }
    internal NelderimRegionSchools BannedSchools { get; set; }
    internal double Female { get; set; } = Double.NaN;
    internal Dictionary<Race, double> Population { get; set; }
    internal Dictionary<Race, double> Intolerance { get; set; }
    internal Dictionary<GuardType, NelderimRegionGuard> Guards { get; set; }
    internal Dictionary<CraftResource, double> Resources { get; set; }

    public bool Validate()
    {
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

    public void ResourceVeins()
    {
        return; //TODO: 
    }
}