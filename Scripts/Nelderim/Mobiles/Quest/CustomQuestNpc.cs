using System;
using Server.Items;
using Server;
using Server.Misc;

namespace Server.Mobiles
{

	public class CustomQuestNpc : BaseCreature
	{
        [CommandProperty(AccessLevel.Counselor)]
        public String qName
        {
            get { return Name; }
            set { Name = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public String qTitle
        {
            get { return Title; }
            set { Title = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public String qLabel1
        {
            get { return Label1; }
            set { Label1 = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public String qLabel2
        {
            get { return Label2; }
            set { Label2 = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool qBlessed
        {
            get { return Blessed; }
            set { Blessed = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public Race qRace
        {
            get { return Race; }
            set { Race = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool qCantWalk
        {
            get { return CantWalk; }
            set { CantWalk = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public int qKills
        {
            get { return Kills; }
            set { Kills = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool qFemale
        {
            get { return Female; }
            set { Female = value; }
        }

        [Constructable]
		public CustomQuestNpc()
			: base( AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			InitStats( 100, 100, 100 );

			SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.MagicResist, 60);
            SetSkill(SkillName.DetectHidden, 0);

            CantWalk = true;

            SpeechHue = Utility.RandomDyedHue();
			
			Hue = Utility.RandomSkinHue();

			if( this.Female = Utility.RandomBool() )
			{
				this.Body = 0x191;
				this.Name = NameList.RandomName( "female" );
			}
			else
			{
				this.Body = 0x190;
				this.Name = NameList.RandomName( "male" );
			}
			AddItem( new Doublet( Utility.RandomDyedHue() ) );
			AddItem( new Sandals( Utility.RandomNeutralHue() ) );
			AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			AddItem( new HalfApron( Utility.RandomDyedHue() ) );

			Utility.AssignRandomHair( this );

			Container pack = new Backpack();

			//pack.DropItem( new Gold( 250, 300 ) );

			pack.Movable = false;

			AddItem( pack );
		}

		public void ReplaceCloth(Item wearable, PlayerMobile provider)
		{
            if (wearable is BaseClothing || wearable is BaseWeapon || wearable is BaseArmor || wearable is Spellbook || wearable is BaseHarvestTool)
			{
				Item replaced = FindItemOnLayer(wearable.Layer);
				if (replaced != null)
				{
					provider.Backpack.DropItem(replaced);

					provider.SendMessage("Ciuszek z NPC l¹duje w twoim plecaku.");
				}

				AddItem(wearable);
			}
        }

        public void RemoveCloth(Item worn, PlayerMobile taker)
        {
            if (worn != null && worn.Parent == this)
            {
                taker.Backpack.DropItem(worn);

                taker.SendMessage("Ciuszek z NPC l¹duje w twoim plecaku.");
            }
        }

        public override bool ClickTitle { get { return false; } }


		public CustomQuestNpc( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
