using Sandbox;
using Sandbox.Component;

[Spawnable]
[Library( "weapon_lasergun", Title = "Laser Gun" )]
partial class LaserGun : EnergyWeapon
{
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float EnergyCost => 5.0f;
	public override float EnergyOverheatMultiplier => 0.2f;

	public override float PrimaryRate => 10.0f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
	}

	public override void AttackPrimary() 
	{
		if ( Overheat )
		{

		} else
		{

			(Owner as AnimatedEntity)?.SetAnimParameter( "b_attack", true );

			ShootEffects();
			PlaySound( "rust_smg.shoot" );

			if( EnergyRechargeTimer > 0.5)
			{
				ShootBullet( 0.0f, 1.5f, 5.0f, 3.0f );
			} else
			{
				ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );
			}
			EnergyRechargeTimer = 0;

			DrainEnergy();
		}
	}

	

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );


		ViewModelEntity?.SetAnimParameter( "fire", true );
	}

	public override void SimulateAnimator( CitizenAnimationHelper anim )
	{
		anim.HoldType = CitizenAnimationHelper.HoldTypes.Rifle;
		anim.Handedness = CitizenAnimationHelper.Hand.Both;
		anim.AimBodyWeight = 1.0f;
	}
}
