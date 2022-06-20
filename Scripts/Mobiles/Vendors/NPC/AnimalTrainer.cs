using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System.Text.RegularExpressions;

namespace Server.Mobiles
{
    public class StableEntry : ContextMenuEntry
    {
        private BaseVendor m_Trainer;
        private Mobile m_From;

        public StableEntry(BaseVendor trainer, Mobile from)
            : base(6126, 12)
        {
            m_Trainer = trainer;
            m_From = from;
        }

        public override void OnClick()
        {
            if (!Owner.From.CheckAlive())
                return;

            if (m_Trainer.CheckVendorAccess(m_From))
                AnimalTrainer.BeginStable(m_From, m_Trainer);
        }
    }

    public class PetShrinkEntry : ContextMenuEntry
    {
        private BaseVendor m_Trainer;
        private Mobile m_From;

        public PetShrinkEntry(BaseVendor trainer, Mobile from)
            : base(6065, 12)
        {
            m_Trainer = trainer;
            m_From = from;
        }

        public override void OnClick()
        {
            if (!Owner.From.CheckAlive())
                return;

            if (m_Trainer.CheckVendorAccess(m_From))
            {
                m_From.SendLocalizedMessage(1070058);
                m_From.Target = new ShrinkTarget(m_Trainer);
            }
        }
    }

    public class ShrinkTarget : Target
    {
        private BaseVendor m_Trainer;

        public ShrinkTarget(BaseVendor trainer) : base(12, false, TargetFlags.None)
        {
            m_Trainer = trainer;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            ShrunkPet.Shrink(m_Trainer, from as PlayerMobile, targeted as BaseCreature);
        }
    }

    public class ClaimListGump : Gump
    {
        private BaseVendor m_Trainer;
        private Mobile m_From;
        private List<BaseCreature> m_List;

        public ClaimListGump(BaseVendor trainer, Mobile from, List<BaseCreature> list)
            : base(50, 50)
        {
            m_Trainer = trainer;
            m_From = from;
            m_List = list;

            from.CloseGump(typeof(ClaimListGump));

            AddPage(0);

            AddBackground(0, 0, 325, 50 + (list.Count * 20), 9250);
            AddAlphaRegion(5, 5, 315, 40 + (list.Count * 20));

            AddHtml(15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Wybierz zwierze, które chcesz wyciągnąć ze stajni:</BASEFONT>", false, false);

            for (int i = 0; i < list.Count; ++i)
            {
                BaseCreature pet = list[i];

                if (pet == null || pet.Deleted)
                    continue;

                AddButton(15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0);
                AddHtml(32, 35 + (i * 20), 275, 18, String.Format("<BASEFONT COLOR=#C0C0EE>{0}</BASEFONT>", pet.Name), false, false);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index >= 0 && index < m_List.Count)
                AnimalTrainer.EndClaimList(m_From, m_List[index], m_Trainer);
        }
    }

    public class ClaimAllEntry : ContextMenuEntry
    {
        private BaseVendor m_Trainer;
        private Mobile m_From;

        public ClaimAllEntry(BaseVendor trainer, Mobile from)
            : base(6127, 12)
        {
            m_Trainer = trainer;
            m_From = from;
        }

        public override void OnClick()
        {
            if (!Owner.From.CheckAlive())
                return;

            if (m_Trainer.CheckVendorAccess(m_From))
                AnimalTrainer.Claim(m_From, m_Trainer);
        }
    }

    public class StableTarget : Target
    {
        private BaseVendor m_Trainer;

        public StableTarget(BaseVendor trainer) : base(12, false, TargetFlags.None)
        {
            m_Trainer = trainer;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!m_Trainer.IsAssignedBuildingWorking())
            {
                m_Trainer.SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
            }
            else if (targeted is Mobile && m_Trainer.GetDistanceToSqrt((Mobile)targeted) > 7)
                from.SendLocalizedMessage(500446); // That is too far away.
            else if (targeted is BaseCreature)
                AnimalTrainer.EndStable(from, (BaseCreature)targeted, m_Trainer);
            else if (targeted == from)
                m_Trainer.SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
            else
                m_Trainer.SayTo(from, 1048053); // You can't stable that!
        }
    }
    
