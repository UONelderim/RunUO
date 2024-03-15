using System;
using Server;
using System.Collections.Generic;
using System.Threading;
using Server.Network;

namespace Server.Items
{
	public class Beastmaster : GoldEarrings
	{
		
		private DateTime m_LastStaminaLoss;
		private Thread m_TimerThread;
		
		public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }


		[Constructable]
		public Beastmaster()
		{
			Name = "Kolczuyki Wladcy Bestii";
			Hue = 1153;

			Attributes.BonusStr = -20;

            SkillBonuses.SetValues( 0, SkillName.AnimalTaming, 5.0 );
			SkillBonuses.SetValues( 1, SkillName.AnimalLore, 5.0 );
			Label1 = "*grawer w języku krasnoludow rzecze, iz owe kolczyki wysysaja wytrzymalosc noszacego*";
			m_LastStaminaLoss = DateTime.UtcNow;
			
			m_TimerThread = new Thread(new ThreadStart(StaminaLossThread));
			m_TimerThread.IsBackground = true;
			m_TimerThread.Start();


	}

		public Beastmaster ( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		public override bool OnEquip(Mobile from)
        {
            Mobile player = from as Mobile;
            if (player != null)
            {
                player.SendMessage("Zakładając te kolczyki czujesz jak krasnoludzkie runy powoduja spadek Twojej wytrzymałości.");
            }
            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            Mobile from = parent as Mobile;
            if (from != null)
            {
                if (from is Mobile)
                {
                    Mobile player = from as Mobile;
                    if (player != null)
                    {
                        player.SendMessage("Zdejmując kolczyki, runy przestaja działać.");
                    }
                }
            }
            base.OnRemoved(parent);
        }

        private void StaminaLossThread()
        {
            while (!Deleted)
            {
                foreach (Mobile player in GetPlayersWithEarringsEquipped())
                {
                    if (player.Alive)
                    {
                        TimeSpan timeSinceLastStaminaLoss = DateTime.UtcNow - m_LastStaminaLoss;
                        if (timeSinceLastStaminaLoss.TotalSeconds >= 1)
                        {
                            player.Stam -= 1;
                            m_LastStaminaLoss = DateTime.UtcNow;
                        }
                    }
                }

                System.Threading.Thread.Sleep(1000);
            }
        }

        private List<Mobile> GetPlayersWithEarringsEquipped()
        {
            List<Mobile> players = new List<Mobile>();

            foreach (NetState state in NetState.Instances)
            {
                Mobile player = state.Mobile;

                if (player != null && player.Backpack != null)
                {
                    Item earrings = player.FindItemOnLayer(Layer.Earrings);

                    if (earrings is EarringsOfTheMagician)
                    {
                        players.Add(player);
                    }
                }
            }

            return players;
        }
    }
	
}
