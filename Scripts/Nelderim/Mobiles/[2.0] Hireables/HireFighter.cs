using System; 
using System.Collections;
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
    public class HireFighter : BaseHire 
    { 
        [Constructable] 
        public HireFighter():  base( AIType.AI_Boss ) 
        { 
            SpeechHue = Utility.RandomDyedHue(); 
            Hue = Utility.RandomSkinHue(); 

            if ( this.Female = Utility.RandomBool() ) 
            { 
                Body = 0x191; 
                Name = NameList.RandomName( "female" ); 
            } 
            else 
            { 
                Body = 0x190; 
                Name = NameList.RandomName( "male" ); 
                AddItem( new ShortPants( Utility.RandomNeutralHue() ) ); 
            } 
		     Title = "- wojownik";
            
            
    
         
		
			
			Utility.AssignRandomHair( this );			
			
		

            SetStr( 496, 525 );
			SetDex( 86, 105 );
			SetInt( 86, 125 );

			SetHits( 298, 315 );

			SetDamage( 16, 22 );
			
			SetResistance( ResistanceType.Physical, 30, 50 );
			SetResistance( ResistanceType.Fire, 30, 50 );
			SetResistance( ResistanceType.Cold, 30, 50 );
			SetResistance( ResistanceType.Poison, 30, 50 );
			SetResistance( ResistanceType.Energy, 30, 50 );

		SetSkill( SkillName.Tactics, 36, 67 );
		SetSkill( SkillName.Magery, 22, 22 );
		SetSkill( SkillName.Swords, 64, 100 );
		SetSkill( SkillName.Parry, 60, 82 );
		SetSkill( SkillName.Macing, 36, 67 );
		SetSkill( SkillName.Focus, 36, 67 );
		SetSkill( SkillName.Wrestling, 25, 47 );
        SetSkill( SkillName.Healing, 60, 90 );   

            Fame = 100; 
            Karma = 100; 

            AddItem( new Shoes( Utility.RandomNeutralHue() ) ); 
            AddItem( new Shirt()); 

            // Pick a random sword
            switch ( Utility.Random( 5 )) 
            { 
                case 0: AddItem( new Longsword() ); break; 
                case 1: AddItem( new Broadsword() ); break; 
                case 2: AddItem( new VikingSword() ); break; 
		case 3: AddItem( new BattleAxe() ); break;
		case 4: AddItem( new TwoHandedAxe() ); break;
            } 

            // Pick a random shield
            switch ( Utility.Random( 8 )) 
            { 
                case 0: AddItem( new BronzeShield() ); break; 
                case 1: AddItem( new HeaterShield() ); break; 
                case 2: AddItem( new MetalKiteShield() ); break; 
                case 3: AddItem( new MetalShield() ); break; 
                case 4: AddItem( new WoodenKiteShield() ); break; 
                case 5: AddItem( new WoodenShield() ); break; 
		case 6: AddItem( new OrderShield() ); break;
		case 7: AddItem( new ChaosShield() ); break;
            } 
          
		switch( Utility.Random( 5 ) )
		{
			case 0: break;
			case 1: AddItem( new Bascinet() ); break;
			case 2: AddItem( new CloseHelm() ); break;
			case 3: AddItem( new NorseHelm() ); break;
			case 4: AddItem( new Helmet() ); break;

		}
            // Pick some armour
            switch( Utility.Random( 4 ) )
            {
                case 0: // Leather
                    AddItem( new LeatherChest() );
                    AddItem( new LeatherArms() );
                    AddItem( new LeatherGloves() );
                    AddItem( new LeatherGorget() );
                    AddItem( new LeatherLegs() );
                    break;

                case 1: // Studded Leather
                    AddItem( new StuddedChest() );
                    AddItem( new StuddedArms() );
                    AddItem( new StuddedGloves() );
                    AddItem( new StuddedGorget() );
                    AddItem( new StuddedLegs() );
                    break;

                case 2: // Ringmail
                    AddItem( new RingmailChest() );
                    AddItem( new RingmailArms() );
                    AddItem( new RingmailGloves() );
                    AddItem( new RingmailLegs() );
                    break;

                case 3: // Chain
                    AddItem( new ChainChest() );
                    AddItem( new ChainCoif() );
                    AddItem( new ChainLegs() );
                    break;
            }

            PackGold( 25, 100 ); 
        } 
	public override bool ClickTitle{ get{ return false; } }
        public HireFighter( Serial serial ) : base( serial ) 
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
