using Sandbox.Rendering;

[Icon( "⚡" )]
[Title( "#tool.name.wire" )]
[ClassName( "wire" )]
[Group( "#tool.group.wiremod" )]
public class WireTool : ToolMode
{
	WirePort _sourcePort;
	WireComponent _sourceComponent;
	bool _hasSource;

	public override string Description => _hasSource
		? "#tool.hint.wire.target"
		: "#tool.hint.wire.source";

	public override string PrimaryAction => _hasSource
		? "#tool.hint.wire.connect"
		: "#tool.hint.wire.select";

	public override string ReloadAction => "#tool.hint.wire.disconnect";

	public override string SecondaryAction => "#tool.hint.wire.clear";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		var wireComp = select.IsValid() ? select.GameObject.GetComponent<WireComponent>( true ) : null;

		if ( Input.Pressed( "attack2" ) )
		{
			_hasSource = false;
			_sourcePort = null;
			_sourceComponent = null;
			IsValidState = false;
			return;
		}

		if ( Input.Pressed( "reload" ) )
		{
			if ( wireComp != null )
			{
				wireComp.DisconnectAll();
				ShootEffects( select );
			}
			return;
		}

		if ( Input.Pressed( "attack1" ) )
		{
			if ( !_hasSource )
			{
				if ( wireComp != null )
				{
					var port = FindNearestPort( wireComp, select, false );
					if ( port != null )
					{
						_sourcePort = port;
						_sourceComponent = wireComp;
						_hasSource = true;
						ShootEffects( select );
					}
				}
			}
			else
			{
				if ( wireComp != null && wireComp != _sourceComponent )
				{
					var targetPort = FindNearestPort( wireComp, select, true );
					if ( targetPort != null )
					{
						_sourcePort.ConnectTo( targetPort );
						ShootEffects( select );
					}
				}
				_hasSource = false;
				_sourcePort = null;
				_sourceComponent = null;
			}
			return;
		}

		IsValidState = wireComp != null;
	}

	WirePort FindNearestPort( WireComponent comp, SelectionPoint select, bool input )
	{
		var ports = input ? comp.Inputs : comp.Outputs;
		WirePort best = null;
		float bestDist = float.MaxValue;

		foreach ( var kvp in ports )
		{
			var dist = ( comp.WorldPosition - select.WorldPosition() ).Length;
			if ( dist < bestDist )
			{
				bestDist = dist;
				best = kvp.Value;
			}
		}

		return best;
	}

	public override void DrawHud( HudPainter painter, Vector2 crosshair )
	{
		base.DrawHud( painter, crosshair );

		if ( _hasSource && _sourcePort != null && _sourceComponent != null )
		{
			var sourcePos = _sourceComponent.WorldPosition;
			var player = Toolgun?.Owner;
			if ( player.IsValid() )
			{
				var endPos = player.EyeTransform.Position + player.EyeTransform.Rotation.Forward * 200f;
				DebugOverlay.Line( sourcePos, endPos, Color.Yellow, 0.1f );
			}
		}
	}
}
