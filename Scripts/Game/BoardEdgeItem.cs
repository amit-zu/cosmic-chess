using Godot;

public partial class BoardEdgeItem : Area2D
{
    public EdgeCoordinate Coordinate { get; set; }
    public BoardEdgeItem ConnectedEdgeItem { get; set; }
    public virtual void Destroy()
    {
        QueueFree();
    }
}