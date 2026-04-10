using Godot;

[GlobalClass]
public partial class SpecialPowerIcon : TextureRect
{
	protected bool hoveringOverCurrentItem = false;
	private GameScreen gameScreen;
	private bool isEnabled = true;
	private ShaderMaterial highlightShaderMaterial;
	private ShaderMaterial greyScaleShaderMaterial;

	public override void _Ready()
	{
		SetUpShaders();
		SetToHighlight();

		gameScreen = Util.FindChild<GameScreen>(GetTree().Root);

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	public override void _Process(double delta)
	{
		if (hoveringOverCurrentItem)
		{
			if (Input.IsActionJustPressed("click") && isEnabled && hoveringOverCurrentItem)
			{
				gameScreen.OnSpecialPowerIconClick(GetType());
			}
		}
	}

	public void Disable()
	{
		isEnabled = false;

		highlightShaderMaterial.SetShaderParameter("outline_size", 0.0f);
		SetToGreyScale();
	}

	public void Enable()
	{
		isEnabled = true;
		SetToHighlight();
	}

	private void SetUpShaders()
	{
		Shader highlightShader = GD.Load<Shader>("res://Scripts/Shaders/highlight.gdshader");
		Shader greyScaleShader = GD.Load<Shader>("res://Scripts/Shaders/grey_out.gdshader");

		if (highlightShader == null)
		{
			GD.PrintErr("Failed to load highlight shader!");
			return;
		}
		else if (greyScaleShader == null)
		{
			GD.PrintErr("Failed to load greyScale shader!");
			return;
		}

		highlightShaderMaterial = new ShaderMaterial();
		highlightShaderMaterial.Shader = highlightShader;
		greyScaleShaderMaterial = new ShaderMaterial();
		greyScaleShaderMaterial.Shader = greyScaleShader;
	}

	private void SetToGreyScale()
	{
		if (greyScaleShaderMaterial == null)
		{
			GD.PrintErr("GreyScale shader is null!");
			return;
		}

		greyScaleShaderMaterial.SetShaderParameter("desat", 0.5f);
		greyScaleShaderMaterial.SetShaderParameter("dim", 0.8f);

		Material = greyScaleShaderMaterial;
	}

	private void SetToHighlight()
	{
		if (highlightShaderMaterial == null)
		{
			GD.PrintErr("Highlight shader is null!");
			return;
		}

		highlightShaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1, 1));
		highlightShaderMaterial.SetShaderParameter("outline_size", 0.0f);

		Material = highlightShaderMaterial;
	}

	private void OnMouseEntered()
	{
		if (highlightShaderMaterial != null && isEnabled)
		{
			highlightShaderMaterial.SetShaderParameter("outline_size", 5.0f);
		}

		hoveringOverCurrentItem = true;
	}

	private void OnMouseExited()
	{
		if (highlightShaderMaterial != null)
		{
			highlightShaderMaterial.SetShaderParameter("outline_size", 0.0f);
		}

		hoveringOverCurrentItem = false;
	}
}
