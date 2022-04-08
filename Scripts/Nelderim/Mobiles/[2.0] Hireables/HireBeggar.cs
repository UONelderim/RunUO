using System; 
using System.Collections;
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
    public class HireBeggar : BaseHire 
    { 
        [Constructable] 
        public HireBeggar():  base( AIType.AI_NecroMage ) 
        { 
            SpeechHue = Utility.RandomDyedHue(); 
            Hue = Utility.RandomSkinHue(); 

            if ( this.Female = Utility.RandomBool() ) 
            { 
                Body = 0x191; 
                Name = NameList.RandomName( "female" ); 

		        switch ( Utility.Random ( 1 ) )
		        {
			      case 0: AddItem( new HoodedShroudOfShadows ( Utility.RandomNeutralHue() ) ); break;
			      
		        }
            }
             
            else 
            { 
                Body = 0x190; 
                Name = NameList.RandomName( "male" ); 
                AddItem( new HoodedShroudOfShadows( Utility.RandomNeutralHue() ) ); 
            }
            
		     Title = "- mnich";
         	
			 Utility.AssignRandomHair( this );			
			
            SetStr( 496, 525 );
			SetDex( 86, 105 );
			SetInt( 86, 125 );

			SetHits( 298, 315 );

			SetDamage( 16, 22 ); 
			
			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Fire, 20, 20 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 50 );

		SetSkill( SkillName.Begging, 66, 97 ); 
		SetSkill( SkillName.Tactics, 55, 77 ); 
		SetSkill( SkillName.Wrestling, 55, 77 ); 
		SetSkill( SkillName.Magery, 65, 77 );	
SetSkill( SkillName.Spellweaving, 65, 100 );	
	

            Fame = 0; 
            Karma = 0; 

            AddItem( new Sandals( Utility.RandomNeutralHue() ) ); 

		switch ( Utility.Random( 2 ) )
		{
			case 0: AddItem( new Doublet( Utility.RandomNeutralHue() ) ); break;
			case 1: AddItem( new Shirt( Utility.RandomNeutralHue() ) ); break;
		}
		
            PackGold( 0, 25 ); 
        
        }
        
	public override bool ClickTitle{ get{ return false; } }
        public HireBeggar( Serial serial ) : base( serial ) 
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