    public class AnimalTrainer : BaseVendor
	{
        public override bool CanTeach{ get{ return false; } }

        private class AnimalPrice
        {
            private Type m_Animal;
            private int m_Price;
            public AnimalPrice(int price, Type animal)
            {
                m_Price = price;
                m_Animal = animal;
            }
            public Type Animal{ get{ return m_Animal; } set{ m_Animal = value; } }
            public int Price{ get{ return m_Price; } set{ m_Price = value; } }
        }

        #region ceny zwierzat
        private static List<AnimalPrice> m_AnimalPrices = new List<AnimalPrice>
        {
		    new AnimalPrice( 5000, typeof( GreaterDragon ) ),
		    new AnimalPrice( 4500, typeof( CuSidhe ) ),
            new AnimalPrice( 4500, typeof( worg ) ),
            new AnimalPrice( 4000, typeof( DiamondDragon) ),
            new AnimalPrice( 4000, typeof( RubyDragon) ),
            new AnimalPrice( 4000, typeof( EmeraldDragon) ),
            new AnimalPrice( 4000, typeof( SapphireDragon) ),
            new AnimalPrice( 5000, typeof( feniks) ),
            new AnimalPrice( 4500, typeof( MrocznaCzelusc ) ),
		    new AnimalPrice( 4400, typeof( Reptalon ) ),
		    new AnimalPrice( 4300, typeof( Hiryu ) ),
		    new AnimalPrice( 3900, typeof( RuneBeetle ) ),
		    new AnimalPrice( 3850, typeof( WhiteWyrm ) ),
            new AnimalPrice( 3850, typeof( PomiotPajaka) ),
		    new AnimalPrice( 3800, typeof( LodowySmok ) ),
		    new AnimalPrice( 3800, typeof( OgnistySmok ) ),
		    new AnimalPrice( 3750, typeof( PomiotPajaka ) ),
		    new AnimalPrice( 3700, typeof( Dragon ) ),
		    new AnimalPrice( 3650, typeof( NChimera ) ),
		    new AnimalPrice( 2100, typeof( BakeKitsune ) ),
		    new AnimalPrice( 2850, typeof( LesserHiryu ) ),
		    new AnimalPrice( 2900, typeof( Nightmare ) ),
		    new AnimalPrice( 2950, typeof( Unicorn ) ),
		    new AnimalPrice( 3000, typeof( Kirin ) ),
		    new AnimalPrice( 2000, typeof( FireBeetle ) ),
		    new AnimalPrice( 1000, typeof( ScaledSwampDragon ) ),
		    new AnimalPrice( 1000, typeof( SwampDragon ) ),
		    new AnimalPrice( 970, typeof( HellHorse ) ),
		    new AnimalPrice( 940, typeof( WarHorseA ) ),
		    new AnimalPrice( 940, typeof( WarHorseB ) ),
		    new AnimalPrice( 940, typeof( WarHorseC ) ),
		    new AnimalPrice( 940, typeof( WarHorseD ) ),
		    new AnimalPrice( 940, typeof( WarHorseE ) ),
		    new AnimalPrice( 910, typeof( HellHound ) ),
		    new AnimalPrice( 2000, typeof( FireSteed ) ),
		    new AnimalPrice( 850, typeof( SilverSteed ) ),
		    new AnimalPrice( 820, typeof( RedBeetle ) ),
		    new AnimalPrice( 500, typeof( Beetle ) ),
		    new AnimalPrice( 470, typeof( Drake ) ),
            new AnimalPrice( 470, typeof( DiamondDrake ) ),
            new AnimalPrice( 470, typeof( RubyDrake ) ),
            new AnimalPrice( 470, typeof( EmeraldDrake ) ),
            new AnimalPrice( 470, typeof( SapphireDrake ) ),
		    new AnimalPrice( 440, typeof( FrenziedOstard ) ),
		    new AnimalPrice( 410, typeof( ForestOstard ) ),
		    new AnimalPrice( 410, typeof( DesertOstard ) ),
		    new AnimalPrice( 380, typeof( PRidgeback ) ),
		    new AnimalPrice( 380, typeof( Ridgeback ) ),
		    new AnimalPrice( 380, typeof( SavageRidgeback ) ),
		    new AnimalPrice( 350, typeof( Horse ) ),
		    new AnimalPrice( 350, typeof( RidableLlama ) ),
		    new AnimalPrice( 320, typeof( PackLlama ) ),
		    new AnimalPrice( 320, typeof( PackHorse ) ),
		    new AnimalPrice( 290, typeof( DeathwatchBeetle ) ),
		    new AnimalPrice( 260, typeof( Imp ) ),
		    new AnimalPrice( 230, typeof( GiantSpider ) ),
		    new AnimalPrice( 220, typeof( FrostSpider ) ),
		    new AnimalPrice( 210, typeof( Scorpion ) ),
		    new AnimalPrice( 200, typeof( LavaLizard ) ),
		    new AnimalPrice( 190, typeof( Snake ) ),
		    new AnimalPrice( 130, typeof( PredatorHellCat ) ),
		    new AnimalPrice( 130, typeof( HellCat ) ),
		    new AnimalPrice( 125, typeof( StrongMongbat ) ),
		    new AnimalPrice( 120, typeof( Mongbat ) ),
		    new AnimalPrice( 115, typeof( SkitteringHopper ) ),
		    new AnimalPrice( 110, typeof( CorrosiveSlime ) ),
		    new AnimalPrice( 105, typeof( GiantIceWorm ) ),
		    new AnimalPrice( 100, typeof( Slime ) ),
		    new AnimalPrice( 95, typeof( GiantToad ) ),
		    new AnimalPrice( 90, typeof( BullFrog ) ),
		    new AnimalPrice( 85, typeof( Gaman ) ),
		    new AnimalPrice( 80, typeof( SnowLeopard ) ),
		    new AnimalPrice( 80, typeof( Cougar ) ),
		    new AnimalPrice( 80, typeof( Panther ) ),
		    new AnimalPrice( 75, typeof( Alligator ) ),
		    new AnimalPrice( 70, typeof( PolarBear ) ),
		    new AnimalPrice( 70, typeof( GrizzlyBear ) ),
		    new AnimalPrice( 65, typeof( BlackBear ) ),
		    new AnimalPrice( 65, typeof( BrownBear ) ),
		    new AnimalPrice( 60, typeof( GreatHart ) ),
		    new AnimalPrice( 55, typeof( Hind ) ),
		    new AnimalPrice( 50, typeof( Bull ) ),
		    new AnimalPrice( 45, typeof( Cow ) ),
		    new AnimalPrice( 40, typeof( WhiteWolf ) ),
		    new AnimalPrice( 40, typeof( DireWolf ) ),
		    new AnimalPrice( 40, typeof( GreyWolf ) ),
		    new AnimalPrice( 40, typeof( TimberWolf ) ),
		    new AnimalPrice( 35, typeof( Walrus ) ),
		    new AnimalPrice( 35, typeof( Sheep ) ),
		    new AnimalPrice( 35, typeof( Llama ) ),
		    new AnimalPrice( 35, typeof( Boar ) ),
		    new AnimalPrice( 35, typeof( Pig ) ),
		    new AnimalPrice( 35, typeof( Goat ) ),
		    new AnimalPrice( 30, typeof( Gorilla ) ),
		    new AnimalPrice( 30, typeof( Squirrel ) ),
		    new AnimalPrice( 30, typeof( Ferret ) ),
		    new AnimalPrice( 30, typeof( JackRabbit ) ),
		    new AnimalPrice( 30, typeof( Rabbit ) ),
		    new AnimalPrice( 30, typeof( Eagle ) ),
		    new AnimalPrice( 30, typeof( Parrot ) ),
		    new AnimalPrice( 25, typeof( Dog ) ),
		    new AnimalPrice( 22, typeof( Cat ) ),
		    new AnimalPrice( 18, typeof( Bird ) ),
		    new AnimalPrice( 18, typeof( Chicken ) ),
		    new AnimalPrice( 15, typeof( GiantRat ) ),
		    new AnimalPrice( 14, typeof( Sewerrat ) ),
		    new AnimalPrice( 10, typeof( Rat ) )
        };
        #endregion

