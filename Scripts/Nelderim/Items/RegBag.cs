using Server;
using Server.Items;
using Server.Mobiles;
using System;
using Server.ACC.CSS;
using Server.Engines.Craft;

namespace Items.RegBag
{
    public class RegBag : Bag
    {
        [CommandProperty( AccessLevel.GameMaster)]
        public int WeightReduction
        {
            get{ return m_WeightReduction; }
            set{ m_WeightReduction = value; InvalidateProperties(); }
        }

        private Mobile m_Crafter;
        private ClothingQuality m_Quality;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
	        get{ return m_Crafter; }
	        set{ m_Crafter = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public ClothingQuality Quality
        {
	        get{ return m_Quality; }
	        set{ m_Quality = value; InvalidateProperties(); }
        }
        
        private int m_WeightReduction;

        [Constructable]
        public RegBag()
        {
            this.Name = "worek na reagenty";
            this.Weight = 1;
            this.Hue = 0;

        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(dropped is BaseReagent || dropped is Kindling || dropped is CReagent || dropped  is BaseTobacco ))
            {
                from.SendMessage("Nie mozesz tego umiescic w wroku na reagenty.");
                return false;
            }
            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!(item is BaseReagent || item is Kindling || item is CReagent || item is BaseTobacco ))
            {
                from.SendMessage("Nie mozesz tego umiescic w wroku na reagenty.");
                return false;
            }
            return base.OnDragDropInto(from, item, p);
        }

        public override int GetTotal( TotalType type )
        {
            int total = base.GetTotal( type );

            if ( type == TotalType.Weight )
                total -= total * m_WeightReduction / 100;

            return total;
        }

        public override void  UpdateTotal( Item sender, TotalType type, int delta )
        {
            InvalidateProperties();

            base.UpdateTotal(sender, type, delta);
        }
        
        public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
				
			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == ClothingQuality.Exceptional )
				list.Add( 1063341 ); // exceptional

			int prop;
			
			if ( (prop = m_WeightReduction) != 0 )
				list.Add( 1072210, prop.ToString() ); // Weight reduction: ~1_PERCENTAGE~%	
		}
		
		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		[Flags]
		private enum SaveFlag
		{
			None				= 0x00000000,
			Attributes			= 0x00000001,
			DamageModifier		= 0x00000002,
			LowerAmmoCost		= 0x00000004,
			WeightReduction		= 0x00000008,
			Crafter				= 0x00000010,
			Quality				= 0x00000020,
			Capacity			= 0x00000040,
			DamageIncrease		= 0x00000080
		}

        public RegBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) //TODO: serialise it better, mate 
        {
            base.Serialize(writer);
            writer.Write((int)1); // version 

            
            SaveFlag flags = SaveFlag.None;


            SetSaveFlag( ref flags, SaveFlag.WeightReduction,	m_WeightReduction != 0 );
            SetSaveFlag( ref flags, SaveFlag.Crafter,			m_Crafter != null );
            SetSaveFlag( ref flags, SaveFlag.Quality,			true );

            writer.WriteEncodedInt( (int) flags );
            

            if ( GetSaveFlag( flags, SaveFlag.WeightReduction ) )
	            writer.Write( (int) m_WeightReduction );

            if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
	            writer.Write( (Mobile) m_Crafter );

            if ( GetSaveFlag( flags, SaveFlag.Quality ) )
	            writer.Write( (int) m_Quality );
            
        }

        public override void Deserialize(GenericReader reader)
        {
	        base.Deserialize(reader);
	        int version = reader.ReadInt();
    
	        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

	        if (GetSaveFlag(flags, SaveFlag.WeightReduction))
		        m_WeightReduction = reader.ReadInt();

	        if (GetSaveFlag(flags, SaveFlag.Crafter))
		        m_Crafter = reader.ReadMobile();

	        if (GetSaveFlag(flags, SaveFlag.Quality))
		        m_Quality = (ClothingQuality)reader.ReadInt();
        }


        public void InvalidateWeight()
        {
	        if ( RootParent is Mobile )
	        {
		        Mobile m = (Mobile) RootParent;

		        m.UpdateTotals();
	        }
        }
        
        #region ICraftable
        public virtual int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2, BaseTool tool, CraftItem craftItem, int resHue )
        {
	        Quality = (ClothingQuality) quality;

	        if ( makersMark )
		        Crafter = from;

	        return quality;
        }
        #endregion
    }
}
