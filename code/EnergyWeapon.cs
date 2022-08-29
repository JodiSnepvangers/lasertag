using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public partial class EnergyWeapon : Weapon
	{

		[Net, Predicted]
		public float Energy { get; set; } = 100.0f;
		[Net, Predicted]
		public bool Overheat { get; set; } = false;
		[Net, Predicted]
		public TimeSince EnergyRechargeTimer { get; set; }



		public virtual float MaxEnergy => 100.0f;
		public virtual float EnergyCost => 10.0f;
		public virtual float EnergyRechargePerTick => 2.0f;
		public virtual float EnergyRechargeDelay => 2.0f;
		public virtual float EnergyOverheatMultiplier => 0.5f;

		public void DrainEnergy()
		{
			Energy -= Math.Min(EnergyCost, Energy);
			if(Energy <= 0)
			{
				Overheat = true;
			}
		}

		public override bool CanPrimaryAttack()
		{
			return base.CanPrimaryAttack() && !Overheat;
		}

		public override void Simulate( Client owner )
		{
			base.Simulate( owner );
			SimulateBeam();
			if ( EnergyRechargeTimer > EnergyRechargeDelay && Energy < MaxEnergy)
			{
				if(Overheat)
				{
					Energy += Math.Min( EnergyRechargePerTick * EnergyOverheatMultiplier, MaxEnergy - Energy );
				} else
				{
					Energy += Math.Min( EnergyRechargePerTick, MaxEnergy - Energy );
				}
			}

			if ( Energy >= MaxEnergy )
			{
				Overheat = false;
			}
		}

		public override void ShootBullet( Vector3 pos, Vector3 dir, float spread, float force, float damage, float bulletSize )
		{
			var forward = dir;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			Vector3 end = pos + forward * 5000;
			var trArray = TraceBulletOnce( pos, end, bulletSize );
			foreach ( var tr in trArray )
			{
				if ( IsServer && tr.Hit && tr.Entity.IsValid() )
				{
					tr.Surface.DoBulletImpact( tr );

					//
					// We turn predictiuon off for this, so any exploding effects don't get culled etc
					//
					using ( Prediction.Off() )
					{
						var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
							.UsingTraceResult( tr )
							.WithAttacker( Owner )
							.WithWeapon( this );

						tr.Entity.TakeDamage( damageInfo );
					}

					CreateEffect( tr.EndPosition );
				} else
				{
					CreateEffect( tr.EndPosition );
				}
			}


		}

		public virtual IEnumerable<TraceResult> TraceBulletOnce( Vector3 start, Vector3 end, float radius = 2.0f )
		{
			bool underWater = Trace.TestPoint( start, "water" );

			var trace = Trace.Ray( start, end )
					.UseHitboxes()
					.WithAnyTags( "solid", "player", "npc", "glass" )
					.Ignore( this )
					.Size( radius );

			var tr = trace.Run();

			if ( !underWater )
				trace = trace.WithAnyTags( "water" );

			yield return tr;
		}
	}
}
