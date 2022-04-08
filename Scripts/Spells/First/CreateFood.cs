using System;
using Server.Items;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
 
namespace Server.Spells.First
{
    // szczaw :: 09.01.2013 :: mozliwy wybor jedzenia z gumpa i ogolna refaktoryzacja
    // szczaw :: 11.01.2013 :: refaktoryzacja
	public class CreateFoodSpell : MagerySpell
	{
		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 4.0 ); } }

		private static SpellInfo m_Info = new SpellInfo(
				"Create Food", "In Mani Ylem",
				224,
				9011,
				Reagent.Garlic,
				Reagent.Ginseng,
				Reagent.MandrakeRoot
			);

        private readonly Type _foodKind;

        private static readonly IList<Type> _food = new Type[]
			{
				typeof(Ham),
				typeof(Ribs),
				typeof(CookedBird),
				typeof(Sausage),
				typeof(FishSteak),
				typeof(CheeseWedge),
				typeof(Grapes),
				typeof(Apple),
				typeof(Peach),
				typeof(Muffins),
            };

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public CreateFoodSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
            _foodKind = null;
		}

        public CreateFoodSpell( Mobile caster, Item scroll, Type choosenFoodKind )
            : base(caster, scroll, m_Info)
        {
            _foodKind = choosenFoodKind;
        }

        public override bool CheckCast()
        {
            if(_foodKind == null)
            {
                if(!Caster.HasGump(typeof(GeneralItemGump)))
                {
                    Action<NetState, RelayInfo> action = (s, i) => new CreateFoodSpell(this.Caster, this.Scroll, _food[Utility.Clamp(i.ButtonID,0,_food.Count -1)]).Cast();
                    var gump = new GeneralItemGump( _food, action);
                   
                    gump.ItemPosition = new Point2D(17, 23);
                    Caster.SendGump(gump);
                }
                
                return false;
            }
            else
                return true;
        }

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
                Item food = (Item) Activator.CreateInstance(_foodKind);

				if ( food != null )
				{
					food.LootType = LootType.Cursed;
					Caster.AddToBackpack( food );

					// You magically create food in your backpack:
					Caster.SendLocalizedMessage( 1042695 );
                    Caster.SendLocalizedMessage(food.LabelNumber);

					Caster.FixedParticles( 0, 10, 5, 2003, EffectLayer.RightHand );
					Caster.PlaySound( 0x1E2 );
				}
			}

			FinishSequence();
		}
	}
}