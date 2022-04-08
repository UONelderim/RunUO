using System; 
using System.Collections;
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
    public class HireRanger : BaseHire 
    { 
        [Constructable] 
        public HireRanger(): base( AIType.AI_Archer )
        { 
        	Title = "- najemnik";
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
			
		    SetSkill( SkillName.Wrestling, 15, 37 ); 
		    SetSkill( SkillName.Parry, 45, 60 ); 
		    SetSkill( SkillName.Archery, 66, 97 );

		    SetSkill( SkillName.Swords, 35, 57 ); 
		    SetSkill( SkillName.Fencing, 15, 37 );
		    SetSkill( SkillName.Tactics, 65, 87 );
SetSkill( SkillName.Healing, 65, 87 );			

            Fame = 100; 
            Karma = 125; 

            AddItem( new Shoes( Utility.RandomNeutralHue() ) ); 
            AddItem( new Shirt()); 

            // Pick a random sword
            switch ( Utility.Random( 3 )) 
            { 
		      case 0: AddItem( new Bow() ); break;
		      case 1: AddItem( new CompositeBow() ); break;
		      case 2: AddItem( new Yumi() ); break;
            } 

              AddItem( new RangerChest() );
              AddItem( new RangerArms() );
              AddItem( new RangerGloves() );
              AddItem( new RangerGorget() );
              AddItem( new RangerLegs() );

		    PackItem ( new Arrow( 250 ) );
            PackGold( 10, 75 ); 
        
        }
        
            
	public override bool ClickTitle{ get{ return false; } }
        public HireRanger( Serial serial ) : base( serial ) 
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
