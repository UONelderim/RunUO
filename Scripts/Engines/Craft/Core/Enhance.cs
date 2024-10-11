using System;
using Server;
using Server.Targeting;
using Server.Items;

namespace Server.Engines.Craft
{
	public enum EnhanceResult
	{
		None,
		NotInBackpack,
		BadItem,
		BadResource,
		AlreadyEnhanced,
		Success,
		Failure,
		Broken,
		NoResources,
		NoSkill
	}

	public class Enhance
	{
		public static EnhanceResult Invoke( Mobile from, CraftSystem craftSystem, BaseTool tool, Item item, CraftResource resource, Type resType, CraftResource resource2, Type resType2, ref object resMessage )
		{
			if ( item == null )
				return EnhanceResult.BadItem;

			if ( !item.IsChildOf( from.Backpack ) )
				return EnhanceResult.NotInBackpack;

			if ( !(item is BaseArmor) && !(item is BaseWeapon) )
				return EnhanceResult.BadItem;

			if ( item is IArcaneEquip )
			{
				IArcaneEquip eq = (IArcaneEquip)item;
				if ( eq.IsArcane )
					return EnhanceResult.BadItem;
			}

			CraftItem craftItem = craftSystem.CraftItems.SearchFor( item.GetType() );

			if ( craftItem == null || craftItem.Ressources.Count == 0 )
				return EnhanceResult.BadItem;

            // Podstawowy kolor surowca explicite
            if ( resType == null && craftSystem.CraftSubRes.Count > 0 )
                resType = craftSystem.CraftSubRes.GetAt(0).ItemType;
            if ( resType2 == null && craftSystem.CraftSubRes2.Count > 0 )
                resType2 = craftSystem.CraftSubRes2.GetAt(0).ItemType;
			
            // Ignoruj surowiec wybrany w menu rzemiosla jesli nie odpowiada on typowi zadnego ze skladnikow uzywanych do produkcji
            if ( craftItem != null )
            {
                bool ignore1 = true;
                bool ignore2 = true;
                CraftResourceType menuResType1 =  CraftResources.GetType(CraftResources.GetFromType(resType));
                CraftResourceType menuResType2 =  CraftResources.GetType(CraftResources.GetFromType(resType2));
                for(int i=0; i<craftItem.Ressources.Count; ++i)
                {
                    CraftResourceType ingredientResType = CraftResources.GetType(CraftResources.GetFromType(craftItem.Ressources.GetAt(i).ItemType));
                    if ( ingredientResType == menuResType1 )
                        ignore1 = false;
                    if ( ingredientResType == menuResType2 )
                        ignore2 = false;
                }
                if ( ignore1 )
                {
                    resType = null;
                    resource = CraftResource.None;
                }
                if ( ignore2 ) 
                {
                    resType2 = null;
                    resource2 = CraftResource.None;
                }
            }

            // Brak kolorow?
			if ( CraftResources.IsStandard( resource ) && CraftResources.IsStandard( resource2 ) )
				return EnhanceResult.BadResource;

            // Already enhanced?
            CraftResource itemRes = CraftResource.None;
            CraftResource itemRes2 = CraftResource.None;
            if ( item is BaseWeapon )
            {
                itemRes = craftItem.UseSubRes2 ? ((BaseWeapon)item).Resource2 : ((BaseWeapon)item).Resource;
				itemRes2 = craftItem.UseSubRes2 ? ((BaseWeapon)item).Resource : ((BaseWeapon)item).Resource2;
            }
            else if ( item is BaseArmor )
            {
                itemRes = craftItem.UseSubRes2 ? ((BaseArmor)item).Resource2 : ((BaseArmor)item).Resource;
                itemRes2 = craftItem.UseSubRes2 ? ((BaseArmor)item).Resource : ((BaseArmor)item).Resource2;
            }
            if ( !CraftResources.IsStandard( itemRes ) && !CraftResources.IsStandard( resource ) )
                return EnhanceResult.AlreadyEnhanced;
            if ( !CraftResources.IsStandard( itemRes2 ) && !CraftResources.IsStandard( resource2 ) )
                return EnhanceResult.AlreadyEnhanced;

            // act as there is no resource chosen in craft menu if it's color is blank
            if ( CraftResources.IsStandard( resource ) )
                resType = null;
            if ( CraftResources.IsStandard( resource2 ) )
                resType2 = null;

			int num = craftSystem.CanCraft( from, tool, item.GetType() );
			
			if ( num > 0 )
			{
				resMessage = num;
				return EnhanceResult.None;
			}

			bool allRequiredSkills = false;
			if( craftItem.GetSuccessChance( from, resType, resType2, craftSystem, false, ref allRequiredSkills ) <= 0.0 )
				return EnhanceResult.NoSkill;

			CraftResourceInfo info  = CraftResources.GetInfo( resource );
            CraftResourceInfo info2 = CraftResources.GetInfo( resource2 );

			if ( (info == null || info.ResourceTypes.Length == 0) && (info2 == null || info2.ResourceTypes.Length == 0) )
				return EnhanceResult.BadResource;

			CraftAttributeInfo attributes =  info  != null ? info.AttributeInfo  : null;
            CraftAttributeInfo attributes2 = info2 != null ? info2.AttributeInfo : null;

			if ( attributes == null && attributes2 == null )
				return EnhanceResult.BadResource;

			int resHue = 0, maxAmount = 0;

			if ( !craftItem.ConsumeRes( from, resType, resType2, craftSystem, ref resHue, ref maxAmount, ConsumeType.None, ref resMessage, false, true ) )
				return EnhanceResult.NoResources;

			if ( craftSystem is DefBlacksmithy )
			{
				AncientSmithyHammer hammer = from.FindItemOnLayer( Layer.OneHanded ) as AncientSmithyHammer;
				if ( hammer != null )
				{
					hammer.UsesRemaining--;
					if ( hammer.UsesRemaining < 1 )
						hammer.Delete();
				}
			}
			else if ( craftSystem is DefTailoring )
			{
				AncientSewingKit hammer = from.FindItemOnLayer( Layer.OneHanded ) as AncientSewingKit;
				if ( hammer != null )
				{
					hammer.UsesRemaining--;
					if ( hammer.UsesRemaining < 1 )
						hammer.Delete();
				}
			}
			else if ( craftSystem is DefBowFletching )
			{
				AncientFletcherTools hammer = from.FindItemOnLayer( Layer.OneHanded ) as AncientFletcherTools;
				if ( hammer != null )
				{
					hammer.UsesRemaining--;
					if ( hammer.UsesRemaining < 1 )
						hammer.Delete();
				}
			}

			int phys = 0, fire = 0, cold = 0, pois = 0, nrgy = 0;
			int dura = 0, luck = 0, lreq = 0, dinc = 0;
			int baseChance = 0;

			bool physBonus = false;
			bool fireBonus = false;
			bool coldBonus = false;
			bool nrgyBonus = false;
			bool poisBonus = false;
			bool duraBonus = false;
			bool luckBonus = false;
			bool lreqBonus = false;
			bool dincBonus = false;

			if ( item is BaseWeapon )
			{
				BaseWeapon weapon = (BaseWeapon)item;

				baseChance = 20;

				dura = weapon.MaxHitPoints;
				luck = weapon.Attributes.Luck;
				lreq = weapon.WeaponAttributes.LowerStatReq;
				dinc = weapon.Attributes.WeaponDamage;

                if ( attributes != null )
                {
				    fireBonus = (fireBonus || attributes.WeaponFireDamage > 0 );
				    coldBonus = (coldBonus || attributes.WeaponColdDamage > 0 );
				    nrgyBonus = (nrgyBonus || attributes.WeaponEnergyDamage > 0 );
				    poisBonus = (poisBonus || attributes.WeaponPoisonDamage > 0 );

				    duraBonus = (duraBonus || attributes.WeaponDurability > 0 );
				    luckBonus = (luckBonus || attributes.WeaponLuck > 0 );
				    lreqBonus = (lreqBonus || attributes.WeaponLowerRequirements > 0 );
                }
                if ( attributes2 != null )
                {
				    fireBonus = (fireBonus || attributes2.WeaponFireDamage > 0 );
				    coldBonus = (coldBonus || attributes2.WeaponColdDamage > 0 );
				    nrgyBonus = (nrgyBonus || attributes2.WeaponEnergyDamage > 0 );
				    poisBonus = (poisBonus || attributes2.WeaponPoisonDamage > 0 );

				    duraBonus = (duraBonus || attributes2.WeaponDurability > 0 );
				    luckBonus = (luckBonus || attributes2.WeaponLuck > 0 );
				    lreqBonus = (lreqBonus || attributes2.WeaponLowerRequirements > 0 );
                }
				dincBonus = ( dinc > 0 );
			}
			else
			{
				BaseArmor armor = (BaseArmor)item;

				baseChance = 20;

				phys = armor.PhysicalResistance;
				fire = armor.FireResistance;
				cold = armor.ColdResistance;
				pois = armor.PoisonResistance;
				nrgy = armor.EnergyResistance;

				dura = armor.MaxHitPoints;
				luck = armor.Attributes.Luck;
				lreq = armor.ArmorAttributes.LowerStatReq;

                if( attributes != null )
                {
				    physBonus = (physBonus || attributes.ArmorPhysicalResist > 0 );
				    fireBonus = (fireBonus || attributes.ArmorFireResist > 0 );
				    coldBonus = (coldBonus || attributes.ArmorColdResist > 0 );
				    nrgyBonus = (nrgyBonus || attributes.ArmorEnergyResist > 0 );
				    poisBonus = (poisBonus || attributes.ArmorPoisonResist > 0 );

				    duraBonus = (duraBonus || attributes.ArmorDurability > 0 );
				    luckBonus = (luckBonus || attributes.ArmorLuck > 0 );
				    lreqBonus = (lreqBonus || attributes.ArmorLowerRequirements > 0 );
                }
                if( attributes2 != null )
                {
				    physBonus = (physBonus || attributes2.ArmorPhysicalResist > 0 );
				    fireBonus = (fireBonus || attributes2.ArmorFireResist > 0 );
				    coldBonus = (coldBonus || attributes2.ArmorColdResist > 0 );
				    nrgyBonus = (nrgyBonus || attributes2.ArmorEnergyResist > 0 );
				    poisBonus = (poisBonus || attributes2.ArmorPoisonResist > 0 );

				    duraBonus = (duraBonus || attributes2.ArmorDurability > 0 );
				    luckBonus = (luckBonus || attributes2.ArmorLuck > 0 );
				    lreqBonus = (lreqBonus || attributes2.ArmorLowerRequirements > 0 );
                }
				dincBonus = false;
			}

			int skill = from.Skills[craftSystem.MainSkill].Fixed / 10;

			if ( skill >= 100 )
				baseChance -= (skill - 90) / 10;

			EnhanceResult res = EnhanceResult.Success;

			if ( physBonus )
				CheckResult( ref res, baseChance + phys );

			if ( fireBonus )
				CheckResult( ref res, baseChance + fire );

			if ( coldBonus )
				CheckResult( ref res, baseChance + cold );

			if ( nrgyBonus )
				CheckResult( ref res, baseChance + nrgy );

			if ( poisBonus )
				CheckResult( ref res, baseChance + pois );

			if ( duraBonus )
				CheckResult( ref res, baseChance + (dura / 40) );

			if ( luckBonus )
				CheckResult( ref res, baseChance + 10 + (luck / 2) );

			if ( lreqBonus )
				CheckResult( ref res, baseChance + (lreq / 4) );

			if ( dincBonus )
				CheckResult( ref res, baseChance + (dinc / 4) );

			switch ( res )
			{
				case EnhanceResult.Broken:
				{
					from.PlaySound( 0x03F );
					if ( !craftItem.ConsumeRes( from, resType, resType2, craftSystem, ref resHue, ref maxAmount, ConsumeType.Half, ref resMessage, false, true ) )
						return EnhanceResult.NoResources;

					item.Delete();
					
					break;
				}
				case EnhanceResult.Success:
				{
					from.PlaySound( 0x01FD );
					if ( !craftItem.ConsumeRes( from, resType, resType2, craftSystem, ref resHue, ref maxAmount, ConsumeType.All, ref resMessage, false, true ) )
						return EnhanceResult.NoResources;

                    CraftResource resource_switched = craftItem.UseSubRes2 ? resource2 : resource;
                    CraftResource resource2_switched = craftItem.UseSubRes2 ? resource : resource2;

					if( item is BaseWeapon )
					{
						BaseWeapon w = (BaseWeapon)item;

						if (!CraftResources.IsStandard(resource_switched))
							w.Resource = resource_switched;
						if (!CraftResources.IsStandard(resource2_switched))
							w.Resource2 = resource2_switched;

						if (attributes != null)
							w.DistributeMaterialBonus(attributes);
						if (attributes2 != null)
							w.DistributeMaterialBonus(attributes2);

						w.Attributes.WeaponDamage = Math.Min(50, w.Attributes.WeaponDamage);

						int hue = w.GetElementalDamageHue();
						if( hue > 0 )
							w.Hue = hue;
							
						if ( from.AccessLevel > AccessLevel.Player )
						{
							item.ModifiedBy = from.Account.Username;
							item.ModifiedDate = DateTime.Now;
						}	
					}
					else if( item is BaseArmor )	//Sanity
					{
						if ( ! CraftResources.IsStandard( resource_switched ) )
                            ((BaseArmor)item).Resource = resource_switched;
                        if ( ! CraftResources.IsStandard( resource2_switched ) )
                            ((BaseArmor)item).Resource2 = resource2_switched;
					}

					break;
				}
				case EnhanceResult.Failure:
				{
					from.PlaySound( 0x54 );
					if ( !craftItem.ConsumeRes( from, resType, resType2, craftSystem, ref resHue, ref maxAmount, ConsumeType.Half, ref resMessage, true, true ) )
						return EnhanceResult.NoResources;
					
					break;
					}
			}
			return res;
		}

