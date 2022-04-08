using System; 
using System.Collections;
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
    public class HirePeasant : BaseHire 
    { 
        [Constructable] 
        public HirePeasant():  base( AIType.AI_Melee ) 
        { 
            SpeechHue = Utility.RandomDyedHue(); 
            Hue = Utility.RandomSkinHue(); 

            if ( this.Female = Utility.RandomBool() ) 
            { 
                Body = 0x191; 
                Name = NameList.RandomName( "female" ); 

		switch ( Utility.Random ( 2 ) )
		{
			case 0: AddItem( new Skirt ( Utility.RandomNeutralHue() ) ); break;
			case 1: AddItem( new Kilt ( Utility.RandomNeutralHue() ) ); break;
		}
            } 
            else 
            { 
                Body = 0x190; 
                Name = NameList.RandomName( "male" ); 
                AddItem( new ShortPants( Utility.RandomNeutralHue() ) ); 
            } 
		Title = "- wieśniak";
            
        
			
			Utility.AssignRandomHair( this );			
			
		

		AddItem( new Katana() );

           	SetStr( 496, 525 );
			SetDex( 86, 105 );
			SetInt( 86, 125 );

			SetHits( 298, 315 );

			SetDamage( 16, 22 );
			
			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

		SetSkill( SkillName.Tactics, 55, 77 ); 
		SetSkill( SkillName.Wrestling, 55, 77 ); 
		SetSkill( SkillName.Swords, 55, 77 );
		SetSkill( SkillName.Healing, 50, 60);

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
        public HirePeasant( Serial serial ) : base( serial ) 
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
