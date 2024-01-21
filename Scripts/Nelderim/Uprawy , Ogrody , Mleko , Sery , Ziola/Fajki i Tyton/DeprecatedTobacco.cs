
using Server;
using Server.Helpers;

public class NobleTobaccoApple : Server.Items.BaseTobaccoFlavoured
{
	public NobleTobaccoApple() : base()
	{
	}

	public NobleTobaccoApple(int amount) : base(amount)
	{
	}

	public NobleTobaccoApple(Serial serial) : base(serial)
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

		ItemExtension.ReplaceWith(this, new Server.Items.NobleTobaccoApple(Amount));
	}
}

public class NobleTobaccoPear : Server.Items.BaseTobaccoFlavoured
{
	public NobleTobaccoPear() : base()
	{
	}

	public NobleTobaccoPear(int amount) : base(amount)
	{
	}

	public NobleTobaccoPear(Serial serial) : base(serial)
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

		ItemExtension.ReplaceWith(this, new Server.Items.NobleTobaccoPear(Amount));
	}
}

public class NobleTobaccoLemon : Server.Items.BaseTobaccoFlavoured
{
	public NobleTobaccoLemon() : base()
	{
	}

	public NobleTobaccoLemon(int amount) : base(amount)
	{
	}

	public NobleTobaccoLemon(Serial serial) : base(serial)
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

		ItemExtension.ReplaceWith(this, new Server.Items.NobleTobaccoLemon(Amount));
	}
}

public class PlainTobaccoApple : Server.Items.BaseTobaccoFlavoured
{
	public PlainTobaccoApple() : base()
	{
	}

	public PlainTobaccoApple(int amount) : base(amount)
	{
	}

	public PlainTobaccoApple(Serial serial) : base(serial)
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

		ItemExtension.ReplaceWith(this, new Server.Items.PlainTobaccoApple(Amount));
	}
}

public class PlainTobaccoPear : Server.Items.BaseTobaccoFlavoured
{
	public PlainTobaccoPear() : base()
	{
	}

	public PlainTobaccoPear(int amount) : base(amount)
	{
	}

	public PlainTobaccoPear(Serial serial) : base(serial)
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

		ItemExtension.ReplaceWith(this, new Server.Items.PlainTobaccoPear(Amount));
	}
}

public class PlainTobaccoLemon : Server.Items.BaseTobaccoFlavoured
{
	public PlainTobaccoLemon() : base()
	{
	}

	public PlainTobaccoLemon(int amount) : base(amount)
	{
	}

	public PlainTobaccoLemon(Serial serial) : base(serial)
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

		ItemExtension.ReplaceWith(this, new Server.Items.PlainTobaccoLemon(Amount));
	}
}

public class NobleTobacco : Server.Items.BaseTobacco
{
	public NobleTobacco() : base()
	{
	}

	public NobleTobacco(int amount) : base(amount)
	{
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

		ItemExtension.ReplaceWith(this, new Server.Items.NobleTobacco(Amount));
	}
}


public class PlainTobacco : Server.Items.BaseTobacco
{
	public PlainTobacco() : base()
	{
	}

	public PlainTobacco(int amount) : base(amount)
	{
	}

	public PlainTobacco(Serial serial) : base(serial)
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

		ItemExtension.ReplaceWith(this, new Server.Items.PlainTobacco(Amount));
	}
}
