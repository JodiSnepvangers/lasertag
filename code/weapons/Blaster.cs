using Sandbox;
using System;

[Spawnable]
[Library( "weapon_blaster", Title = "Blaster" )]
partial class Blaster : EnergyWeapon
{
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	public override float PrimaryRate => 4;
	public override float SecondaryRate => 1;
	public override float ReloadTime => 0.5f;

	public override float EnergyCost => 1;

	[Net, Predicted]
	public float Charge { get; set; } = 0f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl" );
	}

	public override void AttackPrimary()
	{
		if(Charge >= 99)
		{
			(Owner as AnimatedEntity)?.SetAnimParameter( "b_attack", true );

			//
			// Tell the clients to play the shoot effects
			//
			ShootEffects();
			PlaySound( "rust_pumpshotgun.shoot" );

			//
			// Shoot the bullets
			//
			ShootBullets( 30, 0.4f, 10.0f, 9.0f, 3.0f );
			Charge = 0;
		}
		
	}

	public override void AttackSecondary()
	{
		TimeSincePrimaryAttack = -0.5f;
		TimeSinceSecondaryAttack = -0.5f;

		(Owner as AnimatedEntity)?.SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		DoubleShootEffects();
		PlaySound( "rust_pumpshotgun.shootdouble" );

		//
		// Shoot the bullets
		//
		ShootBullet(0.04f, 50.0f, 40.0f, 5.0f );
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );
		if(Input.Down(InputButton.PrimaryAttack) && !Overheat)
		{
			if(Charge < 99)
			{

				Charge += Math.Min(2f, 99 - Charge);
				DrainEnergy();
				EnergyRechargeTimer = 0;
			}
		} else
		{
			Charge = 0;
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

	[ClientRpc]
	protected virtual void DoubleShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimParameter( "fire_double", true );
	}

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		FinishReload();
	}

	[ClientRpc]
	protected virtual void FinishReload()
	{
		ViewModelEntity?.SetAnimParameter( "reload_finished", true );
	}

	public override void SimulateAnimator( CitizenAnimationHelper anim )
	{
		anim.HoldType = CitizenAnimationHelper.HoldTypes.Shotgun;
		anim.Handedness = CitizenAnimationHelper.Hand.Both;
		anim.AimBodyWeight = 1.0f;
	}
}
