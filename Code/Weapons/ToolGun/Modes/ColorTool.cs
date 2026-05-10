[Icon( "🎨" )]
[Title( "#tool.name.color" )]
[ClassName( "color" )]
[Group( "#tool.group.render" )]
public class ColorTool : ToolMode
{
	[Property, Sync]
	public Color Tint { get; set; } = Color.White;

	[Property, Sync, Range( 0, 1 )]
	public float Alpha { get; set; } = 1f;

	[Property, Sync]
	public bool ApplyToAll { get; set; } = true;

	public override string Description => "#tool.hint.color.description";
	public override string PrimaryAction => "#tool.hint.color.apply";
	public override string SecondaryAction => "#tool.hint.color.copy";
	public override string ReloadAction => "#tool.hint.color.reset";

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
