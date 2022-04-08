using System; 
using System.Collections;
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
    public class HireBardArcher : BaseHire 
    { 
        [Constructable] 
        public HireBardArcher() :  base( AIType.AI_Archer ) 
        { 
            SpeechHue = Utility.RandomDyedHue(); 
            Hue = Utility.RandomSkinHue(); 

            if ( this.Female = Utility.RandomBool() ) 
            { 
                Body = 0x191; 
                Name = NameList.RandomName( "female" ); 

		switch ( Utility.Random ( 2 ) )
		{
			case 0: AddItem( new Skirt ( Utility.RandomDyedHue() ) ); break;
			case 1: AddItem( new Kilt ( Utility.RandomNeutralHue() ) ); break;
		}
            }
             
            else 
            { 
                Body = 0x190; 
                Name = NameList.RandomName( "male" ); 
                AddItem( new ShortPants( Utility.RandomNeutralHue() ) ); 
            } 
		Title = "- bard";

			
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
			SetResistance( ResistanceType.Energy, 30, 40 );

		SetSkill( SkillName.Tactics, 35, 57 ); 

		SetSkill( SkillName.Swords, 45, 67 );
		SetSkill( SkillName.Archery, 36, 67 );
		SetSkill( SkillName.Parry, 45, 60 );
		SetSkill( SkillName.Musicianship, 66.0, 97.5 );
		SetSkill( SkillName.Peacemaking, 65.0, 87.5 );

            Fame = 100; 
            Karma = 100; 

            AddItem( new Shoes( Utility.RandomNeutralHue() ) ); 

		switch ( Utility.Random( 2 ) )
		{
			case 0: AddItem( new Doublet( Utility.RandomDyedHue() ) ); break;
			case 1: AddItem( new Shirt( Utility.RandomDyedHue() ) ); break;
		}
		switch ( Utility.Random( 4 ) )
		{
			case 0: PackItem( new Harp() ); break;
			case 1: PackItem( new Lute() ); break;
			case 2: PackItem( new Drums() ); break;
			case 3: PackItem( new Tambourine() ); break;
		}

			PackItem( new Longsword() );
			AddItem( new Bow() );
			PackItem( new Arrow(100) );
            PackGold( 10, 50 ); 
        
        }
	public override bool ClickTitle{ get{ return false; } }
        public HireBardArcher( Serial serial ) : base( serial ) 
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
