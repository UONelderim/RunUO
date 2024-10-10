using System;
using System.Collections.Generic;
using Nelderim;
using Server.Commands;
using Server.Engines.BulkOrders;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public partial class Corpse
	{
		[CommandProperty( AccessLevel.GameMaster )]
		public double CampingCarved
		{
			get => CorpseExt.Get( this ).CampingCarved;
			set => CorpseExt.Get( this ).CampingCarved = value;
		}

		public List<Mobile> Hunters { get; } = new();
		public List<SmallHunterBOD> HunterBods { get; } = new();

        private DateTime lastCampingSkillcheck = DateTime.MinValue;
	}

	class CorpseExt : NExtension<CorpseExtInfo>
	{
		public static string ModuleName = "CorpseExt";

		public static void Initialize()
		{
			CommandSystem.Register("corpseInfo", AccessLevel.Counselor, ShowCorpseInfo);
			EventSink.WorldSave += new WorldSaveEventHandler( Save );
			Load( ModuleName );
		}

		public static void ShowCorpseInfo(CommandEventArgs e)
		{
			if(e.Mobile == null || e.Mobile.Deleted)
				return;
			e.Mobile.BeginTarget(12,
				false,
				TargetFlags.None,
				(from, targeted) =>
				{
					if (targeted is Corpse corpse)
					{
						if (corpse.Owner is BaseCreature bc)
						{
							from.SendMessage("Looting rights");
							foreach (var ds in BaseCreature.GetLootingRights(bc.DamageEntries, bc.HitsMax))
							{
								from.SendMessage($"{ds.m_Mobile.Name}[{ds.m_Mobile.Serial}]: Dmg:{ds.m_Damage} HasRight:{ds.m_HasRight}");
							}
							from.SendMessage("Hunters");
							corpse.Hunters.ForEach(h => from.SendMessage($"{h.Name}[{h.Serial}]"));
							from.SendMessage("HunterBods");
							corpse.HunterBods.ForEach(hb => from.SendMessage($"{hb.Serial}"));
						}
						else
						{
							from.SendMessage("Te zwloki nie maja wlasciciela");
						}

					}
					else
					{
						from.SendMessage("To nie sa zwloki");
					}
				});
		}

		public static void Save( WorldSaveEventArgs args )
		{
			Cleanup();
			Save( args, ModuleName );
		}

		private static void Cleanup()
		{
			List<Serial> toRemove = new List<Serial>();
			foreach ( KeyValuePair<Serial, CorpseExtInfo> kvp in m_ExtensionInfo )
			{
				if ( World.FindItem( kvp.Key ) == null )
					toRemove.Add( kvp.Key );
			}
			foreach ( Serial serial in toRemove )
			{
				m_ExtensionInfo.TryRemove( serial, out _ );
			}
		}
	}

	class CorpseExtInfo : NExtensionInfo
	{
		public double CampingCarved { get; set; } = -1.0;

		public override void Serialize( GenericWriter writer )
		{
			writer.Write( (int)0 ); //version
			writer.Write( CampingCarved );
		}

		public override void Deserialize( GenericReader reader )
		{
			int version = 0;
			if (Fix)
				version = reader.ReadInt(); //version  
			CampingCarved = reader.ReadDouble();
		}
	}
}
