using Server;
using System.IO;
using System;
using Nelderim;
using Nelderim.Speech;
using Server.Spells.Bushido;
using static Server.Items.ArtifactMonster;

namespace Server.Items
{
	public abstract partial class BaseWeapon
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public int LegacyDamageAttr
		{
			get { return BaseWeaponExt.Get(this).LegacyDamageAttr; }
			set
			{ 
				if (BaseWeaponExt.Get(this).LegacyDamageAttr == int.MinValue) // do not overwrite if already set
					BaseWeaponExt.Get(this).LegacyDamageAttr = value;
			}
		}

		public virtual int DamageBonusFromExceptional()
		{
			return 35;
		}
	}

	public abstract partial class BaseRanged
	{
		public override int DamageBonusFromExceptional()
		{
			return 25;
		}
	}

	class BaseWeaponExt : NExtension<BaseWeaponExtInfo>
	{
		private static string moduleName = "BaseWeapon";
		private static bool WeaponsAlreadyFixed = true;

		public static void InitializeWeaponDamageFix()
		{
			Load();

			EventSink.WorldSave += new WorldSaveEventHandler(SaveState);

			if (!WeaponsAlreadyFixed)
			{
				Console.WriteLine("Weapon damage fix: applying...");

				FixAllWeaponsDamage();
			}
			else
			{
				Console.WriteLine("Weapon damage fix: not required");
			}
		}

		private static void FixAllWeaponsDamage()
		{
			foreach (var item in World.Items.Values)
			{
				if (item is BaseWeapon weapon && weapon.PlayerConstructed)
				{
					weapon.LegacyDamageAttr = weapon.Attributes.WeaponDamage; // backup original +DI value

					if (weapon.Quality == WeaponQuality.Exceptional)
						weapon.Attributes.WeaponDamage += weapon.DamageBonusFromExceptional();

					CraftResourceInfo resInfo = CraftResources.GetInfo(weapon.Resource);
					if (resInfo != null)
					{
						CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

						if (attrInfo != null)
							weapon.Attributes.WeaponDamage += attrInfo.WeaponDamageIncrease;
					}
					CraftResourceInfo resInfo2 = CraftResources.GetInfo(weapon.Resource2);
					if (resInfo2 != null)
					{
						CraftAttributeInfo attrInfo2 = resInfo2.AttributeInfo;

						if (attrInfo2 != null)
							weapon.Attributes.WeaponDamage += attrInfo2.WeaponDamageIncrease;
					}

					weapon.Attributes.WeaponDamage = Math.Min(weapon.Attributes.WeaponDamage, 50);

					weapon.InvalidateProperties();
				}
			}
		}

		private static void Load()
		{
			if (!File.Exists(@"Saves/Nelderim/" + moduleName + ".sav"))
				moduleName = char.ToLower(moduleName[0]) + moduleName.Substring(1); // 1st letter lowercase
			if (!File.Exists(@"Saves/Nelderim/" + moduleName + ".sav"))
			{
				WeaponsAlreadyFixed = false;
				return;
			}

			string pathNfile = @"Saves/Nelderim/" + moduleName + ".sav";

			Console.WriteLine(moduleName + ": Wczytywanie...");
			using (FileStream m_FileStream = new FileStream(pathNfile, FileMode.Open, FileAccess.Read))
			{
				BinaryReader m_BinaryReader = new BinaryReader(m_FileStream);
				BinaryFileReader reader = new BinaryFileReader(m_BinaryReader);

				WeaponsAlreadyFixed = reader.ReadBool();

				int count = reader.ReadInt();
				for (int i = 0; i < count; i++)
				{
					Serial serial = (Serial)reader.ReadInt();

					BaseWeaponExtInfo info = new BaseWeaponExtInfo
					{
						Serial = serial,
					};

					info.Deserialize(reader);
				}
			}
		}

		public static void SaveState(WorldSaveEventArgs args)
		{
			if (!Directory.Exists(Path.Combine(World.ServUOSave ? "Servuo" : "", "Saves/Nelderim")))
				Directory.CreateDirectory(Path.Combine(World.ServUOSave ? "Servuo" : "", "Saves/Nelderim"));

			string pathNfile = Path.Combine(World.ServUOSave ? "Servuo" : "", @"Saves/Nelderim/", moduleName + ".sav");

			Console.WriteLine(moduleName + ": Zapisywanie...");
			try
			{
				using (FileStream m_FileStream = new FileStream(pathNfile, FileMode.OpenOrCreate, FileAccess.Write))
				{
					BinaryFileWriter writer = new BinaryFileWriter(m_FileStream, true);

					writer.Write((bool)true); // weapons already fixed

					writer.Write((int)m_ExtensionInfo.Count);
					foreach (BaseWeaponExtInfo info in m_ExtensionInfo.Values)
					{
						writer.Write(info.Serial);
						info.Serialize(writer);
					}

					writer.Close();
					m_FileStream.Close();
				}
			}
			catch (Exception err)
			{
				Console.WriteLine("Failed. Exception: " + err);
			}
		}
	}


	class BaseWeaponExtInfo : NExtensionInfo
	{
		private int m_LegacyDamageAttr;

		public int LegacyDamageAttr { get { return m_LegacyDamageAttr; } set { m_LegacyDamageAttr = value; } }

		public BaseWeaponExtInfo()
		{
			m_LegacyDamageAttr = int.MinValue; // mark as unset
		}

		public override void Serialize(GenericWriter writer)
		{
			writer.Write((int)1); // version

			writer.Write((int)m_LegacyDamageAttr);
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			m_LegacyDamageAttr = reader.ReadInt();
		}
	}
}