        private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public AnimalTrainer() : base( "- treser zwierzat" )
		{
			SetSkill( SkillName.AnimalLore, 64.0, 100.0 );
			SetSkill( SkillName.AnimalTaming, 90.0, 100.0 );
			SetSkill( SkillName.Veterinary, 65.0, 88.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBAnimalTrainer() );
		}

        public override bool CheckVendorAccess(Mobile from)
        {
            return checkWillingness(from, true);
        }

		public override VendorShoeType ShoeType
		{
			get{ return Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( Utility.RandomBool() ? (Item)new QuarterStaff() : (Item)new ShepherdsCrook() );
		}

		public override bool IsActiveBuyer { get{ return false; } }


        private class PetSellfEntry : ContextMenuEntry
		{
			private AnimalTrainer m_Trainer;
			private Mobile m_From;

			public PetSellfEntry( AnimalTrainer trainer, Mobile from ) : base( 6104, 12 )
			{
				m_Trainer = trainer;
				m_From = from;
			}

			public override void OnClick()
			{
				if (!Owner.From.CheckAlive())
					return;

				if (m_Trainer.CheckVendorAccess(m_From))
				    m_Trainer.BeginPetSale( m_From );
			}
		}

        private class PetSaleTarget : Target
        {
            private AnimalTrainer m_Trainer;

            public PetSaleTarget( AnimalTrainer trainer ) : base( 12, false, TargetFlags.None )
            {
                m_Trainer = trainer;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if (!m_Trainer.IsAssignedBuildingWorking())
                {
                    m_Trainer.SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
                }
                else if (targeted is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature) targeted;
                    AnimalPrice found = m_AnimalPrices.Find(x => x.Animal == bc.GetType());

                    if (found == null)
                        m_Trainer.SayTo(from, "Nie kupuje tego typu zwierzat."); // You can't PetSale that!
                    else
                        from.SendGump(new SellPetConfirmation(m_Trainer, from, bc, found.Price));
                }
                else if (targeted == from)
                    m_Trainer.SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
            }
        }

