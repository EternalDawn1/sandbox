[Icon( "🎨" )]
[Title( "Color" )]
[ClassName( "color" )]
[Group( "Render" )]
[Description( "Change the color/tint of props. Click to apply color, right-click to copy color from a prop." )]
public class ColorTool : ToolMode
{
	[Property, Sync]
	public Color Tint { get; set; } = Color.White;

	[Property, Sync, Range( 0, 1 )]
	public float Alpha { get; set; } = 1f;

	[Property, Sync]
	public bool ApplyToAll { get; set; } = true;

	public override string Description => "Click to apply color to prop";
	public override string PrimaryAction => "Apply Color";
	public override string SecondaryAction => "Copy Color";
	public override string ReloadAction => "Reset Color";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		IsValidState = select.IsValid() && !select.IsWorld && !select.IsPlayer;

		if ( !IsValidState ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			ApplyColor( select.GameObject, Tint, Alpha, ApplyToAll );
			ShootEffects( select );
			return;
		}

		if ( Input.Pressed( "attack2" ) )
		{
			CopyColor( select.GameObject );
			ShootEffects( select );
			return;
		}

		if ( Input.Pressed( "reload" ) )
		{
			ResetColor( select.GameObject, ApplyToAll );
			ShootEffects( select );
		}
	}

	[Rpc.Host]
	public void ApplyColor( GameObject go, Color color, float alpha, bool applyToAll )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var root = go.Network?.RootGameObject ?? go;
		var renderers = root.GetComponentsInChildren<ModelRenderer>( true );
		var targetColor = color.WithAlpha( alpha );

		foreach ( var renderer in renderers )
		{
			if ( !renderer.IsValid() ) continue;
			renderer.Tint = targetColor;
		}

		root.Network?.Refresh();
	}

	[Rpc.Host]
	public void CopyColor( GameObject go )
	{
		if ( !go.IsValid() ) return;

		var renderer = go.GetComponent<ModelRenderer>()
			?? go.GetComponentInParent<ModelRenderer>( true )
			?? go.GetComponentInChildren<ModelRenderer>( true );

		if ( !renderer.IsValid() ) return;

		var tint = renderer.Tint;
		SetColor( tint, tint.a );
	}

	[Rpc.Host]
	public void ResetColor( GameObject go, bool applyToAll )
	{
		if ( !go.IsValid() || go.IsProxy ) return;

		var root = go.Network?.RootGameObject ?? go;
		var renderers = root.GetComponentsInChildren<ModelRenderer>( true );

		foreach ( var renderer in renderers )
		{
			if ( !renderer.IsValid() ) continue;
			renderer.Tint = Color.White;
		}

		root.Network?.Refresh();
	}

	[Rpc.Owner]
	void SetColor( Color color, float alpha )
	{
		Tint = color;
		Alpha = alpha;
	}
}
