namespace Szachy;

public interface IPiece
{
    void SetValidMoves();
    void SetPieceMoved(bool moved);
    void MakeMove(Point point);
}