namespace Server.SicknessSys.Illnesses
{
	public static class Virus
	{
		private static int illnessType = 3;
		private static string name;
		private static int statDrain;
		private static int baseDamage;
		private static int powerDegenRate;

		public static int IllnessType
		{
			get { return illnessType; }
		}

		public static string Name
		{
			get
			{
				if (string.IsNullOrEmpty(name))
				{
					name = BaseVirus.GetRandomName(illnessType);
				}

				return name;
			}
		}

		public static int StatDrain
		{
			get
			{
				if (statDrain == 0)
				{
					statDrain = BaseVirus.GetRandomDamage(illnessType);
				}

				return statDrain;
			}
		}

		public static int BaseDamage
		{
			get
			{
				if (baseDamage == 0)
				{
					baseDamage = BaseVirus.GetRandomDamage(illnessType);
				}

				return baseDamage;
			}
		}

		public static int PowerDegenRate
		{
			get
			{
				if (powerDegenRate == 0)
				{
					powerDegenRate = BaseVirus.GetRandomDegen(illnessType);
				}

				return powerDegenRate;
			}
		}


		public static void UpdateBody(VirusCell cell)
		{
			IllnessMutationLists.SetMutation(cell);
		}
	}
}
