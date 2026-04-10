using Godot;

public partial class Portal : BoardEdgeItem
{
	public AnimatedSprite2D portalSprite;

	public override void _Ready()
	{
		portalSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		portalSprite.Play("Open");
		portalSprite.AnimationFinished += OnPortalOpened;
	}

	public void OnPortalOpened()
	{
		portalSprite.Play("Idle");
	}

	public override void Destroy()
	{
		ClosePortal();
		((Portal)ConnectedEdgeItem).ClosePortal();
	}

	public void ClosePortal()
	{
		portalSprite.AnimationFinished -= OnPortalOpened;
		portalSprite.Play("Close");
		portalSprite.AnimationFinished += OnPortalClosed;
	}

	private void OnPortalClosed()
	{
		QueueFree();
	}
}
