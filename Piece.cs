namespace Szachy;

public class Piece : IPiece
{
    //
    // Właściwości figury
    public string Tag { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public Point Current_pos { get; private set; } = new Point();
    public List<Point>? Valid_moves { get; private set; } = new List<Point>();
    public bool Moved { get; private set; } = false;

    public Piece(string tag, Point position)
    {
        Tag = tag;
        var type = (tag.Remove(0,5));
        var color = (tag.Remove(5, tag.Length-5));
        type = type.Remove(type.Length - 1, 1);
        Color = color;
        Type = type;
        Current_pos = position;
        Valid_moves = new List<Point>();
    }

    public void SetValidMoves()
    {
        Type type = typeof(Moves);

        MethodInfo? mi = type.GetMethod(Type);

        if (mi == null)
            return;

        Valid_moves = (List<Point>?)mi.Invoke(null, new object[] {this, true});
    }

    public void MakeMove(Point targetPos)
    {
        Current_pos = targetPos;
    }

    public void SetPieceMoved(bool moved)
    {
        Moved = moved;
    }
}