namespace Szachy;

public static class Moves
{
    // Zmienne pomocnicze
    private static Piece blackKing = MainWindow.wKing;
    private static Piece whiteKing = MainWindow.bKing;

    //
    // Funkcje odpowiedzialne za walidację poszczególnych ruchów figury

    private static int MoveValidation(Point move, Piece piece, bool checkCheck)
    {
        Point oldPos = piece.Current_pos;
        Point newPos = new Point(piece.Current_pos.X + move.X, piece.Current_pos.Y + move.Y);

        Piece? targetPiece = (Piece?)MainWindow.listOfPieces.FirstOrDefault(p => (p as Piece)?.Current_pos == newPos);
        Piece? king = (Piece?)(MainWindow.listOfPieces.FirstOrDefault(p => ((p as Piece)?.Tag)!.ToString().Contains($"{piece.Color}King")));

        if (king == null)
            king = piece.Color == "White" ? whiteKing : blackKing;

        Point kingPos = king.Current_pos;

        var check = false;

        if (newPos == kingPos)
            return 0;

        if (checkCheck)
        {
            //tymczasowe usunięcie figury
            if (targetPiece != null)
                MainWindow.listOfPieces.Remove(targetPiece);
            //pozycja naszej figury w tablicy figur
            var index = MainWindow.listOfPieces.IndexOf(piece);
            //przesunięcie naszej figury na docelową pozycję
            piece.MakeMove(newPos);
            //zaktualizowanie naszej przesuniętej figury w tablicy
            MainWindow.listOfPieces[index] = piece;
            //Jeśli ruszyliśmy się królem, w CheckForCheck() musimy zaktualizować pozycję króla
            if(oldPos == kingPos)
                kingPos = (MainWindow.listOfPieces.FirstOrDefault(p => (p as Piece).Tag.Contains($"{piece.Color}King")) as Piece).Current_pos;
            //sprawdzenie szachu

            check = CheckForCheck(piece.Color, kingPos);

            //przywrócenie usuniętej figury
            if (targetPiece != null)
                MainWindow.listOfPieces.Add(targetPiece);
            //przesunięcie naszej figury na początkową pozycję
            piece.MakeMove(oldPos);
            //zaktualizowanie naszej figury
            MainWindow.listOfPieces[index] = piece;
        }

        if (blackKing == null)
            MainWindow.listOfPieces.Add(blackKing);
        if (whiteKing == null)
            MainWindow.listOfPieces.Add(whiteKing);

        if (check)
            return -1;
        else if (targetPiece == null)
            return 1;
        else if (targetPiece.Color != piece.Color)
            return 2;

        return 0;
    }

