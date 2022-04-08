using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.ACC.CSS.Systems.Avatar;
using Server.ACC.CSS.Systems.Ancient;
using Server.ACC.CSS.Systems.Bard;
using Server.ACC.CSS.Systems.Cleric;
using Server.ACC.CSS.Systems.Druid;
using Server.ACC.CSS.Systems.Ranger;
using Server.ACC.CSS.Systems.Undead;
using Server.ACC.CSS.Systems.Rogue;

namespace Server
{
    public class LootPack
    {
        public static RandomInRange RandomizerForItemLevel( int lvl )
        {
            switch(lvl)
            {
                default: return RandomMinMaxLvl1;
                case 1: return RandomMinMaxLvl1;
                case 2: return RandomMinMaxLvl2;
                case 3: return RandomMinMaxLvl3;
                case 4: return RandomMinMaxLvl4;
                case 5: return RandomMinMaxLvl5;
                case 6: return RandomMinMaxLvl6;
            }
        }

		// Funkcje RandomMinMaxLvl#(min, max) - definiuja rozklad szansy na intensywnosc propsa, inny rozklad dla roznych leveli itemu
        public static int RandomMinMaxLvl1(int min, int max)
        {
            double rnd = Utility.RandomDouble();
			rnd = Math.Pow( rnd, 1.85 );
            return min + (int)Math.Round( (max-min) * rnd );
        }

        public static int RandomMinMaxLvl2(int min, int max)
        {
            double rnd = Utility.RandomDouble();
			rnd = Math.Pow( rnd, 1.60 );
            return min + (int)Math.Round( (max-min) * rnd );
        }

        public static int RandomMinMaxLvl3(int min, int max)
        {
            double rnd = Utility.RandomDouble();
			rnd = Math.Pow( rnd, 1.35 );
            return min + (int)Math.Round( (max-min) * rnd );
        }

        public static int RandomMinMaxLvl4(int min, int max)
        {
            double rnd = Utility.RandomDouble();
			rnd = Math.Pow( rnd, 1.15 );
			//rnd = 1 - Math.Pow( 1 - rnd, 1.25 );
            return min + (int)Math.Round( (max-min) * rnd );
        }

        public static int RandomMinMaxLvl5(int min, int max)
        {
            double rnd = Utility.RandomDouble();
			//rnd = Math.Pow( rnd, 1.8 );
			//rnd = 1 - Math.Pow( 1 - rnd, 1.6 );
            return min + (int)Math.Round( (max-min) * rnd );
        }

        public static int RandomMinMaxLvl6(int min, int max)
        {
            double rnd = Utility.RandomDouble();
			rnd = 1 - Math.Pow( 1 - rnd, 1.1 );
            return min + (int)Math.Round( (max-min) * rnd );
        }

        // 11.10.2012 :: zombie :: maksymalna liczba propsow na itemie
        public static int MaxProps = 5;
        // zombie

        public static int MaxLuckChance { get{ return GetLuckChance(1200); } }

        public static int GetLuckChance( int luck )
        {
            if ( !Core.AOS )
                return 0;

            return (int)(Math.Pow( luck, 1 / 1.8 ) * 100);
        }

		// 20.08.2014 mortuus: zmiana sposobu wybierania postaci, ktorych luck wplywa na loot:
		//                     wybierana jest postac o najwyzszym luck sposrod wszystkich, ktore zadaly
		//                     przynajmniej polowe tego dmg, ktory zadala postac o najwyzszym dmg
        public static int GetLuckChanceForKiller( Mobile dead )
        {
            List<DamageStore> list = BaseCreature.GetDamageList( dead.DamageEntries );

            int highestDmg = 0;

            for ( int i = 0; i < list.Count; ++i )
            {
                DamageStore ds = list[i];

                if (/*ds.m_HasRight &&*/ ds.m_Damage > highestDmg)
                    highestDmg = ds.m_Damage;
            }

            double minDmgReq = 0.5 * (double)highestDmg;

			int highestLuck = 0;
			for ( int i = 0; i < list.Count; ++i )
			{
				DamageStore ds = list[i];

				if ( ds.m_Damage >= minDmgReq )
				{
					Mobile killer = ds.m_Mobile;
					int luck = killer.Luck;

					PlayerMobile pmKiller = killer as PlayerMobile;
					if( pmKiller != null && pmKiller.SentHonorContext != null && pmKiller.SentHonorContext.Target == dead )
						luck += pmKiller.SentHonorContext.PerfectionLuckBonus;

					if ( luck < 0 )
						return 0;

					if ( !Core.SE && luck > 1200 )
						luck = 1200;

					if( luck > highestLuck )
					{
						highestLuck = luck;
					}
				}
			}

            return GetLuckChance( highestLuck );
        }

        public static bool CheckLuck( int chance )
        {
            return ( chance > Utility.Random( 10000 ) );
        }

        private LootPackEntry[] m_Entries;

        public LootPack( LootPackEntry[] entries )
        {
            m_Entries = entries;
        }

        public void Generate( Mobile from, Container cont, bool spawning, int luckChance )
        {
            if ( cont == null )
                return;

            /*bool checkLuck = Core.AOS;*/

            for ( int i = 0; i < m_Entries.Length; ++i )
            {
                LootPackEntry entry = m_Entries[i];

                bool shouldAdd = ( entry.Chance > Utility.Random( 10000 ) );

                if ( !shouldAdd /*&& checkLuck*/ )
                {
                    /*checkLuck = false;*/

                    // luck daje jeszcze jedna szanse na wylosowanie itemu
                    if ( LootPack.CheckLuck( luckChance ) )
                        shouldAdd = ( entry.Chance > Utility.Random( 10000 ) );
                }

                if ( !shouldAdd )
                    continue;
                
                Item item = entry.Construct( from, luckChance, spawning );

                if ( item != null )
                {
                    if ( !item.Stackable || !cont.TryDropItem( from, item, false ) )
                        cont.DropItem( item );
                }
            }
        }

        public static readonly LootPackItem[] Gold = new LootPackItem[]
            {
                new LootPackItem( typeof( Gold ), 1 )
            };

