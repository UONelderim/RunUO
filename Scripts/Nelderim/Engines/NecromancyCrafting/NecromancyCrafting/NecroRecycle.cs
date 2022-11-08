using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Craft
{
    public class NecroRecycle : RecycleHelper
    {
        public NecroRecycle()
		{
		}

        public override int Label { get { return 1044259; } }

        public override void Do(Mobile from, CraftSystem craftSystem, BaseTool tool)
		{
			int num = craftSystem.CanCraft( from, tool, null );

			if ( num > 0 )
			{
				from.SendGump( new CraftGump( from, craftSystem, tool, num ) );
			}
			else
			{
				from.Target = new InternalTarget( craftSystem, tool );
				from.SendLocalizedMessage( 1044273 ); // Target an item to recycle.
			}
		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
			}

			private bool Disassemble( Mobile from, BaseCreature bc)
			{
				try
				{
					Type crystalType = ScriptCompiler.FindTypeByName(bc.GetType().Name + "Crystal");
					if (crystalType != null && crystalType.IsSubclassOf(typeof(BaseNecroCraftCrystal)))
					{
						List<Item> resources = new List<Item>();
						BaseNecroCraftCrystal crystal = (BaseNecroCraftCrystal)Activator.CreateInstance(crystalType);
						resources.Add(crystal);
						
						foreach (Type bodyPartType in crystal.RequiredBodyParts)
						{
							Item bodyPart = (Item)Activator.CreateInstance(bodyPartType);
							resources.Add(bodyPart);
						}

						foreach (Item resource in resources)
						{
							if (!from.AddToBackpack(resource))
							{
								from.SendMessage("Jeden z materiałów nie zmieścił się do plecaka");
							}
						}

						bc.Delete();
						
						from.PlaySound( 0x2A );
						from.PlaySound( 0x240 );
						return true;
					}
				}
				catch
				{
                }

                return false;
			}

            protected override void OnTarget( Mobile from, object targeted )
			{
				int num = m_CraftSystem.CanCraft( from, m_Tool, null );

				if ( num > 0 )
				{
					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, num ) );
				}
				else
				{
					bool success = false;
					bool isStoreBought = false;

					if ( targeted is BaseCreature )
					{
						success = Disassemble( from, (BaseCreature)targeted);
					}

					if ( success )
						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, isStoreBought ? 500418 : 1044270 ) ); // You melt the item down into ingots.
                    else
                        from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1044272 ) ); // You can't melt that down into ingots.
				}
			}
		}
    }
}