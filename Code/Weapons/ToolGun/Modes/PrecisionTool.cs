[Icon( "📐" )]
[Title( "#tool.name.precision" )]
[ClassName( "precision" )]
[Group( "#tool.group.tools" )]
public class PrecisionTool : ToolMode
{
	[Property, Sync]
	public float MoveSpeed { get; set; } = 1f;

	[Property, Sync]
	public float RotateSpeed { get; set; } = 1f;

	[Property, Sync]
	public bool SnapToGrid { get; set; } = true;

	[Property, Sync]
	public float SnapSize { get; set; } = 1f;

	GameObject _target;
	Vector3 _offset;
	Rotation _rotOffset;
	bool _grabbed;
	bool _rotateMode;

	public override string Description => _grabbed ? "#tool.hint.precision.move" : "#tool.hint.precision.select";
	public override string PrimaryAction => _grabbed ? "#tool.hint.precision.drop" : "#tool.hint.precision.grab";
	public override string SecondaryAction => "#tool.hint.precision.toggle";
	public override string ReloadAction => "#tool.hint.precision.reset";

	public override bool AbsorbMouseInput => _grabbed;

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();

		if ( Input.Pressed( "attack2" ) )
		{
			_rotateMode = !_rotateMode;
			return;
		}

		if ( Input.Pressed( "attack1" ) )
		{
			if ( !_grabbed )
			{
				if ( select.IsValid() && !select.IsWorld && !select.IsPlayer )
				{
					_target = select.GameObject.Network?.RootGameObject ?? select.GameObject;
					_offset = _target.WorldTransform.ToLocal( select.WorldTransform() ).Position;
					_rotOffset = _target.WorldTransform.Rotation.Inverse * select.WorldTransform().Rotation;
					_grabbed = true;
					ShootEffects( select );
				}
			}
			else
			{
				_grabbed = false;
				_target = null;
				ShootEffects( select );
			}
			return;
		}

		if ( Input.Pressed( "reload" ) )
		{
			if ( _grabbed && _target.IsValid() )
			{
				_target.WorldTransform = new Transform( _target.WorldPosition, Rotation.Identity, _target.WorldScale );
			}
			return;
		}

		if ( _grabbed && _target.IsValid() )
		{
			var player = Toolgun?.Owner;
			if ( !player.IsValid() ) return;

			var look = player.Controller.EyeAngles;
			var forward = look.Forward;
			var right = look.ToRotation().Right;
			var up = Vector3.Up;

			var move = Vector3.Zero;
			if ( Input.Down( "forward" ) ) move += forward;
			if ( Input.Down( "back" ) ) move -= forward;
			if ( Input.Down( "left" ) ) move -= right;
			if ( Input.Down( "right" ) ) move += right;
			if ( Input.Down( "jump" ) ) move += up;
			if ( Input.Down( "duck" ) ) move -= up;

			move = move.Normal * MoveSpeed * Time.Delta;

			if ( _rotateMode )
			{
				var delta = Input.AnalogLook;
				var rot = Rotation.From( delta.pitch * RotateSpeed * Time.Delta, delta.yaw * RotateSpeed * Time.Delta, 0 );
				_target.WorldRotation = rot * _target.WorldRotation;
			}
			else
			{
				var newPos = _target.WorldPosition + move;
				if ( SnapToGrid )
				{
					newPos = SnapTo( newPos, SnapSize );
				}
				_target.WorldPosition = newPos;
			}

			IsValidState = true;
		}
		else
		{
			IsValidState = select.IsValid() && !select.IsWorld && !select.IsPlayer;
		}
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();
		_grabbed = false;
		_target = null;
	}

	static Vector3 SnapTo( Vector3 pos, float snap )
	{
		return new Vector3(
			MathF.Round( pos.x / snap ) * snap,
			MathF.Round( pos.y / snap ) * snap,
			MathF.Round( pos.z / snap ) * snap
		);
	}
}
