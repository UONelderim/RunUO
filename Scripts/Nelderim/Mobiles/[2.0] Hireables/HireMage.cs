using System; 
using System.Collections;
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
    public class HireMage : BaseHire
    { 
        [Constructable] 
       public HireMage() : base( AIType.AI_Mage ) 
        {
            SpeechHue = Utility.RandomDyedHue(); 
            Hue = Utility.RandomSkinHue(); 
		    Title = "- mag";
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
			
			SetResistance( ResistanceType.Physical, 30, 30 );
			SetResistance( ResistanceType.Fire, 30, 30 );
			SetResistance( ResistanceType.Cold, 30, 30 );
			SetResistance( ResistanceType.Poison, 30, 30 );
			SetResistance( ResistanceType.Energy, 30, 30 );

		SetSkill( SkillName.EvalInt, 100.0, 125 );
		SetSkill( SkillName.Magery, 100, 125 );
		SetSkill( SkillName.Meditation, 100, 125 );
		SetSkill( SkillName.MagicResist, 100, 125 );
		SetSkill( SkillName.Tactics, 100, 125 );
		SetSkill( SkillName.Macing, 100, 125 ); 

            Fame = 100; 
            Karma = 100; 

            AddItem( new Shoes( Utility.RandomNeutralHue() ) ); 
            AddItem( new Shirt()); 

			AddItem( new Robe( Utility.RandomNeutralHue() ) );
			AddItem( new ThighBoots() );


            PackGold( 20, 100 ); 
        } 
	public override bool ClickTitle{ get{ return false; } }
        public HireMage( Serial serial ) : base( serial ) 
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
