using Server.Engines.Craft;
using Server.Items;
using Server;
using System;
using Server.Engines.HunterKiller;

public enum TobaccoFlavour
{
    None = 0,
    Apple,
    Pear,
    Lemon,
}

public abstract class BaseTobaccoFlavoured : BaseTobacco
{
    public override void OnSmoke(Mobile m)
    {
        m.SendMessage("Wspaniale pachnacy dym tytoniowy napelnia twoje pluca.");

        m.Emote("*wypuszcza z ust kleby aromatycznego fajkowego dymu");
        Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 9965);
        m.PlaySound(0x15F);
        m.RevealingAction();
    }

    private TobaccoFlavour m_Flavour;
    public TobaccoFlavour Flavour
    {
        get
        {
            return m_Flavour;
        }
        set
        {
            m_Flavour = value;
            InvalidateProperties();
        }
    }

    public override void GetProperties(ObjectPropertyList list)
    {
        base.GetProperties(list);
        switch (Flavour)
        {
            case TobaccoFlavour.None:
                break;
            case TobaccoFlavour.Apple:
                list.Add(1061201, "jablkiem"); // Aromatyzowany ~1_val~
                break;
            case TobaccoFlavour.Pear:
                list.Add(1061201, "gruszka"); // Aromatyzowany ~1_val~
                break;
            case TobaccoFlavour.Lemon:
                list.Add(1061201, "cytryna"); // Aromatyzowany ~1_val~
                break;
            default:
                list.Add(1061201, "czyms dziwnym"); // Aromatyzowany ~1_val~
                break;
        }
    }

    //public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2, BaseTool tool, CraftItem craftItem, int resHue)
    //{
    //    TobaccoFlavour tf = TobaccoFlavour.None;
    //    foreach (CraftRes res in craftItem.Ressources)
    //    {
    //        if (res.ItemType == typeof(Apple))
    //            tf = TobaccoFlavour.Apple;
    //        else if (res.ItemType == typeof(Pear))
    //            tf = TobaccoFlavour.Pear;
    //        else if (res.ItemType == typeof(Lemon))
    //            tf = TobaccoFlavour.Lemon;

    //        if (tf != TobaccoFlavour.None)
    //            break;
    //    }

    //    Flavour = tf;

    //    return 1; // regular quality
    //}

    public BaseTobaccoFlavoured() : base()
    {
    }

    public BaseTobaccoFlavoured(int amount) : base(amount)
    {
    }

    public BaseTobaccoFlavoured(Serial serial) : base(serial)
    {
    }

    public override void Serialize(GenericWriter writer)
    {
        base.Serialize(writer);

        writer.Write((int)0); // version
        writer.Write((int)Flavour);
    }

    public override void Deserialize(GenericReader reader)
    {
        base.Deserialize(reader);

        int version = reader.ReadInt();
        Flavour = (TobaccoFlavour)reader.ReadInt();
    }
}