		public static void CheckResult( ref EnhanceResult res, int chance )
		{
			if ( res != EnhanceResult.Success )
				return; // we've already failed..

			int random = Utility.Random( 100 );

			if ( 10 > random )
				res = EnhanceResult.Failure;
			else if ( chance > random )
				res = EnhanceResult.Broken;
		}

		public static void BeginTarget( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			CraftContext context = craftSystem.GetContext( from );

			if ( context == null )
				return;

			int lastRes = context.LastResourceIndex;
            int lastRes2 = context.LastResourceIndex2;
			CraftSubResCol subRes = craftSystem.CraftSubRes;
            CraftSubResCol subRes2 = craftSystem.CraftSubRes2;
            bool isResChosen = (lastRes >= 0 && lastRes < subRes.Count);
            bool isRes2Chosen = (lastRes2 >= 0 && lastRes2 < subRes2.Count);
			if ( isResChosen || isRes2Chosen )
			{
                CraftResource resource = CraftResource.None;
                CraftResource resource2 = CraftResource.None;
                Type resourceType = null;
                Type resourceType2 = null;
                if( isResChosen )
                {
                    CraftSubRes res = subRes.GetAt( lastRes );
					if( !craftSystem.ResourcesUnlocked(from, res.ItemType, null) )  // czy postac umie wykorzystywac danu material?
                    {
                        from.SendGump( new CraftGump( from, craftSystem, tool, 1032588 ) );
                        return;
                    }
                    if( from.Skills[craftSystem.MainSkill].Value < res.RequiredSkill )
                    {
                        from.SendGump( new CraftGump( from, craftSystem, tool, res.Message ) );
                        return;
                    }
                    resource = CraftResources.GetFromType( res.ItemType );
                    resourceType = res.ItemType;
                }
                if( isRes2Chosen )
                {
                    CraftSubRes res2 = subRes2.GetAt( lastRes2 );
					if( !craftSystem.ResourcesUnlocked(from, null, res2.ItemType) )  // czy postac umie wykorzystywac danu material?
                    {
                        from.SendGump( new CraftGump( from, craftSystem, tool, 1032588 ) );
                        return;
                    }
                    if( from.Skills[craftSystem.MainSkill].Value < res2.RequiredSkill )
                    {
                        from.SendGump( new CraftGump( from, craftSystem, tool, res2.Message ) );
                        return;
                    }
                    resource2 = CraftResources.GetFromType( res2.ItemType );
                    resourceType2 = res2.ItemType;
                }

				if ( resource != CraftResource.None || resource2 != CraftResource.None )
				{
					from.Target = new InternalTarget( craftSystem, tool, resourceType, resource, resourceType2, resource2 );
					from.SendLocalizedMessage( 1061004 ); // Target an item to enhance with the properties of your selected material.
				}
				else
				{
					from.SendGump( new CraftGump( from, craftSystem, tool, 1061010 ) ); // You must select a special material in order to enhance an item with its properties.
				}
			}
			else
			{
				from.SendGump( new CraftGump( from, craftSystem, tool, 1061010 ) ); // You must select a special material in order to enhance an item with its properties.
			}

		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;
			private Type m_ResourceType;
			private CraftResource m_Resource;
			private Type m_ResourceType2;
			private CraftResource m_Resource2;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool, Type resourceType, CraftResource resource, Type resourceType2, CraftResource resource2 ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
				m_ResourceType = resourceType;
				m_Resource = resource;
                m_ResourceType2 = resourceType2;
				m_Resource2 = resource2;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					object message = null;

                    EnhanceResult res = Enhance.Invoke( from, m_CraftSystem, m_Tool, (Item)targeted, m_Resource, m_ResourceType, m_Resource2, m_ResourceType2, ref message );
					switch ( res )
					{
						case EnhanceResult.NotInBackpack: message = 1061005; break; // The item must be in your backpack to enhance it.
						case EnhanceResult.AlreadyEnhanced: message = 1061012; break; // This item is already enhanced with the properties of a special material.
						case EnhanceResult.BadItem: message = 1061011; break; // You cannot enhance this type of item with the properties of the selected special material.
						case EnhanceResult.BadResource: message = 1061010; break; // You must select a special material in order to enhance an item with its properties.
						case EnhanceResult.Broken: message = 1061080; break; // You attempt to enhance the item, but fail catastrophically. The item is lost.
						case EnhanceResult.Failure: message = 1061082; break; // You attempt to enhance the item, but fail. Some material is lost in the process.
						case EnhanceResult.Success: message = 1061008; break; // You enhance the item with the properties of the special material.
						case EnhanceResult.NoSkill: message = 1044153; break; // You don't have the required skills to attempt this item.
                        case EnhanceResult.NoResources: message = 1032593; break; // Nie posiadasz wystarczajacej ilosci jednego z wybranych skladnikow.
					}

					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, message ) );
				}
			}
		}
	}
}