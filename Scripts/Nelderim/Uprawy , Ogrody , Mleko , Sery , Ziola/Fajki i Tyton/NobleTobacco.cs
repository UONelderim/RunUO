using Server;
using Server.Items;


public class NobleTobacco : BaseTobacco
{
    public override void OnSmoke(Mobile m)
    {
        m.SendMessage("Dym tytoniowy napelnia twoje pluca, czyjesz przyjemne mrowienie w ustach.");

        m.Emote("*wypuszcza z ust wirujace kleby fajkowego dymu*");
        Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 9965);
        m.PlaySound(0x15F);
        m.RevealingAction();
    }

    [Constructable]
    public NobleTobacco() : this(1)
    {
    }

    [Constructable]
    public NobleTobacco(int amount) : base(amount)
    {
        Name = "tyton szlachetny";
        Hue = 2126;
    }

    public NobleTobacco(Serial serial) : base(serial)
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
