using Sandbox;
using Sandbox.Component;
using System;

[Spawnable]
[Library( "weapon_chaingun", Title = "Chain Gun" )]
partial class Chaingun : EnergyWeapon
{
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float EnergyCost => 1.0f;
	public override float EnergyRechargeDelay => 2.5f;
	public override float EnergyOverheatMultiplier => 0.1f;

	public override float PrimaryRate => 15.0f;


	[Net, Predicted]
	public float ChainRate { get; set; } = -0.2f;

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

			ShootBullet( 0.2f + ChainRate, 1.5f, 5.0f, 3.0f );
			EnergyRechargeTimer = 0;

			TimeSincePrimaryAttack = ChainRate;
			ChainRate += Math.Min(0.02f, 0 - ChainRate);

			DrainEnergy();
		}
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );
		if ( (!Input.Down(InputButton.PrimaryAttack) || Overheat) && ChainRate != -0.2 )
		{
			ChainRate = -0.2f;
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
