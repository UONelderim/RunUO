/***************************************************************************
 *                          ExpansionInfo.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: ExpansionInfo.cs 187 2007-05-26 03:12:41Z asayre $
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

namespace Server
{
	public enum Expansion
	{
		None,
		T2A,
		UOR,
		UOTD,
		LBR,
		AOS,
		SE,
		ML,
	}
	
	[Flags]
	public enum FeatureFlags
	{
		None = 0x00000000,
		T2A = 0x00000001,
		UOR = 0x00000002,
		UOTD = 0x00000004,
		LBR = 0x00000008,
		AOS = 0x00000010,
		SixthCharacterSlot = 0x00000020,
		SE = 0x00000040,
		ML = 0x00000080,
		EigthAge = 0x00000100,
		NinthAge = 0x00000200, /* Crystal/Shadow Custom House Tiles */
		TenthAge = 0x00000400,
		IncreasedStorage = 0x00000800, /* Increased Housing/Bank Storage */
		SeventhCharacterSlot = 0x00001000,
		RoleplayFaces = 0x00002000,
		TrialAccount = 0x00004000,
		LiveAccount = 0x00008000,
		SA = 0x00010000,
		HS = 0x00020000,
		Gothic = 0x00040000,
		Rustic = 0x00080000,
		Jungle = 0x00100000,
		Shadowguard = 0x00200000,
		TOL = 0x00400000,
		EJ = 0x00800000,

		ExpansionNone = None,
		ExpansionT2A = T2A,
		ExpansionUOR = ExpansionT2A | UOR,
		ExpansionUOTD = ExpansionUOR | UOTD,
		ExpansionLBR = ExpansionUOTD | LBR,
		ExpansionAOS = ExpansionLBR | AOS | LiveAccount,
		ExpansionSE = ExpansionAOS | SE,
		ExpansionML = ExpansionSE | ML | NinthAge,
		ExpansionSA = ExpansionML | SA | Gothic | Rustic,
		ExpansionHS = ExpansionSA | HS,
		ExpansionTOL = ExpansionHS | TOL | Jungle | Shadowguard,
		ExpansionEJ = ExpansionTOL | EJ,
	}

	[Flags]
	public enum CharacterListFlags
	{
		None = 0x00000000,
		Unk1 = 0x00000001,
		OverwriteConfigButton = 0x00000002,
		OneCharacterSlot = 0x00000004,
		ContextMenus = 0x00000008,
		SlotLimit = 0x00000010,
		AOS = 0x00000020,
		SixthCharacterSlot = 0x00000040,
		SE = 0x00000080,
		ML = 0x00000100,
		Unk2 = 0x00000200,
		UO3DClientType = 0x00000400,
		KR = 0x00000600, // uo:kr support flags
		Unk3 = 0x00000800,
		SeventhCharacterSlot = 0x00001000,
		Unk4 = 0x00002000,
		NewMovementSystem = 0x00004000,
		NewFeluccaAreas = 0x00008000,

		ExpansionNone = ContextMenus, //
		ExpansionT2A = ContextMenus, //
		ExpansionUOR = ContextMenus, // None
		ExpansionUOTD = ContextMenus, //
		ExpansionLBR = ContextMenus, //
		ExpansionAOS = ContextMenus | AOS,
		ExpansionSE = ExpansionAOS | SE,
		ExpansionML = ExpansionSE | ML,
		ExpansionSA = ExpansionML,
		ExpansionHS = ExpansionSA,
		ExpansionTOL = ExpansionHS,
		ExpansionEJ = ExpansionTOL,
	}
	
	public class ExpansionInfo
	{
		private string m_Name;
		private int m_ID, m_NetStateFlag, m_SupportedFeatures, m_CharListFlags, m_CustomHousingFlag;

		private ClientVersion m_RequiredClient;	//Used as an alternative to the flags

		public string Name{ get{ return m_Name; } }
		public int ID{ get{ return m_ID; } }
		public int NetStateFlag{ get{ return m_NetStateFlag; } }
		public int SupportedFeatures{ get{ return m_SupportedFeatures; } }
		public int CharacterListFlags { get { return m_CharListFlags; } }
		public int CustomHousingFlag { get{ return m_CustomHousingFlag; } }
		public ClientVersion RequiredClient { get { return m_RequiredClient; } }

		public ExpansionInfo( int id, string name, int netStateFlag, int supportedFeatures, int charListFlags, int customHousingFlag )
		{
			m_Name = name;
			m_ID = id;
			m_NetStateFlag = netStateFlag;
			m_SupportedFeatures = supportedFeatures;
			m_CharListFlags = charListFlags;
			m_CustomHousingFlag = customHousingFlag;
		}

		public ExpansionInfo( int id, string name, ClientVersion requiredClient, int supportedFeatures, int charListFlags, int customHousingFlag )
		{
			m_Name = name;
			m_ID = id;
			m_SupportedFeatures = supportedFeatures;
			m_CharListFlags = charListFlags;
			m_CustomHousingFlag = customHousingFlag;
			m_RequiredClient = requiredClient;
		}

		public static ExpansionInfo[] Table { get { return m_Table; } }
		private static ExpansionInfo[] m_Table = new ExpansionInfo[]
			{
				new ExpansionInfo( 0, "None",					 					0x00,	0x0000, 0x008, 0x00 ),
				new ExpansionInfo( 1, "The Second Age", 				0x01,	0x0000, 0x008, 0x00 ),
				new ExpansionInfo( 2, "Renaissance",	 					0x02,	0x0000, 0x008, 0x00 ),
				new ExpansionInfo( 3, "Third Dawn", 						0x04,	0x0000, 0x008, 0x00 ),
				new ExpansionInfo( 4, "Blackthorn's Revenges",	0x06,	0x0000, 0x008, 0x00 ),
				new ExpansionInfo( 5, "Age of Shadows", 				0x08,	0x801F, 0x028, 0x20 ),
				new ExpansionInfo( 6, "Samurai Empire",					0x10,	0x805F, 0x0A8, 0x60 ),	//0x40 | 0x20 = 0x60
				new ExpansionInfo( 7, "Mondain's Legacy", new ClientVersion( "5.0.0a" ),	0x82DF, 0x1A8, 0x2E0 )	//0x280 | 0x60 = 0x2E0

				//0x200 + 0x400 for KR?
			};

		public static ExpansionInfo GetInfo( Expansion ex )
		{
			return GetInfo( (int)ex );
		}

		public static ExpansionInfo GetInfo( int ex )
		{
			int v = (int)ex;

			if( v < 0 || v >= m_Table.Length )
				v = 0;

			return m_Table[v];
		}

		public static ExpansionInfo CurrentExpansion { get { return GetInfo( Core.Expansion ); } }

		public override string ToString()
		{
			return m_Name;
		}
	}
}
