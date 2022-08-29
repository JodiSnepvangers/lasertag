using System.Collections.Generic;

namespace Sandbox
{
	public partial class EnergyWeapon : Weapon
	{
		List<BeamContainer> BeamList = new();

		float BeamLifetime => 0.2f;


		Color Color = Color.Magenta;

		public void CreateEffect( Vector3 pos )
		{
			Player player = (Player)Owner;
			BeamContainer container = new BeamContainer(Particles.Create( "particles/physgun_beam_red.vpcf", player.Position ));
			container.Beam.SetEntityAttachment( 0, player.ActiveChild, "muzzle", false);
			container.Beam.SetPosition( 1, pos );
			BeamList.Add( container );
		}

		private void SimulateBeam()
		{
			List<BeamContainer> removed = new();
			foreach (BeamContainer container in BeamList)
			{
				if ( container.BeamCreated > BeamLifetime )
				{
					container.DestroyBeam();
					removed.Add( container );
				}
			}
			
			foreach (BeamContainer container in removed )
			{
				BeamList.Remove( container );
			}
		}
	}

	class BeamContainer
	{
		public Particles Beam { get; }
		public TimeSince BeamCreated { get; }

		public BeamContainer(Particles Beam)
		{
			this.Beam = Beam;
			BeamCreated = 0;
		}

		public void DestroyBeam()
		{
			Beam.Destroy( true );
			Beam.Dispose();
		}
	}
}
