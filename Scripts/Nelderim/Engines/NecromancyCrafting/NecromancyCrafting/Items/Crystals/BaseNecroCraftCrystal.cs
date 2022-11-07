using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseNecroCraftCrystal : Item
	{
		private static Dictionary<Type, string> BodyPartName = new Dictionary<Type, string>()
		{
			
			{typeof(RottingLegs), "gnijące nogi"} ,
			{typeof(RottingTorso), "gnijący tułów "} ,
			{typeof(SkeletonLegs), "nogi szkieleta"} ,
			{typeof(SkeletonMageTorso), "tulow szkieleta maga"} ,
			{typeof(SkeletonTorso), "tulow szkieleta"} ,
			{typeof(WrappedLegs), "zmumifikowane nogi"} ,
			{typeof(WrappedMageTorso), "zmumifikowany tułów oznaczony runami"} ,
			{typeof(WrappedTorso), "zmumifikowany tułów"} ,
			{typeof(Phylacery), "filakterium"} ,
			{typeof(Brain), "mozg"} ,
			
		};
		
		public abstract double RequiredNecroSkill { get; }
		
		public abstract Type[] RequiredBodyParts { get; }
		
		public int[] RequiredBodyPartsAmounts
		{
			get { return RequiredBodyParts.Select(x => 1).ToArray(); }
		}
		
		public abstract Type SummonType { get; }
		
		public  BaseNecroCraftCrystal() : base( 0x1F19 )
		{
			Weight = 1.0;
		}

		public  BaseNecroCraftCrystal( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}

			double NecroSkill = from.Skills[SkillName.Necromancy].Value;

			if ( NecroSkill < RequiredNecroSkill )
			{
				from.SendMessage( String.Format("Musisz mieć przynajmniej {0:F1} umiejętności nekromancji, by stworzyć szkieleta.", RequiredNecroSkill));
				return;
			}
			

			Container pack = from.Backpack;

			if ( pack == null )
				return;
			int res = pack.ConsumeTotal(RequiredBodyParts, RequiredBodyPartsAmounts);

			if (res != -1)
			{
				
				if (BodyPartName.ContainsKey(RequiredBodyParts[res]))
				{
					from.SendMessage("Musisz miec " + BodyPartName[RequiredBodyParts[res]]);
				}
				else
				{
					from.SendMessage("Musisz miec " + RequiredBodyParts[res].Name);
				}
				
				if (from.AccessLevel > AccessLevel.Player)
        		{
        			from.SendMessage("Boskie moce pomagają ci stworzyć przywołańca bez wszystkich części ciała");
        		}
				else
				{
					return;
				}
			}
			BaseCreature m = (BaseCreature) Activator.CreateInstance( SummonType );
			BaseCreature.Summon(m, from, from.Location, 0x241, TimeSpan.Zero);
			Scale(m, NecroSkill / 100);
			Delete();
		}

		private void Scale(BaseCreature m, double scalar)
		{
			
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}