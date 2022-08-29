using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Energy : Panel
{
	public Label Label;

	public Energy()
	{
		Label = Add.Label( "100", "value" );
	}

	public override void Tick()
	{
		var player = (Player) Local.Pawn;
		if ( player == null ) return;

		EnergyWeapon weapon = player.ActiveChild as EnergyWeapon;
		if ( weapon == null ) return;

		Label.SetClass( "overheat", weapon.Overheat);
		Label.Text = $"{weapon.Energy.CeilToInt()}";
	}
}
