using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13B2, 0x13B1 )]
	public class JukaBow : Bow
	{
		public override int AosStrengthReq{ get{ return 70; } }
		public override int AosDexterityReq{ get{ return 70; } }

		public override int OldStrengthReq{ get{ return 80; } }
		public override int OldDexterityReq{ get{ return 80; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsModified
		{
			get{ return ( Hue == 0x453 ); }
		}

		[Constructable]
		public JukaBow()
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsModified )
			{
				from.SendMessage( "That has already been modified." );
			}
			else if ( !IsChildOf( from.Backpack ) )
			{
				from.SendMessage( "This must be in your backpack to modify it." );
			}
			else if ( from.Skills[SkillName.Fletching].Base < 100.0 )
			{
				from.SendMessage( "Only a grandmaster bowcrafter can modify this weapon." );
			}
			else
			{
				from.BeginTarget( 2, false, Targeting.TargetFlags.None, new TargetCallback( OnTargetGears ) );
				from.SendMessage( "Select the gears you wish to use." );
			}
		}

		public void OnTargetGears( Mobile from, object targ )
		{
			Gears g = targ as Gears;

			if ( g == null || !g.IsChildOf( from.Backpack ) )
			{
				from.SendMessage( "Those are not gears." ); // Apparently gears that aren't in your backpack aren't really gears at all. :-(
			}
			else if ( IsModified )
			{
				from.SendMessage( "That has already been modified." );
			}
			else if ( !IsChildOf( from.Backpack ) )
			{
				from.SendMessage( "This must be in your backpack to modify it." );
			}
			else if ( from.Skills[SkillName.Fletching].Base < 100.0 )
			{
				from.SendMessage( "Only a grandmaster bowcrafter can modify this weapon." );
			}
			else
			{
				g.Consume();

				Hue = 0x453;
				Slayer = (SlayerName)Utility.Random( 2, 25 );
				//Attributes.AttackChance = Utility.Random( 2, 10 );
				//Attributes.WeaponDamage =  Utility.Random( 10, 25 );
				
				double AttributesChance = Utility.RandomDouble();
				int AttributeCount;
				
					if ( AttributesChance >= 0.85 )
						AttributeCount = 3;
					else if ( AttributesChance >= 0.5 )
						AttributeCount = 2;
					else
						AttributeCount = 1;
				
				for ( int i = 0; i < AttributeCount; i++ )
					{
						int chance = Utility.Random( 8 );
						switch( chance )
						{
							case 0:
								{
									Attributes.AttackChance = Utility.Random ( 5, 10 );
									break;
								}
							case 1:
								{
									Attributes.DefendChance = Utility.Random ( 5, 10 );
									break;
								}
							case 2:
								{
									Attributes.WeaponDamage = Utility.Random ( 10, 30 );
									break;
								}
							case 3:
								{
									Attributes.WeaponSpeed = Utility.Random ( 10, 20 );
									break;
								}
							case 4:
								{
									WeaponAttributes.HitLeechHits = Utility.Random ( 10, 30 );
									break;
								}
							case 5:
								{
									WeaponAttributes.HitLeechMana = Utility.Random ( 10, 30 );
									break;
								}								
							case 6:
								{
									WeaponAttributes.HitLeechStam = Utility.Random ( 10, 30 );
									break;
								}
							case 7:
								{
									switch ( Utility.Random( 4 ) )
									{
										case 0: WeaponAttributes.HitMagicArrow = Utility.Random ( 10, 30 ); break;
										case 1: WeaponAttributes.HitHarm = Utility.Random ( 10, 30 ); break;
										case 2: WeaponAttributes.HitFireball = Utility.Random ( 10, 30 ); break;
										case 3: WeaponAttributes.HitLightning = Utility.Random ( 10, 30 ); break;
									}
									break;
								}
							
						}
					}
				
				
				
				
				
				
				

				from.SendMessage( "You modify it." );
			}
		}

		public JukaBow( Serial serial ) : base( serial )
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
	}
}