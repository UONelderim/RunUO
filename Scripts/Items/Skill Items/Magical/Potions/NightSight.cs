using System;
using Server;

namespace Server.Items
{
	public class NightSightPotion : BasePotion
	{
        public static int NightSightBonus = 12;
        
        [Constructable]
		public NightSightPotion(int amount) : base( 0xF06, PotionEffect.Nightsight )
		{
            Amount = amount;
		}

		[Constructable]
		public NightSightPotion() : this(1)
		{
		}

		public NightSightPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Drink( Mobile from )
		{
            if (from.BeginAction(typeof(LightCycle)))
			{
				new LightCycle.NightSightTimer( from ).Start();

                from.LightLevel = NightSightPotion.NightSightBonus;

				from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
				from.PlaySound( 0x1E3 );

				BasePotion.PlayDrinkEffect( from );

				this.Consume();
			}
			else
			{
				from.SendMessage( "Masz juz wzmocniony wzrok." );
			}
		}
	}
}