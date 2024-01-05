using Server;
using Server.Engines.HunterKiller;
using Server.Items;

public interface ISmokable
{
    void OnSmoke(Mobile m);
}

public abstract class BaseTobacco : Item, ISmokable
{
    public virtual void OnSmoke(Mobile m)
    {
        m.SendMessage("Dym tytoniowy napelnia twoje pluca.");

        m.Emote("*wypuszcza z ust kleby fajkowego dymu*");
        Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 9965);
        m.PlaySound(0x15F);
        m.RevealingAction();
    }

    public BaseTobacco() : this(1)
    {
    }

    public BaseTobacco(int amount) : base(0x11EB)
    {
        Stackable = true;
        Weight = 0.025;
        Amount = amount;
    }

    public BaseTobacco(Serial serial) : base(serial)
    {
    }

    public override void Serialize(GenericWriter writer)
    {
        base.Serialize(writer);

        writer.Write((int)0); // version
    }

    public override void Deserialize(GenericReader reader)
    {
        base.Deserialize(reader);

        int version = reader.ReadInt();
    }
}
