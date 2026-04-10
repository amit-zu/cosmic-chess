using System;
using System.Collections.Generic;
using Godot;

public partial class DialogueLine
{
    public string SpeakerName;
    public string Text;
    public Texture2D Portrait;
    public bool Skippable = true;
    public Shader? Shader = null;
    public Dictionary<string, Variant>? ShaderParams = null;
}