        private class SellPetConfirmation : Gump
        {
            private AnimalTrainer m_Trainer;
            private Mobile m_From;
            private BaseCreature m_creatureToSell;
            private const int LabelColor = 0x7FFF;

            public SellPetConfirmation(AnimalTrainer trainer, Mobile from, BaseCreature creatureToSell, int price)
                : base(50, 50)
            {
                m_Trainer = trainer;
                m_From = from;
                m_creatureToSell = creatureToSell;

                from.CloseGump(typeof(SellPetConfirmation));

                AddPage(0);

                AddBackground(0, 0, 300, 130, 5054);
                AddHtmlLocalized(10, 10, 250, 50, 1031840, string.Format("{0}", creatureToSell.Name.ToString()), LabelColor, false, false); // Czy na pewno chcesz sprzedac zwierze o imieniu ~1_VALUE~?
                AddHtmlLocalized(10, 50, 250, 25, 1031841, string.Format("{0}", price.ToString()), LabelColor, false, false); // Cena wynosi ~1_VALUE~ sztuk zlota.

                AddButton(50, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(85, 100, 240, 20, 1063803, LabelColor, false, false); // Tak
                AddButton(150, 100, 4005, 4007, -1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(185, 100, 240, 20, 1063804, LabelColor, false, false); // Nie
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID;

                if (index == 1)
                    m_Trainer.EndPetSale(m_From, m_creatureToSell);
            }
        }

        public void BeginPetSale( Mobile from )
        {
            if ( Deleted || !from.CheckAlive() )
                return;

            SayTo( from, "Ktore stworzenie chcesz sprzedac?" );

            from.Target = new PetSaleTarget( this );

        }

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
			{
				list.Add(new StableEntry( this, from ));

                list.Add(new PetSellfEntry(this, from));

                list.Add(new PetShrinkEntry(this, from));

				if ( from.Stabled.Count > 0 )
					list.Add(new ClaimAllEntry( this, from ));
				
                /*
                if (IsAssignedBuildingWorking())
                {
                    // Menu -> Message + Target -> Shrink
                    Action<Mobile, object> action = (m, o) => ShrunkPet.Shrink(this, m as PlayerMobile, o as BaseCreature);
                    var target = new GeneralTarget(3, false, TargetFlags.None, action);
                    var shrink = new GeneralContextMenuEntry(6065, () =>
                        {
                            from.SendLocalizedMessage(1070058);
                            from.Target = target;
                        });

                    list.Add(shrink);
                }
                */
			}

			base.AddCustomContextEntries( from, list );
		}

