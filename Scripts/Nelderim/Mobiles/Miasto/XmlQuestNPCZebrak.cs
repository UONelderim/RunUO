using System;
using Server.Items;

namespace Server.Mobiles
{
	public class XmlQuestNPCZebrak : TalkingBaseCreature
	{

        [Constructable]
        public XmlQuestNPCZebrak() : this(-1)
        {
        }
		private DateTime m_Spoken;
		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( m.Alive && m is PlayerMobile && !IsMuted)
			{
				PlayerMobile pm = (PlayerMobile)m;
					
				int range = 2;
				
			    if ( Utility.RandomDouble() < 0.20 )
			    {
				    if ( range >= 0 && InRange( m, range ) && !InRange( oldLocation, range ) && DateTime.Now >= m_Spoken + TimeSpan.FromSeconds( 10 ) )
				    {
                        if (Race == Tamael.Instance) {
                            switch (Utility.Random(19)) {
                                case 0: Say( "Złota daj żebrakowi..." ); break;
					            case 1: Say( "O Panocku o licu złotym, zlituj sie" ); break;
					            case 2: Say( "Psia mać..." ); break;
					            case 3: Say( "Choć okrucha chleba... błagam" ); Emote("*Składa ręce błagalnie*"); break;
					            case 4: Say( "Mam chorą córkę..." ); break;
					            case 5: Say( "Wszystko mi zabrali... Wszystko..." ); Emote("*Szlocha*"); break;
					            case 6: Say( "Czy Ty przypadkiem ode mnie nie pożyczałeś pieniędzy?" ); break;
					            case 7: Say( "Poratujcie w potrzebie..."); break;
					            case 8: Say( "Kiedyś to sie żylo..." ); Emote("*Wzdycha ze smutkiem*"); break;
					            case 9: Say( "Ta straż ciągle tu tylko węszy." ); break;
					            case 10: Say( "Za co ja dzieciaki wyżywie... Poratujcie błagam" ); break;
					            case 11: Say( "Na chleb jeno..." ); break;
					            case 12: Emote( "*Smarka w brudną chustę*" ); break;
					            case 13: Say( "Kiedyś to było..." ); Emote("Wzdycha ciężko"); break;
								case 14: Emote( "*Poprawia łachmany*" ); break;
								case 15: Say( "Było sie młodym i głupim... Tak wyszło..." ); break;
								case 16: Emote( "*Podnosi coś z ziemi*" ); Say("Ah.. jednak nie"); break;
								case 17: Say( "Cholipka..." ); Emote("*Rozgląda się powoli wzdychając*"); break;
								case 18: Say( "Wszystko co miałem dla kraju oddałem..." ); break;
                            }
                        } else if(Race == Jarling.Instance) {
                            switch (Utility.Random(19)) {
                                case 0: Say( "Złota daj żebrakowi..." ); break;
					            case 1: Say( "O Panocku o licu złotym, zlituj sie" ); break;
					            case 2: Say( "Psia mać..." ); break;
					            case 3: Say( "Choć okrucha chleba... błagam" ); Emote("*Składa ręce błagalnie*"); break;
					            case 4: Say( "Mam chorą córkę..." ); break;
					            case 5: Say( "Wszystko mi zabrali... Wszystko..." ); Emote("*Szlocha*"); break;
					            case 6: Say( "Czy Ty przypadkiem ode mnie nie pożyczałeś pieniędzy?" ); break;
					            case 7: Say( "Poratujcie w potrzebie..."); break;
					            case 8: Say( "Kiedyś to sie żylo..." ); Emote("*Wzdycha ze smutkiem*"); break;
					            case 9: Say( "Ta straż ciągle tu tylko węszy." ); break;
					            case 10: Say( "Za co ja dzieciaki wyżywie... Poratujcie błagam" ); break;
					            case 11: Say( "Na chleb jeno..." ); break;
					            case 12: Emote( "*Smarka w brudną chustę*" ); break;
					            case 13: Say( "Kiedyś to było..." ); Emote("*Wzdycha ciężko*"); break;
								case 14: Emote( "*Poprawia łachmany*" ); break;
								case 15: Say( "Było sie młodym i głupim... Tak wyszło..." ); break;
								case 16: Emote( "*Podnosi coś z ziemi*" ); Say("Ah.. jednak nie"); break;
								case 17: Say( "Cholipka..." ); Emote("*Rozgląda się powoli wzdychając*"); break;
								case 18: Say( "Aż sie tu nie chce żyć... parszywe miasto" ); break;
								
                            }
                        } else if (Race == Krasnolud.Instance) {
                            switch (Utility.Random(18)) {
                                case 0: Say( "Złota daj żebrakowi..." ); break;
					            case 1: Say( "O Panocku o licu złotym, zlituj sie" ); break;
					            case 2: Say( "Psia mać..." ); break;
					            case 3: Say( "Choć okrucha chleba... błagam" ); Emote("*Składa ręce błagalnie*"); break;
					            case 4: Say( "Mam chorą córkę..." ); break;
					            case 5: Say( "Wszystko mi zabrali... Wszystko..." ); Emote("*Szlocha*"); break;
					            case 6: Say( "Czy Ty przypadkiem ode mnie nie pożyczałeś pieniędzy?" ); break;
					            case 7: Say( "Poratujcie w potrzebie..."); break;
					            case 8: Say( "Kiedyś to sie żylo..." ); Emote("*Wzdycha ze smutkiem*"); break;
					            case 9: Say( "Ta straż ciągle tu tylko węszy." ); break;
					            case 10: Say( "Za co ja dzieciaki wyżywie... Poratujcie błagam" ); break;
					            case 11: Say( "Na chleb jeno..." ); break;
					            case 12: Emote( "*Smarka w brudną chustę*" ); break;
					            case 13: Say( "Kiedyś to było..." ); Emote("*Wzdycha ciężko*"); break;
								case 14: Emote( "*Poprawia łachmany*" ); break;
								case 15: Say( "Było sie młodym i głupim... Tak wyszło..." ); break;
								case 16: Emote( "*Podnosi coś z ziemi*" ); Say("Ah.. jednak nie"); break;
								case 17: Say( "Cholipka..." ); Emote("*Rozgląda się powoli wzdychając*"); break;
                            }
                        }
                        m_Spoken = DateTime.Now;
				    }
			    }
			}
		}		
		
