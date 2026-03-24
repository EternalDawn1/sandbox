using Sandbox.UI;

namespace Sandbox;

public abstract class InspectorEditorBase : Panel
{
	public abstract bool TrySetTarget( List<GameObject> selection );

	public bool IsExpanded
	{
		get => _isExpanded;
		set { _isExpanded = value; StateHasChanged(); }
	}
	bool _isExpanded;

	public bool ShowAccordion
	{
		get => _showAccordion;
		set { _showAccordion = value; StateHasChanged(); }
	}
	bool _showAccordion;

	public Action OnToggleExpanded { get; set; }
	public void ToggleExpanded() => OnToggleExpanded?.Invoke();
}
