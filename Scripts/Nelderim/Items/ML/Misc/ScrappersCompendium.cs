using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class ScrappersCompendium : Spellbook
	{

	
		[Constructable]
		public ScrappersCompendium() : base()
		{
			Hue = 0x494;
			Name = "Kompedium Wszelkiej Wiedzy";
			Attributes.SpellDamage = 10;
			Attributes.LowerManaCost = 8;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
		}

		public ScrappersCompendium( Serial serial ) : base( serial )
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
		
		/*public override int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			double magery = from.Skills.Magery.Value - 100;
			
			if ( magery < 0 )
				magery = 0;
					
			int count = (int) Math.Round( magery * Utility.RandomDouble() / 5 );
			
			if ( count > 2 )
				count = 2;
				
			if ( Utility.RandomDouble() < 0.5 )
				count = 0;
			else
				BaseRunicTool.ApplyAttributesTo( this, false, 0, count, 70, 80 );
				
			Attributes.SpellDamage = 25;
			Attributes.LowerManaCost = 10;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
			
			if ( makersMark )
				Crafter = from;
				
			return quality;
		}*/
	}
}