		public static int GetMaxStabled( Mobile from )
		{
			double taming = from.Skills[SkillName.AnimalTaming].Value;
			double anlore = from.Skills[SkillName.AnimalLore].Value;
			double vetern = from.Skills[SkillName.Veterinary].Value;
			double sklsum = taming + anlore + vetern;

			int max;

			if ( sklsum >= 240.0 )
				max = 7;
			else if ( sklsum >= 200.0 )
				max = 6;
			else if ( sklsum >= 160.0 )
				max = 5;
			else
				max = 5;

			if ( taming >= 100.0 )
				max += (int)((taming - 90.0) / 10);

			if ( anlore >= 100.0 )
				max += (int)((anlore - 90.0) / 10);

			if ( vetern >= 100.0 )
				max += (int)((vetern - 90.0) / 10);

            max++; // jeszcze jeden slot gratis dla kazdego :)

			return max;
		}

		public static void BeginClaimList( Mobile from, BaseVendor trainer )
		{
            if (trainer.Deleted || !from.CheckAlive())
				return;

			List<BaseCreature> list = new List<BaseCreature>();

            if (!trainer.IsAssignedBuildingWorking())
            {
                trainer.SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
            }
            else
            {
                for (int i = 0; i < from.Stabled.Count; ++i)
                {
                    BaseCreature pet = from.Stabled[i] as BaseCreature;

                    if (pet == null || pet.Deleted)
                    {
                        pet.IsStabled = false;
                        from.Stabled.RemoveAt(i);
                        --i;
                        continue;
                    }

                    list.Add(pet);
                }

                if (list.Count > 0)
                    from.SendGump(new ClaimListGump(trainer, from, list));
                else
                    trainer.SayTo(from, 502671); // But I have no animals stabled with me at the moment!
            }
		}

		public static void EndClaimList( Mobile from, BaseCreature pet, BaseVendor trainer )
		{
			if ( pet == null || pet.Deleted || from.Map != trainer.Map || !from.InRange( trainer, 14 ) || !from.Stabled.Contains( pet ) || !from.CheckAlive() )
				return;

            if (!trainer.IsAssignedBuildingWorking())
            {
                trainer.SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
            }
			else if ( (from.Followers + pet.ControlSlots) <= from.FollowersMax )
			{
				pet.SetControlMaster( from );

				if ( pet.Summoned )
					pet.SummonMaster = from;

				pet.ControlTarget = from;
				pet.ControlOrder = OrderType.Follow;

				pet.MoveToWorld( from.Location, from.Map );

				pet.IsStabled = false;
				from.Stabled.Remove( pet );

                trainer.SayTo(from, 1042559); // Here you go... and good day to you!
			}
			else
			{
                trainer.SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
			}
		}

		public static void BeginStable( Mobile from, BaseVendor trainer )
		{
			if ( trainer.Deleted || !from.CheckAlive() )
				return;

            if (!trainer.IsAssignedBuildingWorking())
            {
                trainer.SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
            }
			else if ( from.Stabled.Count >= GetMaxStabled( from ) )
			{
                trainer.SayTo(from, 1042565); // You have too many pets in the stables!
			}
			else
			{
                trainer.SayTo(from, 1042558); /* I charge 30 gold per pet for a real week's stable time.
										 * I will withdraw it from thy bank account.
										 * Which animal wouldst thou like to stable here?
										 */

				from.Target = new StableTarget( trainer );
			}
		}

