// ID 0000030
// ID 0000045 
// ID 0000074: JewleryStone dodac koszt za uzycie 

using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    public class JewleryStone : Item
    {
        private int minProperties = 4;
        private int maxProperties = 5;

        private int minIntensity = 80;
        private int maxIntensity = 100;

        private int cost = 0;
        private bool ring = true;

        public int NumberOfProperties = 25; // Ilosc propsow ktore sa mozliwe do wylosowania 

        #region [props
        [CommandProperty( AccessLevel.GameMaster )]
        public int MinProperties
        {
            get
            {
                return minProperties;
            }
            set
            {
                minProperties=(value <= NumberOfProperties ? value : NumberOfProperties);
                if ( minProperties > maxProperties )
                    MaxProperties = minProperties;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxProperties
        {
            get
            {
                return maxProperties;
            }
            set
            {
                maxProperties=(value <= NumberOfProperties ? value : NumberOfProperties);
                if ( maxProperties < minProperties )
                    MinProperties = maxProperties;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MinIntensity
        {
            get
            {
                return minIntensity;
            }
            set
            {
                minIntensity=value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxIntensity
        {
            get
            {
                return maxIntensity;
            }
            set
            {
                maxIntensity=value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost=value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Ring
        {
            get
            {
                return ring;
            }
            set
            {
                ring=value;
            }
        }
        #endregion

        [Constructable]
        public JewleryStone()
            : base( 3796 )
        {
            Name = "kamien bizuterii";
            Movable = false;
        }

        public JewleryStone( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            Container b = from.Backpack;
            if ( b == null )
                return;

            BaseJewel jewel = this.CreateJewel();
            jewel.Identified = true;

            b.DropItem( jewel );
            from.SendMessage( "Bizuteria znalazla sie w Twoim plecaku" );
        }

        public BaseJewel CreateJewel()
        {
            BaseJewel jewel= ( ring ? (BaseJewel)( new GoldRing()) : (BaseJewel)(new GoldBracelet()) );
            int props = Utility.Random( this.maxProperties - this.minProperties ) + this.minProperties;

            BaseRunicTool.ApplyAttributesTo( jewel, props, this.minIntensity, this.maxIntensity );

            return jewel;
        }

        /*
         * Pamiatka jak NIE TWORZYC kodu
        public void AddProperties( BaseJewel jewlery )
        {
            Console.WriteLine( "{0} ({1})", Utility.Random( 1 ) );
            int[] Properties = new int[ this.NumberOfProperties ];
            int NumberOfPropertiesOverMinimum = Utility.Random( maxProperties - minProperties );

            for ( int i = 0; i < NumberOfProperties; ++i )
            {
                Properties[ i ] = i + 1;
            }

            int Random;

            for ( int i = 0; i < (this.minProperties + NumberOfPropertiesOverMinimum); ++i )
            {
                Random = Utility.Random( this.NumberOfProperties - i );

                AddProperty( this.minIntensity, this.maxIntensity, jewlery, Properties[ Random ] );

                Properties[ Random ] = Properties[ (this.NumberOfProperties - (i + 1)) ];
            }
        }

        public void AddProperty( int minIntensity, int maxIntensity, BaseJewel jewlery, int Number )
        {
            float Intensity = ((float) (minIntensity + Utility.Random( (maxIntensity - minIntensity) ))) / 100;
            float SkillValue = (int) (15.0 * Intensity);

            // Console.WriteLine( SkillValue );
            // Console.WriteLine( Intensity );

            switch ( Number )
            {
                // DI
                case 1:
                    jewlery.Attributes.WeaponDamage = (int) Math.Ceiling( 25 * Intensity );
                    break;
                // HCI      
                case 2:
                    jewlery.Attributes.AttackChance = (int) Math.Ceiling( 15 * Intensity );
                    break;
                // DCI
                case 3:
                    jewlery.Attributes.DefendChance = (int) Math.Ceiling( 15 * Intensity );
                    break;
                // LMC
                case 4:
                    jewlery.Attributes.LowerManaCost = (int) Math.Ceiling( 8 * Intensity );
                    break;
                // SDI
                case 5:
                    jewlery.Attributes.SpellDamage = (int) Math.Ceiling( 12 * Intensity );
                    break;
                // FC
                case 6:
                    jewlery.Attributes.CastSpeed = 1;
                    break;
                // FCR
                case 7:
                    jewlery.Attributes.CastRecovery = (int) Math.Ceiling( 3 * Intensity );
                    break;

                // Staty
                // STR
                case 8:
                    jewlery.Attributes.BonusStr = (int) Math.Ceiling( 8 * Intensity );
                    break;
                // DEX
                case 9:
                    jewlery.Attributes.BonusDex = (int) Math.Ceiling( 8 * Intensity );
                    break;
                // INT
                case 10:
                    jewlery.Attributes.BonusInt = (int) Math.Ceiling( 8 * Intensity );
                    break;
                // EP
                case 11:
                    jewlery.Attributes.EnhancePotions = 25;
                    break;

                // Resy
                // AR

                /*
                // Regen
                // Hits
                case 11:
                      jewlery.Attributes.RegenHits = (int) Math.Ceiling( 2 * Intensity );
                      break;
                // Stamina
                case 12:
                      jewlery.Attributes.RegenStam = (int) Math.Ceiling( 3 * Intensity );
                      break;
                // Mana
                case 13:
                      jewlery.Attributes.RegenMana = (int) Math.Ceiling( 2 * Intensity );
                      break;
                
			

            }

            return;
        }
        */

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 3 );

            // 3
            writer.Write( ring );
            // 2
            writer.Write( cost );
            // 1
            writer.Write( minIntensity );
            writer.Write( maxIntensity );
            writer.Write( minProperties );
            writer.Write( maxProperties );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        ring = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        cost = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        minIntensity = reader.ReadInt();
                        maxIntensity = reader.ReadInt();
                        minProperties = reader.ReadInt();
                        maxProperties = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        break;
                    }
            }
        }

    }
}