        public static readonly LootPackItem[] Instruments = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseInstrument ), 1 )
            };


        public static readonly LootPackItem[] LowScrollItems = new LootPackItem[]
            {
                new LootPackItem( typeof( ClumsyScroll ), 1 )
            };

        public static readonly LootPackItem[] MedScrollItems = new LootPackItem[]
            {
                new LootPackItem( typeof( ArchCureScroll ), 1 )
            };

        public static readonly LootPackItem[] HighScrollItems = new LootPackItem[]
            {
                new LootPackItem( typeof( SummonAirElementalScroll ), 1 )
            };

        public static readonly LootPackItem[] GemItems = new LootPackItem[]
            {
                new LootPackItem( typeof( Amber ), 1 )
            };

        public static readonly LootPackItem[] PotionItems = new LootPackItem[]
            {
                // wzgledny udzial ilosci poszczegolnych mikstur definiowany przez drugi argument konstruktorow
                new LootPackItem( typeof( AgilityPotion ), 3 ),
                new LootPackItem( typeof( StrengthPotion ), 3 ),
                new LootPackItem( typeof( RefreshPotion ), 3 ),
                new LootPackItem( typeof( LesserCurePotion ), 2 ),
                new LootPackItem( typeof( LesserHealPotion ), 3 ),
                new LootPackItem( typeof( LesserPoisonPotion ), 1 ),
                new LootPackItem( typeof( NightSightPotion ), 2 ),
                new LootPackItem( typeof( LesserExplosionPotion ), 1 )
            };

        #region Old Magic Items
        public static readonly LootPackItem[] OldMagicItems = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseJewel ), 1 ),
                new LootPackItem( typeof( BaseArmor ), 4 ),
                new LootPackItem( typeof( BaseWeapon ), 3 ),
                new LootPackItem( typeof( BaseRanged ), 1 ),
                new LootPackItem( typeof( BaseShield ), 1 )
            };
        #endregion

        #region AOS Magic Items
        public static readonly LootPackItem[] AosMagicItemsPoor = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 3 ),
                new LootPackItem( typeof( BaseRanged ), 1 ),
                new LootPackItem( typeof( BaseArmor ), 4 ),
                new LootPackItem( typeof( BaseShield ), 1 ),
                new LootPackItem( typeof( BaseJewel ), 2 )
            };

        public static readonly LootPackItem[] AosMagicItemsMeagerType1 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 56 ),
                new LootPackItem( typeof( BaseRanged ), 14 ),
                new LootPackItem( typeof( BaseArmor ), 81 ),
                new LootPackItem( typeof( BaseShield ), 11 ),
                new LootPackItem( typeof( BaseJewel ), 42 )
            };

        public static readonly LootPackItem[] AosMagicItemsMeagerType2 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 28 ),
                new LootPackItem( typeof( BaseRanged ), 7 ),
                new LootPackItem( typeof( BaseArmor ), 40 ),
                new LootPackItem( typeof( BaseShield ), 5 ),
                new LootPackItem( typeof( BaseJewel ), 21 )
            };

        public static readonly LootPackItem[] AosMagicItemsAverageType1 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 90 ),
                new LootPackItem( typeof( BaseRanged ), 23 ),
                new LootPackItem( typeof( BaseArmor ), 130 ),
                new LootPackItem( typeof( BaseShield ), 17 ),
                new LootPackItem( typeof( BaseJewel ), 68 )
            };

        public static readonly LootPackItem[] AosMagicItemsAverageType2 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 54 ),
                new LootPackItem( typeof( BaseRanged ), 13 ),
                new LootPackItem( typeof( BaseArmor ), 77 ),
                new LootPackItem( typeof( BaseShield ), 10 ),
                new LootPackItem( typeof( BaseJewel ), 40 )
            };

        public static readonly LootPackItem[] AosMagicItemsRichType1 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 211 ),
                new LootPackItem( typeof( BaseRanged ), 53 ),
                new LootPackItem( typeof( BaseArmor ), 303 ),
                new LootPackItem( typeof( BaseShield ), 39 ),
                new LootPackItem( typeof( BaseJewel ), 158 )
            };

        public static readonly LootPackItem[] AosMagicItemsRichType2 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 170 ),
                new LootPackItem( typeof( BaseRanged ), 43 ),
                new LootPackItem( typeof( BaseArmor ), 245 ),
                new LootPackItem( typeof( BaseShield ), 32 ),
                new LootPackItem( typeof( BaseJewel ), 128 )
            };

        public static readonly LootPackItem[] AosMagicItemsFilthyRichType1 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 219 ),
                new LootPackItem( typeof( BaseRanged ), 55 ),
                new LootPackItem( typeof( BaseArmor ), 315 ),
                new LootPackItem( typeof( BaseShield ), 41 ),
                new LootPackItem( typeof( BaseJewel ), 164 )
            };

        public static readonly LootPackItem[] AosMagicItemsFilthyRichType2 = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 239 ),
                new LootPackItem( typeof( BaseRanged ), 60 ),
                new LootPackItem( typeof( BaseArmor ), 343 ),
                new LootPackItem( typeof( BaseShield ), 90 ),
                new LootPackItem( typeof( BaseJewel ), 45 )
            };

        public static readonly LootPackItem[] AosMagicItemsUltraRich = new LootPackItem[]
            {
                new LootPackItem( typeof( BaseWeapon ), 276 ),
                new LootPackItem( typeof( BaseRanged ), 69 ),
                new LootPackItem( typeof( BaseArmor ), 397 ),
                new LootPackItem( typeof( BaseShield ), 52 ),
                new LootPackItem( typeof( BaseJewel ), 207 )
            };
        #endregion

         // 09.10.2012 :: zombie :: nowy system lootu
        public static readonly LootPackItem[] RecallRune = new LootPackItem[]
        {
            new LootPackItem( typeof( RecallRune ), 1 )
        };

        public static readonly LootPackItem[] SpellbooksMage = new LootPackItem[]
        {
            new LootPackItem( typeof( Spellbook ), 2 ),
            new LootPackItem( typeof( NecromancerSpellbook ), 1 )
        };

        public static readonly LootPackItem[] SpellbooksWarrior = new LootPackItem[]
        {
            new LootPackItem( typeof( BookOfBushido ), 7 ),
            new LootPackItem( typeof( BookOfChivalry ), 8 ),
            new LootPackItem( typeof( BookOfNinjitsu ), 6 )
        };

        #region Nelderim Scrolls
        public static readonly LootPackItem[] NL_Scrolls1 = new LootPackItem[]
        {
            new LootPackItem( typeof( ClumsyScroll ), 1 ),
            new LootPackItem( typeof( CreateFoodScroll ), 1 ),
            new LootPackItem( typeof( FeeblemindScroll ), 1 ),
            new LootPackItem( typeof( HealScroll ), 1 ),
            new LootPackItem( typeof( MagicArrowScroll ), 1 ),
            new LootPackItem( typeof( NightSightScroll ), 1 ),
            new LootPackItem( typeof( ReactiveArmorScroll ), 1 ),
            new LootPackItem( typeof( WeakenScroll ), 1 ),
            new LootPackItem( typeof( CleanseByFireScroll ), 1 ),
            new LootPackItem( typeof( CurseWeaponScroll ), 1 ),
            new LootPackItem( typeof( AnimalFormScroll ), 1 )
        };

        public static readonly LootPackItem[] NL_Scrolls2 = new LootPackItem[]
        {
            new LootPackItem( typeof( AgilityScroll ), 1 ),
            new LootPackItem( typeof( CunningScroll ), 1 ),
            new LootPackItem( typeof( CureScroll ), 1 ),
            new LootPackItem( typeof( HarmScroll ), 1 ),
            new LootPackItem( typeof( MagicTrapScroll ), 1 ),
            new LootPackItem( typeof( MagicUnTrapScroll ), 1 ),
            new LootPackItem( typeof( ProtectionScroll ), 1 ),
            new LootPackItem( typeof( StrengthScroll ), 1 ),
            new LootPackItem( typeof( RemoveCurseScroll ), 1 ),
            new LootPackItem( typeof( CorpseSkinScroll ), 1 ),
            new LootPackItem( typeof( WraithFormScroll ), 1 ),
            new LootPackItem( typeof( EvilOmenScroll ), 1 ),
            new LootPackItem( typeof( BackstabScroll ), 1 )
        };

        public static readonly LootPackItem[] NL_Scrolls3 = new LootPackItem[]
        {
            new LootPackItem( typeof( BlessScroll ), 1 ),
            new LootPackItem( typeof( FireballScroll ), 1 ),
            new LootPackItem( typeof( MagicLockScroll ), 1 ),
            new LootPackItem( typeof( PoisonScroll ), 1 ),
            new LootPackItem( typeof( TelekinisisScroll ), 1 ),
            new LootPackItem( typeof( TeleportScroll ), 1 ),
            new LootPackItem( typeof( UnlockScroll ), 1 ),
            new LootPackItem( typeof( WallOfStoneScroll ), 1 ),
            new LootPackItem( typeof( ConsecrateWeaponScroll ), 1 ),
            new LootPackItem( typeof( CloseWoundsScroll ), 1 ),
            new LootPackItem( typeof( BloodOathScroll ), 1 ),
            new LootPackItem( typeof( PainSpikeScroll ), 1 ),
            new LootPackItem( typeof( SummonFamiliarScroll ), 1 ),
            new LootPackItem( typeof( ConfidenceScroll ), 1 ),
            new LootPackItem( typeof( HonorableExecutionScroll ), 1 ),
            new LootPackItem( typeof( SurpriseAttackScroll ), 1 )
        };

        public static readonly LootPackItem[] NL_Scrolls4 = new LootPackItem[]
        {
            new LootPackItem( typeof( ArchCureScroll ), 1 ),
            new LootPackItem( typeof( ArchProtectionScroll ), 1 ),
            new LootPackItem( typeof( CurseScroll ), 1 ),
            new LootPackItem( typeof( FireFieldScroll ), 1 ),
            new LootPackItem( typeof( GreaterHealScroll ), 1 ),
            new LootPackItem( typeof( LightningScroll ), 1 ),
            new LootPackItem( typeof( ManaDrainScroll ), 1 ),
            new LootPackItem( typeof( RecallScroll ), 1 ),
            new LootPackItem( typeof( DivineFuryScroll ), 1 ),
            new LootPackItem( typeof( SacredJourneyScroll ), 1 ),
            new LootPackItem( typeof( MindRotScroll ), 1 ),
            new LootPackItem( typeof( HorrificBeastScroll ), 1 ),
            new LootPackItem( typeof( AnimateDeadScroll ), 1 ),
            new LootPackItem( typeof( CounterAttackScroll ), 1 ),
            new LootPackItem( typeof( MirrorImageScroll ), 1 )
        };

        public static readonly LootPackItem[] NL_Scrolls5 = new LootPackItem[]
        {
            new LootPackItem( typeof( BladeSpiritsScroll ), 1 ),
            new LootPackItem( typeof( DispelFieldScroll ), 1 ),
            new LootPackItem( typeof( IncognitoScroll ), 1 ),
            new LootPackItem( typeof( MagicReflectScroll ), 1 ),
            new LootPackItem( typeof( MindBlastScroll ), 1 ),
            new LootPackItem( typeof( ParalyzeScroll ), 1 ),
            new LootPackItem( typeof( PoisonFieldScroll ), 1 ),
            new LootPackItem( typeof( SummonCreatureScroll ), 1 ),
            new LootPackItem( typeof( DispelEvilScroll ), 1 ),
            new LootPackItem( typeof( WitherScroll ), 1 ),
            new LootPackItem( typeof( PoisonStrikeScroll ), 1 ),
            new LootPackItem( typeof( LightningStrikeScroll ), 1 ),
            new LootPackItem( typeof( ShadowJumpScroll ), 1 )
        };

        public static readonly LootPackItem[] NL_Scrolls6 = new LootPackItem[]
        {
            new LootPackItem( typeof( DispelScroll ), 1 ),
            new LootPackItem( typeof( EnergyBoltScroll ), 1 ),
            new LootPackItem( typeof( ExplosionScroll ), 1 ),
            new LootPackItem( typeof( InvisibilityScroll ), 1 ),
            new LootPackItem( typeof( MarkScroll ), 1 ),
            new LootPackItem( typeof( MassCurseScroll ), 1 ),
            new LootPackItem( typeof( ParalyzeFieldScroll ), 1 ),
            new LootPackItem( typeof( RevealScroll ), 1 ),
            new LootPackItem( typeof( EnemyOfOneScroll ), 1 ),
            new LootPackItem( typeof( StrangleScroll ), 1 ),
            new LootPackItem( typeof( LichFormScroll ), 1 ),
            new LootPackItem( typeof( EvasionScroll ), 1 ),
            new LootPackItem( typeof( FocusAttackScroll ), 1 )
        };

        public static readonly LootPackItem[] NL_Scrolls7 = new LootPackItem[]
        {
            new LootPackItem( typeof( ChainLightningScroll ), 1 ),
            new LootPackItem( typeof( EnergyFieldScroll ), 1 ),
            new LootPackItem( typeof( FlamestrikeScroll ), 1 ),
            new LootPackItem( typeof( GateTravelScroll ), 1 ),
            new LootPackItem( typeof( ManaVampireScroll ), 1 ),
            new LootPackItem( typeof( MassDispelScroll ), 1 ),
            new LootPackItem( typeof( MeteorSwarmScroll ), 1 ),
            new LootPackItem( typeof( PolymorphScroll ), 1 ),
            new LootPackItem( typeof( HolyLightScroll ), 1 ),
            new LootPackItem( typeof( ExorcismScroll ), 1 ),
            new LootPackItem( typeof( VengefulSpiritScroll ), 1 ),
            new LootPackItem( typeof( MomentumStrikeScroll ), 1 ),
            new LootPackItem( typeof( KiAttackScroll ), 1 ),
            new LootPackItem( typeof( DeathStrikeScroll ), 1 )
        };

        public static readonly LootPackItem[] NL_Scrolls8 = new LootPackItem[]
        {
            new LootPackItem( typeof( EarthquakeScroll ), 1 ),
            new LootPackItem( typeof( EnergyVortexScroll ), 1 ),
            new LootPackItem( typeof( ResurrectionScroll ), 1 ),
            new LootPackItem( typeof( SummonAirElementalScroll ), 1 ),
            new LootPackItem( typeof( SummonDaemonScroll ), 1 ),
            new LootPackItem( typeof( SummonEarthElementalScroll ), 1 ),
            new LootPackItem( typeof( SummonFireElementalScroll ), 1 ),
            new LootPackItem( typeof( SummonWaterElementalScroll ), 1 ),
            new LootPackItem( typeof( NobleSacrificeScroll ), 1 ),
            new LootPackItem( typeof( VampiricEmbraceScroll ), 1 )
        };

        public static readonly LootPackItem[] DeathKnightItems = new LootPackItem[]
            {
                 new LootPackItem( typeof( DeathKnightSkull750 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull751 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull752 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull753 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull754 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull755 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull756 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull757 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull758 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull759 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull760 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull761 ), 1 ),
                 new LootPackItem( typeof( DeathKnightSkull762 ), 1 )

            };

            public static readonly LootPackItem[] RogueScrollItems = new LootPackItem[]
            {
                 new LootPackItem( typeof( RogueCharmScroll ), 1 ),
                 new LootPackItem( typeof( RogueFalseCoinScroll ), 1 ),
                 new LootPackItem( typeof( RogueIntimidationScroll ), 1 ),
                 new LootPackItem( typeof( RogueShadowScroll ), 1 ),   
                 new LootPackItem( typeof( RogueShieldOfEarthScroll ), 1 ),   
                 new LootPackItem( typeof( RogueSlyFoxScroll ), 1 )
            };
        
        public static readonly LootPackItem[] AncientScrollItems = new LootPackItem[]
            {
               
                new LootPackItem( typeof( AncientCauseFearScroll ), 1 ),
                new LootPackItem( typeof( AncientCloneScroll ), 1 ),
                new LootPackItem( typeof(  AncientDanceScroll ), 1 ),
                new LootPackItem( typeof( AncientDeathVortexScroll ), 1 ),
                new LootPackItem( typeof( AncientDouseScroll ), 1 ),
                new LootPackItem( typeof( AncientEnchantScroll ), 1 ),
                new LootPackItem( typeof( AncientFireRingScroll ), 1 ),
                new LootPackItem( typeof( AncientGreatDouseScroll ), 1 ),
                new LootPackItem( typeof( AncientGreatIgniteScroll ), 1 ),
                new LootPackItem( typeof( AncientIgniteScroll ), 1 ),
                new LootPackItem( typeof( AncientMassMightScroll), 1 ),
                new LootPackItem( typeof( AncientPeerScroll ), 1 ),
                new LootPackItem( typeof( AncientSeanceScroll ), 1 ),
                new LootPackItem( typeof( AncientSwarmScroll ), 1 ),
                new LootPackItem( typeof( AncientDeathVortexScroll ), 1 ),
                new LootPackItem( typeof( AncientDeathVortexScroll ), 1 )
                
            };
        public static readonly LootPackItem[] AvatarScrollItems = new LootPackItem[]
            {
               
                new LootPackItem( typeof( AvatarHeavenlyLightScroll ), 1 ),
                new LootPackItem( typeof( AvatarHeavensGateScroll ), 1 ),
                new LootPackItem( typeof( AvatarMarkOfGodsScroll ), 1 ),
                new LootPackItem( typeof( AvatarRestorationScroll ), 1 ),
                new LootPackItem( typeof( AvatarSacredBoonScroll ), 1 ),
                new LootPackItem( typeof( AvatarAngelicFaithScroll ), 1 ),
                new LootPackItem( typeof( AvatarArmysPaeonScroll ), 1 ),
                new LootPackItem( typeof( AvatarBallSpell ), 1 ),
                new LootPackItem( typeof( AvatarCurseRemovalSpell ), 1 ),
                new LootPackItem( typeof( AvatarEnemyOfOneScroll ), 1 )
                
            };
            public static readonly LootPackItem[] BardScrollItems = new LootPackItem[]
            {
               
                new LootPackItem( typeof( BardArmysPaeonScroll ), 1 ),
                new LootPackItem( typeof( BardEnchantingEtudeScroll ), 1 ),
                new LootPackItem( typeof( BardEnergyCarolScroll ), 1 ),
                new LootPackItem( typeof( AncientDeathVortexScroll ), 1 ),
                new LootPackItem( typeof( BardEnergyThrenodyScroll ), 1 ),
                new LootPackItem( typeof( BardFireCarolScroll ), 1 ),
                new LootPackItem( typeof( BardFireThrenodyScroll ), 1 ),
                new LootPackItem( typeof( BardFoeRequiemScroll ), 1 ),
                new LootPackItem( typeof( BardIceCarolScroll ), 1 ),
                new LootPackItem( typeof( BardIceThrenodyScroll ), 1 ),
                new LootPackItem( typeof( BardKnightsMinneScroll), 1 ),
                new LootPackItem( typeof( BardMagesBalladScroll ), 1 ),
                new LootPackItem( typeof( BardMagicFinaleScroll ), 1 ),
                new LootPackItem( typeof( BardPoisonCarolScroll ), 1 ),
                new LootPackItem( typeof( BardPoisonThrenodyScroll ), 1 ),
                new LootPackItem( typeof( BardSheepfoeMamboScroll ), 1 ),
                new LootPackItem( typeof( BardSinewyEtudeScroll ), 1 )
                
            };
                        public static readonly LootPackItem[] ClericScrollItems = new LootPackItem[]
            {
               
                new LootPackItem( typeof( ClericAngelicFaithScroll ), 1 ),
                new LootPackItem( typeof( ClericBanishEvilScroll ), 1 ),
                new LootPackItem( typeof( ClericDampenSpiritScroll ), 1 ),
                new LootPackItem( typeof( ClericDivineFocusScroll ), 1 ),
                new LootPackItem( typeof( ClericPurgeScroll ), 1 ),
                new LootPackItem( typeof( ClericHammerOfFaithScroll ), 1 ),
                new LootPackItem( typeof( ClericRestorationScroll ), 1 ),
                new LootPackItem( typeof( ClericSacredBoonScroll ), 1 ),
                new LootPackItem( typeof( ClericSacrificeScroll ), 1 ),
                new LootPackItem( typeof( ClericTouchOfLifeScroll ), 1 ),
                new LootPackItem( typeof( ClericSmiteScroll), 1 ),
                new LootPackItem( typeof( ClericTrialByFireScroll), 1 )
                
            };
                        public static readonly LootPackItem[] DruidScrollItems = new LootPackItem[]
            {
               
                new LootPackItem( typeof( DruidBlendWithForestScroll ), 1 ),
                new LootPackItem( typeof( DruidFamiliarScroll ), 1 ),
                new LootPackItem( typeof( DruidEnchantedGroveScroll ), 1 ),
                new LootPackItem( typeof( DruidGraspingRootsScroll ), 1 ),
                new LootPackItem( typeof( DruidHollowReedScroll ), 1 ),
                new LootPackItem( typeof( DruidLeafWhirlwindScroll ), 1 ),
                new LootPackItem( typeof( DruidLureStoneScroll ), 1 ),
                new LootPackItem( typeof( DruidMushroomGatewayScroll ), 1 ),
                new LootPackItem( typeof( DruidNaturesPassageScroll ), 1 ),
                new LootPackItem( typeof( DruidPackOfBeastScroll ), 1 ),
                new LootPackItem( typeof( DruidRestorativeSoilScroll), 1 ),
                new LootPackItem( typeof( DruidShieldOfEarthScroll ), 1 ),
                new LootPackItem( typeof( DruidSpringOfLifeScroll ), 1 ),
                new LootPackItem( typeof( DruidStoneCircleScroll ), 1 ),
                new LootPackItem( typeof( DruidSwarmOfInsectsScroll ), 1 ),
                new LootPackItem( typeof( DruidVolcanicEruptionScroll ), 1 )
                
            };
            public static readonly LootPackItem[] RangerScrollItems = new LootPackItem[]
            {
               
                new LootPackItem( typeof( RangerFireBowScroll ), 1 ),
                new LootPackItem( typeof( RangerHuntersAimScroll ), 1 ),
                new LootPackItem( typeof( RangerIceBowScroll  ), 1 ),
                new LootPackItem( typeof( RangerLightningBowScroll ), 1 ),
                new LootPackItem( typeof( RangerNoxBowScroll ), 1 ),
                new LootPackItem( typeof( RangerPhoenixFlightScroll ), 1 ),
                new LootPackItem( typeof( RangerFamiliarScroll ), 1 ),
                new LootPackItem( typeof( RangerThrowSwordSpell ), 1 ),
                new LootPackItem( typeof( RangerTrialByFireSpell ), 1 ),
                new LootPackItem( typeof( RangerSummonMountScroll ), 1 )
            };

            public static readonly LootPackItem[] UndeadScrollItems = new LootPackItem[]
            {
               
                new LootPackItem( typeof( UndeadAngelicFaithScroll ), 1 ),
                new LootPackItem( typeof( UndeadCauseFearScroll ), 1 ),
                new LootPackItem( typeof( UndeadGraspingRootsScroll ), 1 ),
                new LootPackItem( typeof( UndeadHammerOfFaithScroll ), 1 ),
                new LootPackItem( typeof( UndeadHollowReedScroll ), 1 ),
                new LootPackItem( typeof( UndeadLeafWhirlwindScroll ), 1 ),
                new LootPackItem( typeof( UndeadLureStoneScroll ), 1 ),
                new LootPackItem( typeof( UndeadMushroomGatewayScroll ), 1 ),
                new LootPackItem( typeof( UndeadNaturesPassageScroll ), 1 ),
                new LootPackItem( typeof( UndeadSeanceScroll ), 1 ),
                new LootPackItem( typeof( UndeadSwarmOfInsectsScroll), 1 ),
                new LootPackItem( typeof( UndeadVolcanicEruptionScroll ), 1 )
                
            };
        #endregion

        #region AIType Items
        public static readonly LootPackItem[] NL_BossItems = new LootPackItem[]
        {
            new LootPackItem( typeof( BaseWeapon ), 25 ),
            new LootPackItem( typeof( BaseRanged ), 15 ),
            new LootPackItem( typeof( BaseArmor ), 25 ),
            new LootPackItem( typeof( BaseShield ), 15 ),
            new LootPackItem( typeof( BaseJewel ), 20 )
        };
 
        public static readonly LootPackItem[] NL_ArcherItems = new LootPackItem[]
        {
            new LootPackItem( typeof( BaseWeapon ), 20 ),
            new LootPackItem( typeof( BaseRanged ), 35 ),
            new LootPackItem( typeof( BaseArmor ), 30 ),
            new LootPackItem( typeof( BaseShield ), 10 ),
            new LootPackItem( typeof( BaseJewel ), 5 )
        };
 
        public static readonly LootPackItem[] NL_MageItems = new LootPackItem[]
        {
            new LootPackItem( typeof( BaseWeapon ), 15 ),
            new LootPackItem( typeof( BaseRanged ), 15 ),
            new LootPackItem( typeof( BaseArmor ), 35 ),
            new LootPackItem( typeof( BaseShield ), 20 ),
            new LootPackItem( typeof( BaseJewel ), 15 )
        };
 
        public static readonly LootPackItem[] NL_BattleMageItems = new LootPackItem[]
        {
            new LootPackItem( typeof( BaseWeapon ), 30 ),
            new LootPackItem( typeof( BaseRanged ), 20 ),
            new LootPackItem( typeof( BaseArmor ), 30 ),
            new LootPackItem( typeof( BaseShield ), 10 ),
            new LootPackItem( typeof( BaseJewel ), 10 )
        };
 
        public static readonly LootPackItem[] NL_MeleeItems = new LootPackItem[]
        {
            new LootPackItem( typeof( BaseWeapon ), 30 ),
            new LootPackItem( typeof( BaseRanged ), 10 ),
            new LootPackItem( typeof( BaseArmor ), 40 ),
            new LootPackItem( typeof( BaseShield ), 15 ),
            new LootPackItem( typeof( BaseJewel ), 5 )
        };

        public static int WeaponChance  { get { return 20; } } // było 9
        public static int RangedChance  { get { return 4; } }
        public static int ArmorChance   { get { return 57; } }  // 60
        public static int HatChance     { get { return 3; } }   // 0
        public static int ShieldChance  { get { return 7; } }
        public static int JewelChance   { get { return 20; } }

        public static readonly LootPackItem[] NelderimItems = new LootPackItem[]
        {
            new LootPackItem( typeof( BaseWeapon ), WeaponChance ),
            new LootPackItem( typeof( BaseRanged ), RangedChance ),
            new LootPackItem( typeof( BaseArmor ), ArmorChance ),
            new LootPackItem( typeof( BaseHat ),    HatChance ),
            new LootPackItem( typeof( BaseShield ), ShieldChance ),
            new LootPackItem( typeof( BaseJewel ), JewelChance )
        };
        #endregion

        public static LootPackItem[] GetLootPackItems( AIType ai )
        {
            return NelderimItems;
            /*
            if( ai == AIType.AI_Boss )
                return NL_BossItems;
            else if( ai == AIType.AI_Mage || ai == AIType.AI_NecroMage || ai == AIType.AI_Healer )
                return NL_MageItems;
            else if( ai == AIType.AI_BattleMage )
                return NL_BattleMageItems;
            else if( ai == AIType.AI_Archer )
                return NL_ArcherItems;
            else
                return NL_MeleeItems;
            */
        }

        // 11.10.2012 :: zombie  
        public static int GetRandomIndex( double[] chances )
        {
            double rand = Utility.RandomDouble();

            for ( int i = 0; i < chances.Length; i++ )
            {
                double chance = chances[ i ];

                if ( rand < chance )
                    return i;
                else
                    rand -= chance;
            }

            return -1;
        }

        public static double[] GetChances( double baseChance, double[] factors )
        {
            double[] chances = new double[ factors.Length ];
            double sum = 0;

            // zdefiniuj warunek na maksymalna szanse dla danego kregu:
            double[] max = new double[factors.Length];
            for (int i = 0; i < factors.Length; i++)
            {
                switch(i)
                {
                    case 0: max[i] = 0.20; break;    // 8 krag
                    case 1: max[i] = 0.25; break;    // 7 krag
                    case 2: max[i] = 1.00; break;    // 6 krag
                    case 3: max[i] = 1.00; break;    // 5 krag
                    case 4: max[i] = 1.00; break;    // 4 krag
                    case 5: max[i] = 1.00; break;    // 3 krag
                    case 6: max[i] = 1.00; break;    // 2 krag
                    case 7:
                    default:max[i] = 1.00; break;   // 1 krag
                }
            }

            for ( int i = 0; i < factors.Length; i++ )
            {
                chances[i] = (i == 0) ? baseChance : Math.Min(1 - sum, chances[i - 1] * factors[i]);

                chances[i] = Math.Min(max[i], chances[i]);
                
                sum += chances[ i ];
            }

            return chances;
        }
      
        public static void GenerateGold( BaseCreature bc, ref List<LootPackEntry> entries )
        {
            int minGold = (int)Math.Ceiling( Math.Pow( ( bc.Difficulty * 630 ), 0.49 ) );
            int maxGold = (int)Math.Ceiling( (double)( minGold + minGold / 3 ) );

            int gold = Utility.RandomMinMax( minGold, maxGold );

            entries.Add( new LootPackEntry( true, Gold, 100, gold ) );
        }

        public static void GenerateRecallRunes( BaseCreature bc, ref List<LootPackEntry> entries )
        {
            int count     = (int)Math.Min( 5, 1 + bc.Difficulty / 500 );
            double chance = (double)Math.Pow( bc.Difficulty, 0.6 ) / 10;

            for( int i = 0; i < count; i++ )
                entries.Add( new LootPackEntry( false, RecallRune, chance, 1 ) );
        }

        public static void GenerateSpellbooks( BaseCreature bc, ref List<LootPackEntry> entries )
        {
            // szanse w konstruktorze LootPackEntry() podaje sie w zakresie 0-100
            double chanceMage    = (double)Math.Pow( bc.Difficulty, 0.6 ) / 10.0 * 2.5;
            double chanceWarrior = (double)Math.Pow( bc.Difficulty, 0.6 ) / 10.0 * 2.5;
            switch( bc.AI )
            {
                case AIType.AI_Mage:
                    chanceMage    *= 1.0;
                    chanceWarrior *= 0.5;
                    break;
                case AIType.AI_BattleMage:
                    chanceMage    *= 0.5;
                    chanceWarrior *= 1.0;
                    break;
                case AIType.AI_Boss:
                    if ( bc.Skills.Magery.Value >= 80.0 )
                    {
                        chanceMage    *= 1.0;
                        chanceWarrior *= 0.5;
                    }
                    else
                    {
                        chanceMage    *= 0.5;
                        chanceWarrior *= 1.0;
                    }
                    break;
                default:
                    return;
            }

            entries.Add( new LootPackEntry( false, SpellbooksMage, chanceMage, 1 ) );
            entries.Add( new LootPackEntry( false, SpellbooksWarrior, chanceWarrior, 1 ) );
        }

        public static void GenerateGems( BaseCreature bc, ref List<LootPackEntry> entries )
        {
            int count     = (int)Math.Ceiling( Math.Pow( bc.Difficulty, 0.35 ) );
            double chance = (double)Math.Max( 0.01, Math.Min( 1, 0.1 + Math.Pow( bc.Difficulty, 0.3 ) / 20 ) ) * 100;
            
            for( int i = 0; i < count; i++ )
                entries.Add( new LootPackEntry( false, GemItems, chance, 1 ) );
        }

        public static void GenerateInstruments( BaseCreature bc, ref List<LootPackEntry> entries )
        {
            double chance = (double)Math.Max( 0.01, Math.Min( 1, Math.Pow( bc.Difficulty, 0.3 ) / 30 ) ) * 100;
            entries.Add( new LootPackEntry( false, Instruments, chance, 1 ) );
        }

        public static void GenerateScrolls( BaseCreature bc, ref List<LootPackEntry> entries )
        {
            int count = (int)Math.Floor( Math.Pow( (1+bc.Difficulty) * 39.0 , 0.15 ) );
            double scrollChance =  Math.Max( 0.01, Math.Min( 0.8, Math.Pow( bc.Difficulty * 0.5 , 0.29 ) * 0.03 ) );

            LootPackItem[][] scrolls = new LootPackItem[][] 
            { 
                NL_Scrolls8, NL_Scrolls7, NL_Scrolls6, NL_Scrolls5, NL_Scrolls4, NL_Scrolls3, NL_Scrolls2, NL_Scrolls1
            };

            double[] lvlChances = GetChances( Math.Min( 1, Math.Pow( bc.Difficulty, 0.9 ) / 10000 ), new double[] { 0, 6, 8, 4, 3, 1.3, 1, 100 } );

            for ( int i = 0, index = -1; i < count; i++ )
            {
                if ( Utility.RandomDouble() >= scrollChance || ( index = GetRandomIndex( lvlChances ) ) == -1 )
                    continue;

                entries.Add( new LootPackEntry( false, scrolls[ index ], 100, 1 ) );
            }
        }

        public static void GeneratePotions( BaseCreature bc, ref List<LootPackEntry> entries )
        {
            int count = (int)Math.Floor( Math.Pow( (1+bc.Difficulty) * 39.0 , 0.15 ) );
            double potionChance =  100 * Math.Max( 0.01, Math.Min( 0.8, Math.Pow( bc.Difficulty * 0.5 , 0.29 ) * 0.1 ) );

            for( int i = 0; i < count; i++ )
                entries.Add( new LootPackEntry( false, PotionItems, potionChance, 1 ) );
        }

        // Ilosc magicznych przedmiotow w zaleznosci od trudnosci potwora:
        public static int NumberOfMagicItems(double difficulty)
        {
            int min = (int)Math.Floor(Math.Pow(difficulty, 0.275));
            int max = (int)Math.Max(1, min + Math.Ceiling((double)min / 3));
            return Utility.RandomMinMax(min, max);
        }

        // Progi intensywnosci propsow w zaleznosci od levelu przedmiotu:
        private const int LevelCount = 5;
        private static double[,] LevelPropsIntensity = new double[LevelCount, 4] {
			{ 5,          5,        30,      100 }, // Lvl_5
            { 4,          5,        25,      100 }, // Lvl_4
            { 2,          4,        15,      100 }, // Lvl_3
            { 1,          3,        10,      90 }, // Lvl_2
            { 1,          1,        5,      80 }  // Lvl_1
        };

        // Losowanie ilosci propsow i ich intensywnosci w oparciu o trudnosc potwora:
        public static void GenPropsIntensity(double difficulty, out int minProps, out int maxProps, out int minInt, out int maxInt)
        {
            int lvl;
            GenPropsIntensity(difficulty, out minProps, out maxProps, out minInt, out maxInt, out lvl);
        }
        public static void GenPropsIntensity(double difficulty, out int minProps, out int maxProps, out int minInt, out int maxInt, out int level)
        {
            double[] chances = GetChances(Math.Pow(difficulty, 0.5) / 1000, new double[] { 0, 4, 8, 10, 2 });

			// progowanie (zeruj szanse na dany level jesli diff nie przekroczyl progowej wartosci):
			double[] tresholds = new double[] { 92, 24, 8, 1.8, 0.1 };
			for (int i=0; i<chances.Length && i<tresholds.Length; i++)
			{
				if (difficulty < tresholds[i] )
					chances[i] = 0;
			}

            int index;
            if ((index = GetRandomIndex(chances)) == -1)
            {
                minProps = maxProps = minInt = maxInt = 0;
				level = -1; // (wylosowano item pozniej 1 lvl - czyli brak itemu)
 				return;
            }

            minProps = (int)LevelPropsIntensity[index, 0];
            maxProps = (int)LevelPropsIntensity[index, 1];
            minInt = (int)LevelPropsIntensity[index, 2];
            maxInt = (int)LevelPropsIntensity[index, 3];
            level = LevelCount - index;
        }

        public static void GenerateMagicItems(BaseCreature bc, ref List<LootPackEntry> entries)
        {
            int itemsCnt = NumberOfMagicItems(bc.Difficulty);

            LootPackItem[] lootPackItems = GetLootPackItems( bc.AI );

            for ( int i = 0; i < itemsCnt; i++ )
            {
				int minProps, maxProps, minInt, maxInt, level;
                //Console.WriteLine( "Adding item: index: {0}, minProps: {1}, maxProps: {2}, minInt: {3}, maxInt: {4}", index, minProps, maxProps, minInt, maxInt );
                GenPropsIntensity(bc.Difficulty, out minProps, out maxProps, out minInt, out maxInt, out level);
				if( level == -1 )
					continue;
                entries.Add( new LootPackEntry( false, lootPackItems, 100, 1, minProps, maxProps, minInt, maxInt, level ) );
            }
        }

        public static LootPack GetLootPack( BaseCreature bc, bool spawning )
        {
            if ( bc.Controlled || bc.Summoned || bc.AI == AIType.AI_Animal )
                return null;

            List<LootPackEntry> entries = new List<LootPackEntry>();

            if ( spawning )
                GenerateGold( bc, ref entries );
            else
            {
                //GenerateSpellbooks( bc, ref entries );
                GenerateRecallRunes( bc, ref entries );
                GenerateGems( bc, ref entries );
                GenerateScrolls( bc, ref entries );
                GeneratePotions( bc, ref entries );
                GenerateInstruments( bc, ref entries );
                GenerateMagicItems( bc, ref entries );
            }

            if ( entries.Count > 0 )
                return new LootPack( entries.ToArray() );
            else
                return null;
        }
        // zombie

        public static void GiveAdditionalResists(Item item, int lvl)
        {

            if ((item is BaseArmor && !(item is BaseShield)) || item is BaseHat)
            {
                int minResTotal = 5;
                int maxResTotal = 20;
                int singleMin = 4;
                int singleMax = 10;

                double rand = Utility.RandomDouble();
                switch (lvl)
                {
                    case 1: rand = Math.Pow(rand, 1.6); break;
                    case 2: rand = Math.Pow(rand, 1.2); break;
                    case 3: /* rand = rand; */ break;
                    case 4: rand = 1.0 - Math.Pow(1 - rand, 1.35); break;
                    case 5: rand = 1.0 - Math.Pow(1 - rand, 1.70); break;
                }

                int totalLeft = minResTotal + (int)Math.Round((double)(maxResTotal - minResTotal) * rand, 0, MidpointRounding.AwayFromZero);

                if (singleMax > totalLeft)
                    singleMax = totalLeft;
                if (singleMin * 5 < totalLeft)
                    singleMin = (int)Math.Ceiling(totalLeft / 5.0);
                if (singleMax < singleMin)
                    singleMax = singleMin;

                List<int> indexes = new List<int>() { 0, 1, 2, 3, 4 };
                int[] resists = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    int resIndex = Utility.Random(indexes.Count);
                    int res = Utility.Random(singleMin, singleMax - singleMin);
                    if (res > totalLeft)
                        res = totalLeft;

                    resists[indexes[resIndex]] += res;

                    indexes.RemoveAt(resIndex);
                    totalLeft -= res;
                    if (totalLeft <= 0)
                        break;
                }

                for (int i = 0; i < 5; i++)
                {
                    if (item is BaseHat)
                    {
                        switch (i)
                        {
                            case 0: ((BaseHat)item).Resistances.Physical += resists[i]; break;
                            case 1: ((BaseHat)item).Resistances.Fire += resists[i]; break;
                            case 2: ((BaseHat)item).Resistances.Cold += resists[i]; break;
                            case 3: ((BaseHat)item).Resistances.Poison += resists[i]; break;
                            case 4: ((BaseHat)item).Resistances.Energy += resists[i]; break;
                        }
                    }
                    else
                        if (item is BaseArmor)
                        {
                            switch (i)
                            {
                                case 0: ((BaseArmor)item).PhysicalBonus += resists[i]; break;
                                case 1: ((BaseArmor)item).FireBonus += resists[i]; break;
                                case 2: ((BaseArmor)item).ColdBonus += resists[i]; break;
                                case 3: ((BaseArmor)item).PoisonBonus += resists[i]; break;
                                case 4: ((BaseArmor)item).EnergyBonus += resists[i]; break;
                            }
                        }
                }
            }
        }

        #region SE definitions
        public static readonly LootPack SePoor = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,                        100.00, "2d10+20" ),
                new LootPackEntry( false, AosMagicItemsPoor,          1.00, 1, 5, 0, 100 ),
                new LootPackEntry( false, Instruments,                  0.02, 1 )
            } );

        public static readonly LootPack SeMeager = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,                        100.00, "4d10+40" ),
                new LootPackEntry( false, AosMagicItemsMeagerType1,     20.40, 1, 2, 0, 50 ),
                new LootPackEntry( false, AosMagicItemsMeagerType2,     10.20, 1, 5, 0, 100 ),
                new LootPackEntry( false, Instruments,                  0.10, 1 )
            } );

        public static readonly LootPack SeAverage = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,                        100.00, "8d10+100" ),
                new LootPackEntry( false, AosMagicItemsAverageType1, 32.80, 1, 3, 0, 50 ),
                new LootPackEntry( false, AosMagicItemsAverageType1, 32.80, 1, 4, 0, 75 ),
                new LootPackEntry( false, AosMagicItemsAverageType2, 19.50, 1, 5, 0, 100 ),
                new LootPackEntry( false, Instruments,                  0.40, 1 )
            } );

        public static readonly LootPack SeRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,                        100.00, "15d10+225" ),
                new LootPackEntry( false, AosMagicItemsRichType1,     76.30, 1, 4, 0, 75 ),
                new LootPackEntry( false, AosMagicItemsRichType1,     76.30, 1, 4, 0, 75 ),
                new LootPackEntry( false, AosMagicItemsRichType2,     61.70, 1, 5, 0, 100 ),
                new LootPackEntry( false, Instruments,                  1.00, 1 )
            } );

        public static readonly LootPack SeFilthyRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,                           100.00, "3d100+400" ),
                new LootPackEntry( false, AosMagicItemsFilthyRichType1,    79.50, 1, 5, 0, 100 ),
                new LootPackEntry( false, AosMagicItemsFilthyRichType1,    79.50, 1, 5, 0, 100 ),
                new LootPackEntry( false, AosMagicItemsFilthyRichType2,    77.60, 1, 5, 25, 100 ),
                new LootPackEntry( false, Instruments,                     2.00, 1 )
            } );

        public static readonly LootPack SeUltraRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,                        100.00, "6d100+600" ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, Instruments,                  2.00, 1 )
            } );

        public static readonly LootPack SeSuperBoss = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,                        100.00, "10d100+800" ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 50, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 50, 100 ),
                new LootPackEntry( false, Instruments,                  2.00, 1 )
            } );
        #endregion

        #region AOS definitions
        public static readonly LootPack AosPoor = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "1d10+10" ),
                new LootPackEntry( false, AosMagicItemsPoor,      0.02, 1, 5, 0, 90 ),
                new LootPackEntry( false, Instruments,      0.02, 1 )
            } );

        public static readonly LootPack AosMeager = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "3d10+20" ),
                new LootPackEntry( false, AosMagicItemsMeagerType1,      1.00, 1, 2, 0, 10 ),
                new LootPackEntry( false, AosMagicItemsMeagerType2,      0.20, 1, 5, 0, 90 ),
                new LootPackEntry( false, Instruments,      0.10, 1 )
            } );

        public static readonly LootPack AosAverage = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "5d10+50" ),
                new LootPackEntry( false, AosMagicItemsAverageType1,  5.00, 1, 4, 0, 20 ),
                new LootPackEntry( false, AosMagicItemsAverageType1,  2.00, 1, 3, 0, 50 ),
                new LootPackEntry( false, AosMagicItemsAverageType2,  0.50, 1, 5, 0, 90 ),
                new LootPackEntry( false, Instruments,      0.40, 1 )
            } );

        public static readonly LootPack AosRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "10d10+150" ),
                new LootPackEntry( false, AosMagicItemsRichType1,     20.00, 1, 4, 0, 40 ),
                new LootPackEntry( false, AosMagicItemsRichType1,     10.00, 1, 5, 0, 60 ),
                new LootPackEntry( false, AosMagicItemsRichType2,      1.00, 1, 5, 0, 90 ),
                new LootPackEntry( false, Instruments,      1.00, 1 )
            } );

        public static readonly LootPack AosFilthyRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "2d100+200" ),
                new LootPackEntry( false, AosMagicItemsFilthyRichType1,     33.00, 1, 4, 0, 50 ),
                new LootPackEntry( false, AosMagicItemsFilthyRichType1,     33.00, 1, 4, 0, 60 ),
                new LootPackEntry( false, AosMagicItemsFilthyRichType2,     20.00, 1, 5, 0, 75 ),
                new LootPackEntry( false, AosMagicItemsFilthyRichType2,      5.00, 1, 5, 0, 100 ),
                new LootPackEntry( false, Instruments,      2.00, 1 )
            } );

        public static readonly LootPack AosUltraRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "5d100+500" ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 35, 100 ),
                new LootPackEntry( false, Instruments,      2.00, 1 )
            } );

        public static readonly LootPack AosSuperBoss = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "5d100+500" ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 25, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 33, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 50, 100 ),
                new LootPackEntry( false, AosMagicItemsUltraRich,    100.00, 1, 5, 50, 100 ),
                new LootPackEntry( false, Instruments,      2.00, 1 )
            } );
        #endregion

        #region Pre-AOS definitions
        public static readonly LootPack OldPoor = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "1d25" ),
                new LootPackEntry( false, Instruments,      0.02, 1 )
            } );

        public static readonly LootPack OldMeager = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "5d10+25" ),
                new LootPackEntry( false, Instruments,      0.10, 1 ),
                new LootPackEntry( false, OldMagicItems,  1.00, 1, 1, 0, 60 ),
                new LootPackEntry( false, OldMagicItems,  0.20, 1, 1, 10, 70 )
            } );

        public static readonly LootPack OldAverage = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "10d10+50" ),
                new LootPackEntry( false, Instruments,      0.40, 1 ),
                new LootPackEntry( false, OldMagicItems,  5.00, 1, 1, 20, 80 ),
                new LootPackEntry( false, OldMagicItems,  2.00, 1, 1, 30, 90 ),
                new LootPackEntry( false, OldMagicItems,  0.50, 1, 1, 40, 100 )
            } );

        public static readonly LootPack OldRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "10d10+250" ),
                new LootPackEntry( false, Instruments,      1.00, 1 ),
                new LootPackEntry( false, OldMagicItems, 20.00, 1, 1, 60, 100 ),
                new LootPackEntry( false, OldMagicItems, 10.00, 1, 1, 65, 100 ),
                new LootPackEntry( false, OldMagicItems,  1.00, 1, 1, 70, 100 )
            } );

        public static readonly LootPack OldFilthyRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "2d125+400" ),
                new LootPackEntry( false, Instruments,      2.00, 1 ),
                new LootPackEntry( false, OldMagicItems, 33.00, 1, 1, 50, 100 ),
                new LootPackEntry( false, OldMagicItems, 33.00, 1, 1, 60, 100 ),
                new LootPackEntry( false, OldMagicItems, 20.00, 1, 1, 70, 100 ),
                new LootPackEntry( false, OldMagicItems,  5.00, 1, 1, 80, 100 )
            } );

        public static readonly LootPack OldUltraRich = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "5d100+500" ),
                new LootPackEntry( false, Instruments,      2.00, 1 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 40, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 40, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 50, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 50, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 60, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 60, 100 )
            } );

        public static readonly LootPack OldSuperBoss = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry(  true, Gold,            100.00, "5d100+500" ),
                new LootPackEntry( false, Instruments,      2.00, 1 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 40, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 40, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 40, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 50, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 50, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 50, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 60, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 60, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 60, 100 ),
                new LootPackEntry( false, OldMagicItems,    100.00, 1, 1, 70, 100 )
            } );
        #endregion

        #region Generic accessors
        public static LootPack Poor{ get{ return Core.AOS ? AosPoor : OldPoor; } }
        public static LootPack Meager{ get{ return Core.AOS ? AosMeager : OldMeager; } }
        public static LootPack Average{ get{ return Core.AOS ? AosAverage : OldAverage; } }
        public static LootPack Rich{ get{ return Core.AOS ? AosRich : OldRich; } }
        public static LootPack FilthyRich{ get{ return Core.AOS ? AosFilthyRich : OldFilthyRich; } }
        public static LootPack UltraRich{ get{ return Core.AOS ? AosUltraRich : OldUltraRich; } }
        public static LootPack SuperBoss{ get{ return Core.AOS ? AosSuperBoss : OldSuperBoss; } }
        #endregion

        public static readonly LootPack LowScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, LowScrollItems,    100.00, 1 )
            } );

        public static readonly LootPack MedScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, MedScrollItems,    100.00, 1 )
            } );

        public static readonly LootPack HighScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, HighScrollItems,    100.00, 1 )
            } );

        public static readonly LootPack Gems = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, GemItems,            100.00, 1 )
            } );

        public static readonly LootPack Potions = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, PotionItems,        100.00, 1 )
            } );
        public static readonly LootPack AvatarScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, AvatarScrollItems, 30.00, 1 )
            } );
            public static readonly LootPack DruidScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, DruidScrollItems, 30.00, 1 )
            } );
            public static readonly LootPack UndeadScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, UndeadScrollItems, 30.00, 1 )
            } );
            public static readonly LootPack RangerScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, RangerScrollItems, 30.00, 1 )
            } );
            public static readonly LootPack ClericScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, ClericScrollItems, 30.00, 1 )
            } );
             public static readonly LootPack DeathKnightScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, DeathKnightItems, 30.00, 1 )
            } );
             public static readonly LootPack  RogueScrollScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false,  RogueScrollItems, 30.00, 1 )
            } );
           
            public static readonly LootPack BardScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, BardScrollItems, 30.00, 1 )
            } );
                        public static readonly LootPack AncientScrolls = new LootPack( new LootPackEntry[]
            {
                new LootPackEntry( false, AncientScrollItems, 30.00, 1 )
            } );
    }

    // 11.10.2012 :: zombie :: dodanie MinProps
    public class LootPackEntry
    {
        private int m_Chance;
        private LootPackDice m_Quantity;

        private int m_MinProps, m_MaxProps, m_MinIntensity, m_MaxIntensity;
        private int m_Lvl;

        private bool m_AtSpawnTime;

        private LootPackItem[] m_Items;

        public int Lvl
        {
            get{ return m_Lvl; }
            //set{ m_Lvl = value; }
        }

        public int Chance
        {
            get{ return m_Chance; }
            set{ m_Chance = value; }
        }

        public LootPackDice Quantity
        {
            get{ return m_Quantity; }
            set{ m_Quantity = value; }
        }

       public int MinProps
        {
            get{ return m_MinProps; }
            set{ m_MinProps = value; }
        }

        public int MaxProps
        {
            get{ return m_MaxProps; }
            set{ m_MaxProps = value; }
        }

        public int MinIntensity
        {
            get{ return m_MinIntensity; }
            set{ m_MinIntensity = value; }
        }

        public int MaxIntensity
        {
            get{ return m_MaxIntensity; }
            set{ m_MaxIntensity = value; }
        }

        public LootPackItem[] Items
        {
            get{ return m_Items; }
            set{ m_Items = value; }
        }

        public Item Construct( Mobile from, int luckChance, bool spawning )
        {
            if ( m_AtSpawnTime != spawning )
                return null;

            int totalChance = 0;

            for ( int i = 0; i < m_Items.Length; ++i )
                totalChance += m_Items[i].Chance;

            int rnd = Utility.Random( totalChance );

            for ( int i = 0; i < m_Items.Length; ++i )
            {
                LootPackItem item = m_Items[i];

                if ( rnd < item.Chance )
                    return Mutate( from, luckChance, item.Construct() );

                rnd -= item.Chance;
            }

            return null;
        }

        private int GetRandomOldBonus() // niewazne pre-AOS
        {
            int rnd = Utility.RandomMinMax( m_MinIntensity, m_MaxIntensity );

            if ( 50 > rnd )
                return 1;
            else
                rnd -= 50;

            if ( 25 > rnd )
                return 2;
            else
                rnd -= 25;

            if ( 14 > rnd )
                return 3;
            else
                rnd -= 14;

            if ( 8 > rnd )
                return 4;

            return 5;
        }

        public Item Mutate(Mobile from, int luckChance, Item item)
        {
            if ( item != null )
            {
				if( item is Spellbook )
				{
					// Blessed doesn't appear in corpses. Here is a part of a workaround.
					// The other part is in Spellbook.cs, OnDragLift().
					((Spellbook)item).LootType = LootType.Regular;
				}

                if ( item is BaseWeapon && 1 > Utility.Random( 100 ) )
                {
                    item.Delete();
                    item = new FireHorn();
                    return item;
                }

                if ( item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat )
                {
                    if ( Core.AOS )
                    {
                        // 11.10.2012 :: zombie :: zamiast GetBonusProperties
                        int props = Utility.RandomMinMax( m_MinProps, m_MaxProps );

                        int min = m_MinIntensity;
                        int max = m_MaxIntensity;

                        if ( props < LootPack.MaxProps && LootPack.CheckLuck( luckChance ) )
                            props++;
                        
                        // Make sure we're not spawning items with 6 properties.
                        if ( props > LootPack.MaxProps )
                            props = LootPack.MaxProps;
                        // zombie

                        if ( item is BaseWeapon )
                            BaseRunicTool.ApplyAttributesTo( (BaseWeapon)item, false, luckChance, props, m_MinIntensity, m_MaxIntensity, LootPack.RandomizerForItemLevel(m_Lvl) );
                        else if ( item is BaseArmor )
                            BaseRunicTool.ApplyAttributesTo( (BaseArmor)item, false, luckChance, props, m_MinIntensity, m_MaxIntensity, LootPack.RandomizerForItemLevel(m_Lvl) );
                        else if ( item is BaseJewel )
                            BaseRunicTool.ApplyAttributesTo( (BaseJewel)item, false, luckChance, props, m_MinIntensity, m_MaxIntensity, LootPack.RandomizerForItemLevel(m_Lvl) );
                        else if ( item is BaseHat )
                            BaseRunicTool.ApplyAttributesTo( (BaseHat)item, false, luckChance, props, m_MinIntensity, m_MaxIntensity, LootPack.RandomizerForItemLevel(m_Lvl) );

                        // Dodatkowe resy dla zbroi z loot:
                        LootPack.GiveAdditionalResists(item, m_Lvl);
                    }
                    else // not aos
                    {
                        # region not_AOS
                        if ( item is BaseWeapon )
                        {
                            BaseWeapon weapon = (BaseWeapon)item;

                            if ( 80 > Utility.Random( 100 ) )
                                weapon.AccuracyLevel = (WeaponAccuracyLevel)GetRandomOldBonus();

                            if ( 60 > Utility.Random( 100 ) )
                                weapon.DamageLevel = (WeaponDamageLevel)GetRandomOldBonus();

                            if ( 40 > Utility.Random( 100 ) )
                                weapon.DurabilityLevel = (WeaponDurabilityLevel)GetRandomOldBonus();

                            if ( 5 > Utility.Random( 100 ) )
                                weapon.Slayer = SlayerName.Silver;

                            if ( from != null && weapon.AccuracyLevel == 0 && weapon.DamageLevel == 0 && weapon.DurabilityLevel == 0 && weapon.Slayer == SlayerName.None && 5 > Utility.Random( 100 ) )
                                weapon.Slayer = SlayerGroup.GetLootSlayerType( from.GetType() );
                        }
                        else if ( item is BaseArmor )
                        {
                            BaseArmor armor = (BaseArmor)item;

                            if ( 80 > Utility.Random( 100 ) )
                                armor.ProtectionLevel = (ArmorProtectionLevel)GetRandomOldBonus();

                            if ( 40 > Utility.Random( 100 ) )
                                armor.Durability = (ArmorDurabilityLevel)GetRandomOldBonus();
                        }
                        #endregion
                    }
                }
                else if ( item is BaseInstrument )
                {
                    SlayerName slayer = SlayerName.None;

                    if ( Core.AOS )
                        slayer = BaseRunicTool.GetRandomSlayer();
                    else
                        slayer = SlayerGroup.GetLootSlayerType( from.GetType() );

                    if ( slayer == SlayerName.None )
                    {
                        item.Delete();
                        return null;
                    }

                    BaseInstrument instr = (BaseInstrument)item;

                    instr.Quality = InstrumentQuality.Regular;
                    instr.Slayer = slayer;
                }

                if ( item.Stackable )
                    item.Amount = m_Quantity.Roll();
            }

            return item;
        }

         public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, string quantity ) : this( atSpawnTime, items, chance, new LootPackDice( quantity ), 0, 0, 0 )
        {
        }

        public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, int quantity ) : this( atSpawnTime, items, chance, new LootPackDice( 0, 0, quantity ), 0, 0, 0 )
        {
        }

        public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, string quantity, int maxProps, int minIntensity, int maxIntensity ) : this( atSpawnTime, items, chance, new LootPackDice( quantity ), maxProps, minIntensity, maxIntensity )
        {
        }

        public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, int quantity, int maxProps, int minIntensity, int maxIntensity ) : this( atSpawnTime, items, chance, new LootPackDice( 0, 0, quantity ), maxProps, minIntensity, maxIntensity )
        {
        }

        public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, LootPackDice quantity, int maxProps, int minIntensity, int maxIntensity ) : this( atSpawnTime, items, chance, quantity, 1, maxProps, minIntensity, maxIntensity )
        {
        }

        public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, int quantity, int minProps, int maxProps, int minIntensity, int maxIntensity, int level ) : this( atSpawnTime, items, chance, new LootPackDice( 0, 0, quantity ), minProps, maxProps, minIntensity, maxIntensity, level )
        {
        }

        public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, LootPackDice quantity, int minProps, int maxProps, int minIntensity, int maxIntensity) : this( atSpawnTime, items, chance, quantity, minProps, maxProps, minIntensity, maxIntensity, 0 )
        {
        }

        public LootPackEntry( bool atSpawnTime, LootPackItem[] items, double chance, LootPackDice quantity, int minProps, int maxProps, int minIntensity, int maxIntensity, int level )
        {
            m_AtSpawnTime = atSpawnTime;
            m_Items = items;
            m_Chance = (int)(100 * chance);
            m_Quantity = quantity;
            m_MinProps = minProps;
            m_MaxProps = maxProps;
            m_MinIntensity = minIntensity;
            m_MaxIntensity = maxIntensity;
            m_Lvl = level;
        }

        public int GetBonusProperties() // nieuzywane
        {
            int p0=0, p1=0, p2=0, p3=0, p4=0, p5=0;

            switch ( m_MaxProps )
            {
                case 1: p0= 3; p1= 1; break;
                case 2: p0= 6; p1= 3; p2= 1; break;
                case 3: p0=10; p1= 6; p2= 3; p3= 1; break;
                case 4: p0=16; p1=12; p2= 6; p3= 5; p4=1; break;
                case 5: p0=30; p1=25; p2=20; p3=15; p4=9; p5=1; break;
            }

            int pc = p0+p1+p2+p3+p4+p5;

            int rnd = Utility.Random( pc );

            if ( rnd < p5 )
                return 5;
            else
                rnd -= p5;

            if ( rnd < p4 )
                return 4;
            else
                rnd -= p4;

            if ( rnd < p3 )
                return 3;
            else
                rnd -= p3;

            if ( rnd < p2 )
                return 2;
            else
                rnd -= p2;

            if ( rnd < p1 )
                return 1;

            return 0;
        }
    }

    public class LootPackItem
    {
        private Type m_Type;
        private int m_Chance;

        public Type Type
        {
            get{ return m_Type; }
            set{ m_Type = value; }
        }

        public int Chance
        {
            get{ return m_Chance; }
            set{ m_Chance = value; }
        }

        private static Type[]   m_BlankTypes = new Type[]{ typeof( BlankScroll ) };
        private static Type[][] m_NecroTypes = new Type[][]
            {
                new Type[] // low
                {
                    typeof( AnimateDeadScroll ),        typeof( BloodOathScroll ),        typeof( CorpseSkinScroll ),    typeof( CurseWeaponScroll ),
                    typeof( EvilOmenScroll ),            typeof( HorrificBeastScroll ),    typeof( MindRotScroll ),    typeof( PainSpikeScroll ),
                    typeof( SummonFamiliarScroll ),        typeof( WraithFormScroll )
                },
                new Type[] // med
                {
                    typeof( LichFormScroll ),            typeof( PoisonStrikeScroll ),    typeof( StrangleScroll ),    typeof( WitherScroll )
                },

                ((Core.SE) ?
                new Type[] // high
                {
                    typeof( VengefulSpiritScroll ),        typeof( VampiricEmbraceScroll ), typeof( ExorcismScroll )
                } : 
                new Type[] // high
                {
                    typeof( VengefulSpiritScroll ),        typeof( VampiricEmbraceScroll )
                })
            };

        public static Item RandomScroll( int index, int minCircle, int maxCircle )
        {
            --minCircle;
            --maxCircle;

            int scrollCount = ((maxCircle - minCircle) + 1) * 8;

            if ( index == 0 )
                scrollCount += m_BlankTypes.Length;

            if ( Core.AOS )
                scrollCount += m_NecroTypes[index].Length;

            int rnd = Utility.Random( scrollCount );

            if ( index == 0 && rnd < m_BlankTypes.Length )
                return Loot.Construct( m_BlankTypes );
            else if ( index == 0 )
                rnd -= m_BlankTypes.Length;

            if ( Core.AOS && rnd < m_NecroTypes.Length )
                return Loot.Construct( m_NecroTypes[index] );
            else if ( Core.AOS )
                rnd -= m_NecroTypes[index].Length;

            return Loot.RandomScroll( minCircle * 8, (maxCircle * 8) + 7, SpellbookType.Regular );
        }

        public Item Construct()
        {
            try
            {
                Item item;

                if ( m_Type == typeof( BaseRanged ) )
                    item = Loot.RandomRangedWeapon();
                else if ( m_Type == typeof( BaseWeapon ) )
                    item = Loot.RandomWeapon();
                //else if ( m_Type == typeof( BaseArmor ) )
                //    item = Loot.RandomArmorOrHat();
                else if ( m_Type == typeof( BaseArmor ) )
                    item = Loot.RandomArmor();
                else if ( m_Type == typeof( BaseHat ) )
                    item = Loot.RandomHat();
                else if ( m_Type == typeof( BaseShield ) )
                    item = Loot.RandomShield();
                else if ( m_Type == typeof( BaseJewel ) )
                    item = Core.AOS ? Loot.RandomJewelry() : Loot.RandomArmorOrShieldOrWeapon();
                else if ( m_Type == typeof( BaseInstrument ) )
                    item = Loot.RandomInstrument();
                else if ( m_Type == typeof( Amber ) ) // gem
                    item = Loot.RandomGem();
                else
                    item = Activator.CreateInstance( m_Type ) as Item;

                return item;
            }
            catch
            {
            }

            return null;
        }

        public LootPackItem( Type type, int chance )
        {
            m_Type = type;
            m_Chance = chance;
        }
    }

    public class LootPackDice
    {
        private int m_Count, m_Sides, m_Bonus;

        public int Count
        {
            get{ return m_Count; }
            set{ m_Count = value; }
        }

        public int Sides
        {
            get{ return m_Sides; }
            set{ m_Sides = value; }
        }

        public int Bonus
        {
            get{ return m_Bonus; }
            set{ m_Bonus = value; }
        }

        // rzut kostka m_Count razy, zwraca sume oczek + m_Bonus
        public int Roll()
        {
            int v = m_Bonus;

            for ( int i = 0; i < m_Count; ++i )
                v += Utility.Random( 1, m_Sides );

            return v;
        }

        // parsowanie stringa: XdY+Z ---> X=ilosc rzutow, Y=ilosc scianek kostki, Z=bonus do sumy oczek (moze byc ujemny)
        // <m_Count>d<m_Sides>[+-]<m_Bonus>
        public LootPackDice( string str )
        {
            int start = 0;
            int index = str.IndexOf( 'd', start );

            if ( index < start )
                return;

            m_Count = Utility.ToInt32( str.Substring( start, index-start ) );
            bool negative;

            start = index + 1;
            index = str.IndexOf( '+', start );

            if ( negative = (index < start) )
                index = str.IndexOf( '-', start );

            if ( index < start )
                index = str.Length;

            m_Sides = Utility.ToInt32( str.Substring( start, index-start ) );

            if ( index == str.Length )
                return;

            start = index + 1;
            index = str.Length;

            m_Bonus = Utility.ToInt32( str.Substring( start, index-start ) );
            
            if ( negative )
                m_Bonus *= -1;
        }

        public LootPackDice( int count, int sides, int bonus )
        {
            m_Count = count;    // ilosc rzutow
            m_Sides = sides;    // ilosc scianek (ilosc oczek na sciankach: 1, 2, 3 ... m_Sides-1)
            m_Bonus = bonus;    // bonus do otrzymanej sumy oczek po wszystkich rzutach
        }
    }
}