		public static void EndStable( Mobile from, BaseCreature pet, BaseVendor trainer )
		{
			if ( trainer.Deleted || !from.CheckAlive() )
				return;

            if (!trainer.IsAssignedBuildingWorking())
            {
                trainer.SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
            }
			else if ( !pet.Controlled || pet.ControlMaster != from )
			{
                trainer.SayTo(from, 1042562); // You do not own that pet!
			}
			else if ( pet.IsDeadPet )
			{
                trainer.SayTo(from, 1049668); // Living pets only, please.
			}
			else if ( pet.Summoned )
			{
                trainer.SayTo(from, 502673); // I can not stable summoned creatures.
			}
            else if (pet.Allured)
            {
                trainer.SayTo(from, 1048053); // You can't stable that!
            }
			else if ( pet.Body.IsHuman )
			{
                trainer.SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
			}
			else if ( (pet is PackLlama || pet is PackHorse || pet is Beetle) && (pet.Backpack != null && pet.Backpack.Items.Count > 0) )
			{
                trainer.SayTo(from, 1042563); // You need to unload your pet.
			}
			else if ( pet.Combatant != null && pet.InRange( pet.Combatant, 12 ) && pet.Map == pet.Combatant.Map )
			{
                trainer.SayTo(from, 1042564); // I'm sorry.  Your pet seems to be busy.
			}
			else if ( from.Stabled.Count >= GetMaxStabled( from ) )
			{
                trainer.SayTo(from, 1042565); // You have too many pets in the stables!
			}
			else
			{
				Container bank = from.FindBankNoCreate();

				if ( bank != null && bank.ConsumeTotal( typeof( Gold ), 30 ) )
				{
					pet.ControlTarget = null;
					pet.ControlOrder = OrderType.Stay;
					pet.Internalize();

					pet.SetControlMaster( null );
					pet.SummonMaster = null;

					pet.IsStabled = true;

					if ( Core.SE )	
						pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

					from.Stabled.Add( pet );

                    trainer.SayTo(from, 502679); // Very well, thy pet is stabled. Thou mayst recover it by saying 'claim' to me. In one real world week, I shall sell it off if it is not claimed!
				}
				else
				{
                    trainer.SayTo(from, 502677); // But thou hast not the funds in thy bank account!
				}
			}
		}

		public static void Claim( Mobile from, BaseVendor trainer )
		{
            if (trainer.Deleted || !from.CheckAlive())
				return;

			bool claimed = false;
			int stabled = 0;

            if (!trainer.IsAssignedBuildingWorking())
            {
                trainer.SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
            }
            else
            {
                for (int i = 0; i < from.Stabled.Count; ++i)
                {
                    BaseCreature pet = from.Stabled[i] as BaseCreature;

                    if (pet == null || pet.Deleted)
                    {
                        pet.IsStabled = false;
                        from.Stabled.RemoveAt(i);
                        --i;
                        continue;
                    }

                    ++stabled;

                    if ((from.Followers + pet.ControlSlots) <= from.FollowersMax)
                    {
                        pet.SetControlMaster(from);

                        if (pet.Summoned)
                            pet.SummonMaster = from;

                        pet.ControlTarget = from;
                        pet.ControlOrder = OrderType.Follow;

                        pet.MoveToWorld(from.Location, from.Map);

                        pet.IsStabled = false;

                        if (Core.SE)
                            pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

                        from.Stabled.RemoveAt(i);
                        --i;

                        claimed = true;
                    }
                    else
                    {
                        trainer.SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
                    }
                }

                if (claimed)
                    trainer.SayTo(from, 1042559); // Here you go... and good day to you!
                else if (stabled == 0)
                    trainer.SayTo(from, 502671); // But I have no animals stabled with me at the moment!
            }
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			return true;
		}