        [Constructable]
        public XmlQuestNPCZebrak(int gender) : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.8, 3.0 )
        {
            SetStr( 100, 300 );
            SetDex( 100, 300 );
            SetInt( 100, 300 );

            Fame = 0;
            Karma = 300;

            CanHearGhosts = false;

            SpeechHue = Utility.RandomDyedHue();
            
            Hue = Utility.RandomSkinHue();
            
            switch(gender)
            {
                case -1: this.Female = Utility.RandomBool(); break;
                case 0: this.Female = false; break;
                case 1: this.Female = true; break;
            }

            if ( this.Female)
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName( "female" );
                Title = "- Żebraczka";
                Item hat = null;
                switch ( Utility.Random( 2 ) )//4 hats, one empty, for no hat
                {
                    /*case 0: hat = new FeatheredHat( GetRandomHue() );	    break;
                    case 1: hat = new Bonnet(GetRandomHue());			break;*/
                    case 0: hat = new SkullCap(GetRandomHue());			    break;
					/*case 3: hat = new WideBrimHat(GetRandomHue());     break;
					case 4: hat = new FloppyHat( GetRandomHue() );					break;*/
					case 1: hat = null; break;
					
                }
                AddItem( hat );

                Item pants = null;
                switch ( Utility.Random( 4 ) )
                {
                    case 0: pants = new ShortPants( GetRandomHue() );	break;
                    case 1: pants = new LongPants( GetRandomHue() );	break;
                    case 2: pants = new Skirt( GetRandomHue() );		break;
					/*case 3: pants = new ElvenPants( GetRandomHue() );		break;*/
                }
                AddItem( pants );

                Item shirt = null;
                switch ( Utility.Random( 3 ) )
                {
                    case 0: shirt = new Shirt( GetRandomHue() );		break;
                    case 1: shirt = new FancyShirt( GetRandomHue() );		break;
                    case 2: shirt = new Robe( GetRandomHue() );		break;
                    /*case 3: shirt = new FancyShirt( GetRandomHue() );	break;
                    case 4: shirt = new ElvenDarkShirt( GetRandomHue() );		break;
					case 5: shirt = new ElvenShirt( GetRandomHue() );		break;*/
                }
                AddItem( shirt );
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName( "male" );
				Title = "- Żebrak";
                Item hat = null;
                switch ( Utility.Random( 2 ) ) //6 hats, one empty, for no hat
                {
                    case 0: hat = new SkullCap( GetRandomHue() );					break;
                    /*case 0: hat = new FeatheredHat( GetRandomHue() );	    break;
                    case 1: hat = new Bonnet(GetRandomHue());			break;
                    case 2: hat = new Cap(GetRandomHue());			    break;
					case 3: hat = new WideBrimHat(GetRandomHue());     break;
					case 4: hat = new FloppyHat( GetRandomHue() );					break;*/
					case 1: hat = null; break;
                }
			
                AddItem( hat );
                Item pants = null;
                switch ( Utility.Random( 2 ) )
                {
                    case 0: pants = new ShortPants( GetRandomHue() );	break;
                    case 1: pants = new LongPants( GetRandomHue() );	break;
					//case 2: pants = new ElvenPants( GetRandomHue() );		break;
                }

                AddItem( pants );
                Item shirt = null;
                switch ( Utility.Random( 4 ) )
                {
                   // case 0: shirt = new Doublet( GetRandomHue() );		break;
                   // case 1: shirt = new Surcoat( GetRandomHue() );		break;
                   // case 2: shirt = new Tunic( GetRandomHue() );		break;
                    case 0: shirt = new FancyShirt( GetRandomHue() );	break;
                    case 1: shirt = new Shirt( GetRandomHue() );		break;
					case 2: shirt = new Robe( GetRandomHue() );		break;
					case 3: shirt = null; break;
					//case 5: shirt = new ElvenDarkShirt( GetRandomHue() );		break;
					
                }
                AddItem( shirt );
				
                /*Item hand = null;
                switch ( Utility.Random( 4 ) )
                {
                    case 0: hand = new Dagger( Utility.RandomNeutralHue() );	    break;
                    case 1: hand = new Club( Utility.RandomNeutralHue() );	break;
					case 2: hand = new ButcherKnife( Utility.RandomNeutralHue() );	break;
					case 3: hand = new AssassinSpike( Utility.RandomNeutralHue() );	break;
                }
				AddItem( hand );*/
            }

            Item feet = null;
            switch ( Utility.Random( 3 ) )
            {
                case 0: feet = new Sandals( Utility.RandomNeutralHue() );	break;
                case 1: feet = new Shoes( Utility.RandomNeutralHue() );	break;
				case 2: feet = null; break;
                //case 2: feet = new Sandals( Utility.RandomNeutralHue() );		break;
				//case 3: feet = new ThighBoots( Utility.RandomNeutralHue() );		break;
				
            }
            AddItem( feet ); 
            Container pack = new Backpack();

            pack.Movable = false;

            AddItem( pack );
        }

        public XmlQuestNPCZebrak( Serial serial ) : base( serial )
        {
        }

		

        private static int GetRandomHue()
        {
            switch ( Utility.Random( 6 ) )
            {
                default:
                case 0: return 0;
                case 1: return Utility.RandomBlueHue();
                case 2: return Utility.RandomGreenHue();
                case 3: return Utility.RandomRedHue();
                case 4: return Utility.RandomYellowHue();
                case 5: return Utility.RandomNeutralHue();
            }
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
