using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Factions;

namespace Server.Mobiles
{
    public class NPCSzachy : BaseCreature
    {
        [Constructable]
        public  NPCSzachy()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            InitStats(100, 125, 25);

            Body = 400;
            CantWalk = true;
            Hue = 1641;
            Blessed = true;
            Name = "Zenobiusz z Tafroel";
			Title = "- Mistrz Szachów";

            Skills[SkillName.Anatomy].Base = 120.0;
            Skills[SkillName.Tactics].Base = 120.0;
            Skills[SkillName.Magery].Base = 120.0;
            Skills[SkillName.MagicResist].Base = 120.0;
            Skills[SkillName.DetectHidden].Base = 100.0;

        }

        public  NPCSzachy(Serial serial)
            : base(serial)
        {
        }

        public override void OnSpeech( SpeechEventArgs e )
{
base.OnSpeech( e );

Mobile from = e.Mobile;

if ( from.InRange( this, 2 ))
{
if (e.Speech.ToLower().IndexOf( "zadan" ) >= 0 || e.Speech.ToLower().IndexOf( "witaj" ) >= 0 ) {
 string message1 = "No, No, No... Widze, ze mamy tu amatora szachow! *zaciera rece* Moze chcesz sprobowac sie w Szachach Bojowych? Kosztuje to jedyne 5000 centarow, 100 sztuk srebra i 1 krysztal komunikacyjny! Przekaz mi te pieniadze, a pozwole Ci zagrac w Szachy Bojowe! *wyciaga rece po pieniadze*";
                    this.Say(message1);
}
}
}








public int gold1;
public int silver2;
public int broadcastcrystal3;

public override bool OnDragDrop( Mobile from, Item dropped )
		{

			if ( dropped is Gold )
			{
				dropped.Delete();
			
	
						
								Say( true, " Dziekuje bardzo! Potrza mnie jeszcze srebra, sztuk 200 i 1 krysztal komunikacyjny! " );

			gold1=5000;
}

if ( dropped is Silver && gold1<1 )
			{
		
			
	

						
Say( true, "Glupcze, najpierw potrzebuje zloto!" );

			
}
		if ( dropped is Silver && gold1>0 )
			{
				dropped.Delete();
					
								Say( true, " No, no. Teraz to mowisz w moim jezyku! Jeszcze krysztal komunikacyjny. " );

			silver2=100;
}
if ( dropped is BroadcastCrystal && gold1<1 )
			{
		
			
	

						
								Say( true, "Glupcze, najpierw potrzebuje srebra!" );

			
}
if ( dropped is BroadcastCrystal && silver2<1 && gold1>0 )
			{
		
			
	

						
								Say( true, "Glupcze, teraz potrzebuje srevra!" );

			
}

if ( dropped is BroadcastCrystal && silver2>0 )
			{
				dropped.Delete();

			broadcastcrystal3=1;
if ( gold1>0 && silver2>0 && broadcastcrystal3>0)
{
	Point3D loc = new Point3D(2099, 3435, 3); 
			Map map = Map.Felucca;
WrotaSzachy portal = new WrotaSzachy();
portal.MoveToWorld( loc, map );



						
								Say( true, " Ooo tak! Wspaniale... Prosze oto portal, ktory zaprowadzi cie do zwyciestwa... hahaha, ruszaj smialo, tylko spiesz sie! Za chwile go zamkne! " );

gold1=0;
silver2=0;
broadcastcrystal3=0;

}
            }

            return base.OnDragDrop(from, dropped);
        }
        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}