    private static bool CheckForCheck(string color, Point kingPos)
    {
        foreach (var piece in MainWindow.listOfPieces.ToList())
        {
            List<Point> validEnemyMoves = new List<Point>();

            var checkedPiece = piece as Piece;

            if(checkedPiece.Color != color)
            {
                Type type = typeof(Moves);

                MethodInfo? mi = type.GetMethod(checkedPiece.Type);

                if (mi != null)
                {
                    validEnemyMoves = (List<Point>)mi.Invoke(null, new object[] { checkedPiece, false });
                }

                foreach(var enemyMove in validEnemyMoves.ToList())
                {
                    if (enemyMove == kingPos)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //
    // Funkcje odpowiedzialne za ustalenie możliwych ruchów figury

    public static List<Point> Pawn(Piece piece, bool checkCheck)
    {
        List<Point> validMoves = new List<Point>();
        var t = piece.Color == "White" ? -1 : 1;

        if(piece.Current_pos.Y > 0 && piece.Current_pos.Y < 7)
        {
            //Ruch do przodu
            if (MoveValidation(new Point(0,t), piece, checkCheck) == 1)
            {
                //Ruch o jedno pole
                validMoves.Add(new Point(piece.Current_pos.X, piece.Current_pos.Y + t));

                //Ruch o dwa pola
                if (MoveValidation(new Point(0, 2*t), piece, checkCheck) == 1 && piece.Current_pos.Y == (7+t)%7)
                    validMoves.Add(new Point(piece.Current_pos.X, piece.Current_pos.Y + 2 * t));
            }

            //Bicie
            if (piece.Current_pos.X > 0)
                if (MoveValidation(new Point(-1, t), piece, checkCheck) == 2)
                    validMoves.Add(new Point(piece.Current_pos.X - 1, piece.Current_pos.Y + t));
            if (piece.Current_pos.X < 7)
                if (MoveValidation(new Point(1, t), piece, checkCheck) == 2)
                    validMoves.Add(new Point(piece.Current_pos.X + 1, piece.Current_pos.Y + t));
        }

        return validMoves;
    }

    public static List<Point> Bishop(Piece piece, bool checkCheck)
    {
        List<Point> validMoves = new List<Point>();
        List<Point> directions = new List<Point>() { new Point(-1, -1), new Point(-1, 1), new Point(1, -1), new Point(1, 1) };

        foreach (var dir in directions)
        {
            for (var i = 1; i < 8; i++)
            {
                var newPos = new Point(piece.Current_pos.X + i * dir.X, piece.Current_pos.Y + i * dir.Y);

                if (newPos.Y >= 0 && newPos.Y < 8 && newPos.X >= 0 && newPos.X < 8)
                {
                    var canMove = MoveValidation(new Point(i * dir.X, i * dir.Y), piece, checkCheck);

                    if (canMove > 0)
                        validMoves.Add(new Point(newPos.X, newPos.Y));

                    if (canMove != 1 && canMove != -1)
                        break;
                }
                else
                    break;
            }
        }
        return validMoves;

    }

    public static List<Point> Knight(Piece piece, bool checkCheck)
    {
        List<Point> validMoves = new List<Point>();
        List<Point> directions = new List<Point>() { new Point(-1, 2), new Point(-1, -2), new Point(-2, 1), new Point(-2, -1), new Point(1, -2), new Point(1, 2), new Point(2, -1), new Point(2, 1) };

        foreach (var dir in directions)
        {
            var newPos = new Point(piece.Current_pos.X + dir.X, piece.Current_pos.Y + dir.Y);
            if (newPos.Y >= 0 && newPos.Y < 8 && newPos.X >= 0 && newPos.X < 8)
            {
                var canMove = MoveValidation(new Point(dir.X, dir.Y), piece, checkCheck);

                if (canMove > 0)
                    validMoves.Add(new Point(newPos.X, newPos.Y));
            }
        }

        return validMoves;
    }

    public static List<Point> Rook(Piece piece, bool checkCheck)
    {
        List<Point> validMoves = new List<Point>();
        List<Point> directions = new List<Point>() { new Point(-1, 0), new Point(0, 1), new Point(0, -1), new Point(1, 0) };

        foreach (var dir in directions)
        {
            for (var i = 1; i < 8; i++)
            {
                var newPos = new Point(piece.Current_pos.X + i * dir.X, piece.Current_pos.Y + i * dir.Y);

                if (newPos.Y >= 0 && newPos.Y < 8 && newPos.X >= 0 && newPos.X < 8)
                {
                    var canMove = MoveValidation(new Point(i * dir.X, i * dir.Y), piece, checkCheck);

                    if (canMove > 0)
                        validMoves.Add(new Point(newPos.X, newPos.Y));

                    if (canMove != 1 && canMove != -1)
                        break;
                }
                else
                    break;
            }
        }
        return validMoves;

    }

    public static List<Point> Queen(Piece piece, bool checkCheck)
    {
        List<Point> validMoves = new List<Point>();
        List<Point> directions = new List<Point>() { new Point(-1, -1), new Point(-1, 1), new Point(1, -1), new Point(1, 1),
                                                    new Point(-1, 0), new Point(0, 1), new Point(0, -1), new Point(1, 0) };

        foreach (var dir in directions)
        {
            for (var i = 1; i < 8; i++)
            {
                var newPos = new Point(piece.Current_pos.X + i * dir.X, piece.Current_pos.Y + i * dir.Y);

                if (newPos.Y >= 0 && newPos.Y < 8 && newPos.X >= 0 && newPos.X < 8)
                {
                    var canMove = MoveValidation(new Point(i * dir.X, i * dir.Y), piece, checkCheck);

                    if (canMove > 0)
                        validMoves.Add(new Point(newPos.X, newPos.Y));

                    if (canMove != 1 && canMove != -1)
                        break;
                }
                else
                    break;
            }
        }

        return validMoves;
    }

    public static List<Point> King(Piece piece, bool checkCheck)
    {
        List<Point> validMoves = new List<Point>();

        List<Point> directions = new List<Point>() { new Point(-1, -1), new Point(-1, 1), new Point(1, -1), new Point(1, 1),
                                                    new Point(-1, 0), new Point(0, 1), new Point(0, -1), new Point(1, 0) };

        foreach (var dir in directions)
        {
            var newPos = new Point(piece.Current_pos.X + dir.X, piece.Current_pos.Y + dir.Y);

            if (newPos.Y >= 0 && newPos.Y < 8 && newPos.X >= 0 && newPos.X < 8)
            {
                var canMove = MoveValidation(new Point(dir.X, dir.Y), piece, checkCheck);

                if (canMove > 0)
                    validMoves.Add(new Point(newPos.X, newPos.Y));
            }
        }

        //ROSZADA
        //jeśli król się jeszcze nie ruszył
        if (!piece.Moved && checkCheck)
        {
            Piece longCastleRook = (Piece)MainWindow.listOfPieces.FirstOrDefault(p => (p as Piece).Current_pos == new Point(0, piece.Current_pos.Y));
            Piece shortCastleRook = (Piece)MainWindow.listOfPieces.FirstOrDefault(p => (p as Piece).Current_pos == new Point(7, piece.Current_pos.Y));

            var canLongCastle = false;
            var canShortCastle = false;

            //Długa roszada
            //Jeśli może ruszyć się w 

            if(longCastleRook != null)
                canLongCastle = (
                (MoveValidation(new Point(-1, 0), piece, checkCheck) == 1) &&
                (MoveValidation(new Point(-2, 0), piece, checkCheck) == 1) &&
                (MoveValidation(new Point(-3, 0), piece, checkCheck) == 1) &&
                (!longCastleRook.Moved));

            if(shortCastleRook != null)
                canShortCastle = 
                ((MoveValidation(new Point(1, 0), piece, checkCheck) == 1) &&
                (MoveValidation(new Point(2, 0), piece, checkCheck) == 1) &&
                (!shortCastleRook.Moved));

            if (canLongCastle)
                validMoves.Add(new Point(2, piece.Current_pos.Y));

            if (canShortCastle)
                validMoves.Add(new Point(6, piece.Current_pos.Y));
        }

        return validMoves;
    }

    //
    // Funkcje odpowiedzialne za ustalenie notacji wykonanego ruchu
    
    public static string MoveNotation(Piece piece, Point target)
    {
        if (piece == null)
            return "";

        Piece? targetPiece = (Piece?)MainWindow.listOfPieces.FirstOrDefault(p => (p as Piece)?.Current_pos == target);

        //Przejście
        if(targetPiece == null)
        {
            if(piece.Type == "Pawn")
                return $"{GetX((int)target.X)}{GetY((int)target.Y)}";
            
            return $"{PieceSymbol(piece)}{GetX((int)target.X)}{GetY((int)target.Y)}";
        }
        //Bicie
        else
        {
            return $"{PieceSymbol(piece)}x{GetX((int)targetPiece.Current_pos.X)}{GetY((int)targetPiece.Current_pos.Y)}";
        }
    }

    public static char PieceSymbol(Piece piece)
    {
        switch (piece.Type)
        {
            case "Pawn":
                return GetX((int)piece.Current_pos.X);
            case "Knight":
                return 'N';
            case "Bishop":
                return 'B';
            case "Rook":
                return 'R';
            case "Queen":
                return 'Q';
            case "King":
                return 'K';
            default:
                return '\0';
        }
    }

    private static char GetX(int posX)
    {
        char x = (char)(posX + 97);
        return x;
    }

    private static char GetY(int posY)
    {
        char y = (char)(posY + 49);
        return y;
    }
}