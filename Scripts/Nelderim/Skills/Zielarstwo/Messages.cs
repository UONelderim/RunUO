using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Gumps;
using Server.Items; 
using Server.Mobiles;
using Server.Engines;
using Server.Engines.Harvest;

namespace Server.Items.Crops
{
	public class SeedlingMsgs
	{
		public virtual string CantBeMounted { get { return "Musisz stac na ziemi, aby moc sadzic rosliny."; } }
		public virtual string BadTerrain { get { return "Roslina na pewno nie urosnie na tym terenie."; } }
		public virtual string PlantAlreadyHere { get { return "W tym miejscu cos juz rosnie."; } }
		public virtual string TooLowSkillToPlant { get { return "Nie wiesz zbyt wiele o sadzeniu ziol."; } }
		public virtual string PlantSuccess { get { return "Udalo ci sie zasadzic rosline."; } }
		public virtual string PlantFail { get { return "Nie udalo ci sie zasadzic rosliny, zmarnowales szczepke."; } }
	}

	public class PlantMsgs
	{

		public virtual string CantBeMounted { get { return "Nie mozesz zbierac roslin bedac konno."; } }
		public virtual string MustGetCloser { get { return "Musisz podejsc blizej, aby to zebrac."; } }
		public virtual string PlantTooYoung { get { return "Roslina jest jeszcze niedojrzala."; } }
		public virtual string NoChanceToGet { get { return "Twoja wiedza o tej roslinie jest za mala, aby ja zebrac."; } }
		public virtual string Succesfull { get { return "Udalo ci sie zebrac rosline."; } }
		public virtual string GotSeed { get { return "Udalo ci sie zebrac szczepke rosliny!"; } }
		public virtual string FailToGet { get { return "Nie udalo ci sie zebrac ziela."; } }
		public virtual string PlantDestroyed { get { return "Zniszczyles rosline."; } }

	}

	public class ResourceMsgs : PlantMsgs
	{
		public override string CantBeMounted { get { return "Nie mozesz zbierac surowcow bedac konno."; } }
		public override string MustGetCloser { get { return "Musisz podejsc blizej, aby to zebrac."; } }
		public override string PlantTooYoung { get { return "Ilosc surowca w tym miejscu nie jest jeszcze wystarczajaca."; } }
		public override string NoChanceToGet { get { return "Twoja wiedza o tym surowcu jest za mala, aby go wykorzystac."; } }
		public override string Succesfull { get { return "Udalo ci sie zebrac surowiec."; } }
		public override string GotSeed { get { return "Udalo ci sie zebrac szczepke rosliny!"; } }
		public override string FailToGet { get { return "Nie udalo ci sie zebrac surowca."; } }
		public override string PlantDestroyed { get { return "Zmarnowales okazje."; } }
	}

	public class CropMsgs
	{
		public virtual string CreatedZeroReagent { get { return "Nie uzyskales wystarczajacej ilosci reagentu."; } }
		public virtual string FailedToCreateReagents { get { return "Nie udalo ci sie uzyskac reagentow."; } }
		public virtual string CreatedReagent { get { return "Uzyskales nieco reagentu."; } }
		public virtual string StartedToCut { get { return "Zaczynasz obrabiac surowiec..."; } }
	}

}