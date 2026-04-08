using Sandbox.UI;
using System.Linq;

[Title( "Materials" )]
[Icon( "🎨" )]
[ClassName( "materials" )]
[Group( "Render" )]
public class MaterialsTool : ToolMode
{
	public enum MaterialTargetMode
	{
		WholeObject,
		HitRenderer
	}

	[Property, ResourceSelect( Extension = "vmat", PackageType = "material", AllowPackages = true ), Title( "Material" ), Group( "Material" )]
	public string MaterialPath { get; set; }

	[Property, Sync, Title( "Target" ), Group( "Target" )]
	public MaterialTargetMode TargetMode { get; set; } = MaterialTargetMode.WholeObject;

	[Property, Sync, Title( "All Slots" ), Group( "Apply" )]
	public bool ApplyToAllSlots { get; set; } = true;

	[Property, Sync, Range( 0, 31 ), Title( "Slot" ), Group( "Apply" ), ShowIf( nameof( ApplyToAllSlots ), false )]
	public int Slot { get; set; }

	public override string Description => "Override materials on the prop you're aiming at.";
	public override string PrimaryAction => "Apply material";
	public override string SecondaryAction => "Copy target material";
	public override string ReloadAction => "Clear override";

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		var renderers = GetTargetRenderers( select.GameObject, TargetMode );

		IsValidState = select.IsValid() && !select.IsWorld && !select.IsPlayer && renderers.Count > 0;
		if ( !IsValidState )
			return;

		if ( Input.Pressed( "attack1" ) )
		{
			if ( string.IsNullOrWhiteSpace( MaterialPath ) )
				return;

			ApplyMaterial( select.GameObject, TargetMode, ApplyToAllSlots, Slot, MaterialPath );
			ShootEffects( select );
			return;
		}

		if ( Input.Pressed( "attack2" ) )
		{
			CopyMaterial( select.GameObject, TargetMode, ApplyToAllSlots, Slot );
			ShootEffects( select );
			return;
		}

		if ( Input.Pressed( "reload" ) )
		{
			ClearMaterial( select.GameObject, TargetMode, ApplyToAllSlots, Slot );
			ShootEffects( select );
		}
	}

	[Rpc.Host]
	public async void ApplyMaterial( GameObject hit, MaterialTargetMode targetMode, bool applyToAllSlots, int slot, string materialPath )
	{
		if ( !hit.IsValid() || string.IsNullOrWhiteSpace( materialPath ) )
			return;

		var material = Material.Load( materialPath );
		material ??= await Cloud.Load<Material>( materialPath );

		if ( material is null || !hit.IsValid() )
			return;

		var renderers = GetTargetRenderers( hit, targetMode );
		if ( renderers.Count == 0 )
			return;

		foreach ( var renderer in renderers )
		{
			if ( !renderer.IsValid() || renderer.IsProxy )
				continue;

			ApplyOverride( renderer, material, applyToAllSlots, slot );
		}

		RefreshRenderers( renderers );
	}

	[Rpc.Host]
	public void ClearMaterial( GameObject hit, MaterialTargetMode targetMode, bool applyToAllSlots, int slot )
	{
		if ( !hit.IsValid() )
			return;

		var renderers = GetTargetRenderers( hit, targetMode );
		if ( renderers.Count == 0 )
			return;

		foreach ( var renderer in renderers )
		{
			if ( !renderer.IsValid() || renderer.IsProxy )
				continue;

			ApplyOverride( renderer, null, applyToAllSlots, slot );
		}

		RefreshRenderers( renderers );
	}

	[Rpc.Host]
	public void CopyMaterial( GameObject hit, MaterialTargetMode targetMode, bool applyToAllSlots, int slot )
	{
		if ( !hit.IsValid() )
			return;

		var renderer = GetTargetRenderers( hit, targetMode ).FirstOrDefault( x => x.IsValid() );
		if ( !renderer.IsValid() )
			return;

		var accessor = renderer.Materials;
		if ( accessor.Count <= 0 )
			return;

		var index = applyToAllSlots ? 0 : Math.Clamp( slot, 0, accessor.Count - 1 );
		var material = accessor.HasOverride( index ) ? accessor.GetOverride( index ) : accessor.GetOriginal( index );
		SetSelectedMaterial( material?.ResourcePath );
	}

	[Rpc.Owner]
	void SetSelectedMaterial( string materialPath )
	{
		MaterialPath = materialPath;
	}

	static void ApplyOverride( ModelRenderer renderer, Material material, bool applyToAllSlots, int slot )
	{
		var accessor = renderer.Materials;
		if ( accessor.Count <= 0 )
			return;

		if ( applyToAllSlots )
		{
			for ( int i = 0; i < accessor.Count; i++ )
			{
				accessor.SetOverride( i, material );
			}

			return;
		}

		accessor.SetOverride( Math.Clamp( slot, 0, accessor.Count - 1 ), material );
	}

	static void RefreshRenderers( IEnumerable<ModelRenderer> renderers )
	{
		foreach ( var go in renderers
			.Where( x => x.IsValid() )
			.Select( x => x.GameObject?.Network?.RootGameObject ?? x.GameObject )
			.Where( x => x.IsValid() )
			.Distinct() )
		{
			go.Network?.Refresh();
		}
	}

	static List<ModelRenderer> GetTargetRenderers( GameObject hit, MaterialTargetMode targetMode )
	{
		if ( targetMode == MaterialTargetMode.HitRenderer )
		{
			var direct = ResolveNearestRenderer( hit );
			if ( direct.IsValid() )
				return new() { direct };

			return new();
		}

		var target = ResolveTargetObject( hit, targetMode );
		if ( !target.IsValid() )
			return new();

		return target.GetComponentsInChildren<ModelRenderer>( false, true )
			.Where( x => x.IsValid() )
			.Distinct()
			.ToList();
	}

	static GameObject ResolveTargetObject( GameObject hit, MaterialTargetMode targetMode )
	{
		if ( !hit.IsValid() )
			return null;

		if ( targetMode == MaterialTargetMode.HitRenderer )
		{
			var renderer = ResolveNearestRenderer( hit );
			if ( renderer.IsValid() )
				return renderer.GameObject;
		}

		return hit.Network?.RootGameObject ?? hit;
	}

	static ModelRenderer ResolveNearestRenderer( GameObject hit )
	{
		if ( !hit.IsValid() )
			return null;

		return hit.GetComponent<ModelRenderer>()
			?? hit.GetComponentInParent<ModelRenderer>( true )
			?? hit.GetComponentInChildren<ModelRenderer>( true );
	}
}
