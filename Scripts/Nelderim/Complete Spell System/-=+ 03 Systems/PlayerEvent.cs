using Server.Items;

namespace Server.ACC.CSS.Systems
{
	public class PlayerEvent
	{
		public delegate void OnWeaponHit( Mobile attacker, Mobile defender, int damage, WeaponAbility a );
		public static event OnWeaponHit HitByWeapon;

		public static void InvokeHitByWeapon( Mobile attacker, Mobile defender, int damage, WeaponAbility a )
		{
			if ( HitByWeapon != null )
				HitByWeapon( attacker, defender, damage, a );
		}
	}
}