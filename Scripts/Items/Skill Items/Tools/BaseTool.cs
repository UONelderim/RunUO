using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items
{
	

	public enum ToolQuality
	{
		Low,
		Regular,
		Exceptional
	}

	// 09.09.2013 :: mortuus - wspolczynniki zwiekszajace ilosc uzyc narzedzi zaleznie od materialu uzyteko do ich wykonania
	public abstract class BaseTool : Item, IUsesRemaining, ICraftable
	{
		private static Hashtable m_MetalBonus;
		public static double getMaterialBonus(Type typeRes)
		{
			if( typeRes == null )
				return 1.0;	// 100% means no additional bonus

			if( m_MetalBonus == null )
			{
				m_MetalBonus = new Hashtable();
				m_MetalBonus[typeof(IronIngot)]       = 1.0;
				m_MetalBonus[typeof(DullCopperIngot)] = 1.2;
				m_MetalBonus[typeof(ShadowIronIngot)] = 1.4;
				m_MetalBonus[typeof(CopperIngot)]     = 1.6;
				m_MetalBonus[typeof(BronzeIngot)]     = 1.8;
				m_MetalBonus[typeof(GoldIngot)]       = 2.0;
				m_MetalBonus[typeof(AgapiteIngot)]    = 2.3;
				m_MetalBonus[typeof(VeriteIngot)]     = 2.7;
				m_MetalBonus[typeof(ValoriteIngot)]   = 3.1;
			}

			Object ret  = m_MetalBonus[typeRes];
			if( ret == null )
				return 1.0;

			return (double) m_MetalBonus[typeRes];
		}

		private Mobile m_Crafter;
		private ToolQuality m_Quality;
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ToolQuality Quality
		{
			get{ return m_Quality; }
			set{ UnscaleUses(); m_Quality = value; InvalidateProperties(); ScaleUses(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		public void ScaleUses()
		{
			m_UsesRemaining = (m_UsesRemaining * GetUsesScalar()) / 100;
			InvalidateProperties();
		}

		public void UnscaleUses()
		{
			m_UsesRemaining = (m_UsesRemaining * 100) / GetUsesScalar();
		}

		public int GetUsesScalar()
		{
			if ( m_Quality == ToolQuality.Exceptional )
				return 150;
			if ( m_Quality == ToolQuality.Low )
				return 50;

			return 100;
		}

		public bool ShowUsesRemaining{ get{ return true; } set{} }

		public abstract CraftSystem CraftSystem{ get; }

		public BaseTool( int itemID ) : this( Utility.RandomMinMax( 24, 26 ), itemID )
		{
		}

		public BaseTool( int uses, int itemID ) : base( itemID )
		{
			// 27.01.2013 mortuus - domyslnie narzedzia beda niskiej jakosci:
			m_UsesRemaining = uses;
			m_Quality = ToolQuality.Low;
		}

		public BaseTool( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			// Makers mark not displayed on OSI
			//if ( m_Crafter != null )
			//	list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == ToolQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public virtual void DisplayDurabilityTo( Mobile m )
		{
			LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString() ); // Durability
		}

		public static bool CheckAccessible( Item tool, Mobile m )
		{
			return ( tool.IsChildOf( m ) || tool.Parent == m );
		}

		public static bool CheckTool( Item tool, Mobile m )
		{
			Item check = m.FindItemOnLayer( Layer.OneHanded );

			if ( check is BaseTool && check != tool )//&& !(check is AncientSmithyHammer) )
				return false;

			check = m.FindItemOnLayer( Layer.TwoHanded );

			if ( check is BaseTool && check != tool )//&& !(check is AncientSmithyHammer) )
				return false;

			return true;
		}

		public override void OnSingleClick( Mobile from )
		{
			DisplayDurabilityTo( from );

			base.OnSingleClick( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
            // szczaw :: 08.01.2013 :: nie mozna korzystac z narzedzi siedzac na wierzchowcu.
            if(from.Mounted)
            {
                from.SendLocalizedMessage(1071120);
                return;
            }
            else
            {
                if(IsChildOf(from.Backpack) || Parent == from)
                {
                    CraftSystem system = this.CraftSystem;

                    int num = system.CanCraft(from, this, null);

                    if(num > 0)
                    {
                        from.SendLocalizedMessage(num);
                    }
                    else
                    {
                        CraftContext context = system.GetContext(from);

                        from.SendGump(new CraftGump(from, system, this, null));
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (Mobile) m_Crafter );
			writer.Write( (int) m_Quality );

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Crafter = reader.ReadMobile();
					m_Quality = (ToolQuality) reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_UsesRemaining = reader.ReadInt();
					break;
				}
			}
		}
		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2,  BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ToolQuality)quality;

			if ( makersMark )
				Crafter = from;

			// 09.09.2013 :: mortuus - Ilosc uzyc oraz kolor narzedzi bedzie zalezec od materialu uzytego w ich produkcji
			Type resourceType = typeRes;
            if ( resourceType == null )
                resourceType = craftItem.Ressources.GetAt( 0 ).ItemType;
			
			UnscaleUses();
			m_UsesRemaining = (int) (m_UsesRemaining * BaseTool.getMaterialBonus(resourceType));
			ScaleUses();
			Hue = resHue;

			return quality;
		}

		#endregion
	}
}