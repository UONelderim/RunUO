/***************************************************************************
 *                                  Race.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Race.cs 173 2007-04-25 01:06:43Z krrios $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
	[Parsable]
	public abstract class Race
	{
		public static Race DefaultRace { get { return m_Races[0];  } }

		private static Race[] m_Races = new Race[0x100];

		public static Race[] Races { get { return m_Races; } }
		
		private static List<Race> m_AllRaces = new List<Race>();

		public static List<Race> AllRaces { get { return m_AllRaces; } }

		private int m_RaceID, m_RaceIndex;

		private string m_Name, m_PluralName;

		private static string[] m_RaceNames;
		private static Race[] m_RaceValues;

		public static string[] GetRaceNames()
		{
			CheckNamesAndValues();
			return m_RaceNames;
		}

		public static Race[] GetRaceValues()
		{
			CheckNamesAndValues();
			return m_RaceValues;
		}

		public static Race Parse( string value )
		{
			CheckNamesAndValues();

			for( int i = 0; i < m_RaceNames.Length; ++i )
			{
				if( Insensitive.Equals( m_RaceNames[i], value ) )
					return m_RaceValues[i];
			}

			int index;
			if( int.TryParse( value, out index ) )
			{
				if( index >= 0 && index < m_Races.Length && m_Races[index] != null )
					return m_Races[index];
			}

			throw new Exception( "Invalid race name" );
		}

		private static void CheckNamesAndValues()
		{
			if( m_RaceNames != null && m_RaceNames.Length == m_AllRaces.Count )
				return;

			m_RaceNames = new string[m_AllRaces.Count];
			m_RaceValues = new Race[m_AllRaces.Count];

			for( int i = 0; i < m_AllRaces.Count; ++i )
			{
				Race race = m_AllRaces[i];

				m_RaceNames[i] = race.Name;
				m_RaceValues[i] = race;
			}
		}

		public override string ToString()
		{
			return m_Name;
		}

		private int m_MaleBody, m_FemaleBody, m_MaleGhostBody, m_FemaleGhostBody;

		private Expansion m_RequiredExpansion;

        protected Dictionary<SkillName, double> m_skillGainModifiers = new Dictionary<SkillName, double>();

        public double SkillGainModifier(SkillName skillName) {
            if (m_skillGainModifiers.ContainsKey(skillName)) {
                return m_skillGainModifiers[skillName];
            }
            return 1.0;
        }

		public Expansion RequiredExpansion { get { return m_RequiredExpansion; } }

		public int MaleBody { get { return m_MaleBody; } }
		public int MaleGhostBody { get { return m_MaleGhostBody; } }

		public int FemaleBody { get { return m_FemaleBody; } }
		public int FemaleGhostBody { get { return m_FemaleGhostBody; } }

		public Race( int raceID, int raceIndex, int maleBody, int femaleBody, int maleGhostBody, int femaleGhostBody, Expansion requiredExpansion )
		{
			m_RaceID = raceID;
			m_RaceIndex = raceIndex;

            m_Name = GetName( Cases.Mianownik );
			m_MaleBody = maleBody;
			m_FemaleBody = femaleBody;
			m_MaleGhostBody = maleGhostBody;
			m_FemaleGhostBody = femaleGhostBody;

			m_RequiredExpansion = requiredExpansion;
            m_PluralName = GetName( Cases.Mianownik, true );
		}

        protected abstract string[] Names{ get; }
        protected abstract string[] PluralNames { get; }
        public virtual int DescNumber { get { return 1072202; } } // Description
        public abstract int[] SkinHues { get; }
        public abstract int[] HairHues { get; }
        public abstract int[] MaleHairStyles { get;  }
        public abstract int[] FemaleHairStyles { get; }
        public abstract int[] FacialHairStyles { get; }

        public string GetName( Cases c )
        {
            return GetName( c, false );
        }

        public string GetName( Cases c, bool plural )
        {
            int index = (int)c;
            string[] list = plural ? PluralNames : Names;

            if ( list[ index ] != null )
                return list[ index ];

            return "~ERROR~";
        }

        public virtual int ClipSkinHue( int hue )
        {
	        if ( SkinHues.Contains( hue ) )
                return hue;
            else
                return SkinHues[ 0 ];
        }

        public virtual int RandomSkinHue()
        {
            return SkinHues[ Utility.Random( SkinHues.Length ) ];
        }

        public virtual int ClipHairHue( int hue )
        {
            if ( HairHues.Contains( hue ) )
                return hue;
            else
                return HairHues[ 0 ];
        }

        public virtual int RandomHairHue()
        {
            return HairHues[ Utility.Random( HairHues.Length ) ];
        }

        public virtual int RandomFacialHair( Mobile m ) { return RandomFacialHair( m.Female ); }
        public virtual int RandomFacialHair( bool female )
        {
            return female ? 0 : (int)FacialHairStyles[ Utility.Random( FacialHairStyles.Length )];
        }

        public virtual int RandomHair( Mobile m ) { return RandomHair( m.Female ); }
        public virtual int RandomHair( bool female )
        {
            return Utility.RandomList(female ? FacialHairStyles : MaleHairStyles);
        }

        public virtual bool ValidateHair( Mobile m, int itemID ) { return ValidateHair( m.Female, itemID ); }
        public virtual bool ValidateHair( bool female, int itemID )
        {
            if ( ( female && itemID == 0x2048 ) || ( !female && itemID == 0x2046 ) )
                return false;	//Buns & Receeding Hair

            if ( itemID < 0x203B || itemID > 0x203D )
                return false;

            if ( itemID < 0x2044 || itemID > 0x204A )
                return false;

            int[] hairStyles = female ? FemaleHairStyles : MaleHairStyles;
            
            foreach( int hair in hairStyles )
            {
                if( hair == itemID )
                    return true;
            }

            return false;
        }

		public virtual bool ValidateFacialHair( Mobile m, int itemID ) { return ValidateFacialHair( m.Female, itemID ); }
        public virtual bool ValidateFacialHair( bool female, int itemID )
        {
            if ( female )
                return false;

            if ( itemID < 0x203E || itemID > 0x2041 )
                return true;

            if ( itemID < 0x204B || itemID > 0x204D )
                return true;

            foreach ( int facialHair in FacialHairStyles )
            {
                if ( facialHair == itemID )
                    return true;
            }

            return false;
        }

        public virtual int Body( Mobile m )
		{
			if( m.Alive )
				return AliveBody( m.Female );

			return GhostBody( m.Female );
		}

		public virtual int AliveBody( Mobile m ) { return AliveBody( m.Female ); }
		public virtual int AliveBody( bool female )
		{
			return female ? m_FemaleBody : m_MaleBody;
		}

		public virtual int GhostBody( Mobile m ) { return GhostBody( m.Female ); }
		public virtual int GhostBody( bool female )
		{
			return (female ? m_FemaleGhostBody : m_MaleGhostBody);
		}

		public int RaceID
		{
			get
			{
				return m_RaceID;
			}
		}

		public int RaceIndex
		{
			get
			{
				return m_RaceIndex;
			}
		}

		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		public string PluralName
		{
			get
			{
				return m_PluralName;
			}
			set
			{
				m_PluralName = value;
			}
		}
		
        public virtual bool MakeRandomAppearance( Mobile m )
        {
            if ( !( m.BodyValue == 400 || m.BodyValue == 401 ) )
                return false;

            m.HairItemID       = RandomHair( m.Female );
            m.FacialHairItemID = RandomFacialHair( m.Female );
            m.HairHue          = ClipHairHue( RandomHairHue() );
            m.FacialHairHue    = m.HairHue;
            m.Hue              = ClipSkinHue( RandomSkinHue() );
            
            return true;
        }
	}

	public enum Cases
    {
        Mianownik,
        Dopelniacz,
        Celownik,
        Biernik,
        Narzednik,
        Miejscownik, 
        Wolacz 
    }
}