        public override void OnSpeech( SpeechEventArgs e )
        {
            base.OnSpeech( e );

            if (e.Handled || !e.Mobile.InRange(this, 3))
                return;
            
            if (!checkAccess(e.Mobile))
                return;

            int[] keywords = e.Keywords;
            string speech = e.Speech.ToLower();

            if (Regex.IsMatch(e.Speech, "opiek", RegexOptions.IgnoreCase) && checkWillingness(e.Mobile, true))
            {
                e.Handled = true;
                if (e.Speech.ToLower() == "opiek" || Regex.IsMatch(e.Speech, "^opiek..?$", RegexOptions.IgnoreCase)) OnLazySpeech();
                else AnimalTrainer.BeginStable(e.Mobile, this);
            }
            else if (Regex.IsMatch(e.Speech, "oddaj", RegexOptions.IgnoreCase) && Regex.IsMatch(e.Speech, "wszyst", RegexOptions.IgnoreCase) && checkWillingness(e.Mobile, true))
            {
                e.Handled = true;
                AnimalTrainer.Claim(e.Mobile, this);
            }
            else if (Regex.IsMatch(e.Speech, "oddaj", RegexOptions.IgnoreCase) && checkWillingness(e.Mobile, true))
            {
                e.Handled = true;
                if (e.Speech.ToLower() == "oddaj") OnLazySpeech();
                else AnimalTrainer.BeginClaimList(e.Mobile, this);
            }
            else if (Regex.IsMatch(e.Speech, "sprzeda", RegexOptions.IgnoreCase) && checkWillingness(e.Mobile, true))
            {
                e.Handled = true;
                if (e.Speech.ToLower() == "sprzeda" || Regex.IsMatch(e.Speech, "^sprzeda.$", RegexOptions.IgnoreCase))OnLazySpeech();
                else BeginPetSale(e.Mobile);
            }
            else if (Regex.IsMatch(e.Speech, "zmniejsz", RegexOptions.IgnoreCase) && checkWillingness(e.Mobile, true))
            {
                e.Handled = true;
                if (e.Speech.ToLower() == "zmniejsz") OnLazySpeech();
                else e.Mobile.Target = new ShrinkTarget(this);
            }
        }

	    private void SellPetForGold(Mobile from, BaseCreature pet, int goldamount)
	    {
		    Item gold = new Gold(goldamount);
		    pet.ControlTarget = null; 
		    pet.ControlOrder = OrderType.None; 
		    pet.Internalize(); 
		    pet.SetControlMaster( null ); 
		    pet.SummonMaster = null;
		    pet.Delete();
		
		    Container backpack = from.Backpack;
		    if ( backpack == null || !backpack.TryDropItem( from, gold, false ) ) 
		    { 
			    gold.MoveToWorld( from.Location, from.Map );
		    }
            SayTo( from, "Dziekuje ci za to zwierze!" );

	    }

        public void EndPetSale( Mobile from, BaseCreature pet ) 
        { 
            if ( Deleted || !from.CheckAlive() ) 
                return;

            if (!IsAssignedBuildingWorking())
            {
                SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
            }
	        else if ( !pet.Controlled || pet.ControlMaster != from ) 
		        SayTo( from, 1042562 ); // You do not own that pet! 
	        else if ( pet.IsDeadPet ) 
		        SayTo( from, 1049668 ); // Living pets only, please. 
	        else if ( pet.Summoned ) 
		        SayTo( from, 502673 ); // I can not PetSale summoned creatures. 
	        else if ( pet.Body.IsHuman ) 
		        SayTo( from, 502672 ); // HA HA HA! Sorry, I am not an inn. 
	        else if ( (pet is PackLlama || pet is PackHorse || pet is Beetle) && (pet.Backpack != null && pet.Backpack.Items.Count > 0) ) 
		        SayTo( from, 1042563 ); // You need to unload your pet. 
	        else if ( pet.Combatant != null && pet.InRange( pet.Combatant, 12 ) && pet.Map == pet.Combatant.Map ) 
                SayTo( from, 1042564 ); // I'm sorry.  Your pet seems to be busy. 
	        else 
	        { 
                AnimalPrice found = m_AnimalPrices.Find(x => x.Animal == pet.GetType());

                if ( found == null )
                    SayTo( from, "Nie kupuje tego typu zwierzat." ); // You can't PetSale that!
                else
                    SellPetForGold( from, pet, found.Price );
            }
        }

		public AnimalTrainer( Serial serial ) : base( serial )
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