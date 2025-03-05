namespace Szachy;

public partial class MainWindow : Window
{
    public static List<object> listOfPieces = new();
    public static Grid board = new();
    public static Piece wKing;
    public static Piece bKing;

    public static string move = string.Empty;

    Piece lastActivePiece = null;
    Image validMoveHighlight = new();
    Image currentPieceHighlight = new();
    BitmapImage validBitmapMove = new();
    BitmapImage currentBitmapPiece = new();

    private DispatcherTimer whiteTimer = new();
    private DispatcherTimer blackTimer = new();

    private TimeSpan whiteTimeRemaining = new();
    private TimeSpan blackTimeRemaining = new();

    private bool whitePromotion = false;
    private bool blackPromotion = false;

    private Canvas promotionPanel = new();

    //0 - White, 1 - Black
    int turn = 0;
    int promoID = 3;

    public MainWindow()
    {
        InitializeComponent();

        promotionPanel = PromotionPanel;
        
        listOfPieces = new()
        {
            //White Pieces
            new Piece("WhitePawn1", new Point(0,6)),
            new Piece("WhitePawn2", new Point(1,6)),
            new Piece("WhitePawn3", new Point(2,6)),
            new Piece("WhitePawn4", new Point(3,6)),
            new Piece("WhitePawn5", new Point(4,6)),
            new Piece("WhitePawn6", new Point(5,6)),
            new Piece("WhitePawn7", new Point(6,6)),
            new Piece("WhitePawn8", new Point(7,6)),

            new Piece("WhiteKnight1", new Point(1,7)),
            new Piece("WhiteKnight2", new Point(6,7)),
            new Piece("WhiteBishop1", new Point(2,7)),
            new Piece("WhiteBishop2", new Point(5,7)),
            new Piece("WhiteRook1", new Point(0,7)),
            new Piece("WhiteRook2", new Point(7,7)),

            new Piece("WhiteQueen1", new Point(3,7)),
            new Piece("WhiteKing1", new Point(4,7)),

            new Piece("BlackPawn1", new Point(0,1)),
            new Piece("BlackPawn2", new Point(1,1)),
            new Piece("BlackPawn3", new Point(2,1)),
            new Piece("BlackPawn4", new Point(3,1)),
            new Piece("BlackPawn5", new Point(4,1)),
            new Piece("BlackPawn6", new Point(5,1)),
            new Piece("BlackPawn7", new Point(6,1)),
            new Piece("BlackPawn8", new Point(7,1)),


            new Piece("BlackBishop1", new Point(2,0)),
            new Piece("BlackBishop2", new Point(5,0)),
            new Piece("BlackKnight1", new Point(1,0)),
            new Piece("BlackKnight2", new Point(6,0)),
            new Piece("BlackRook1", new Point(0,0)),
            new Piece("BlackRook2", new Point(7,0)),

            new Piece("BlackQueen1", new Point(3,0)),
            new Piece("BlackKing1", new Point(4,0))
        };
        
        PrepareTimers();
        PrepareImages();
        UpdateChessboard();
    }

    private void PrepareTimers()
    {
        whiteTimeRemaining = TimeSpan.FromMinutes(10);
        blackTimeRemaining = TimeSpan.FromMinutes(10);

        whiteTimer = new DispatcherTimer();
        blackTimer = new DispatcherTimer();

        whiteTimer.Interval = TimeSpan.FromSeconds(1);
        blackTimer.Interval = TimeSpan.FromSeconds(1);

        whiteTimer.Tick += WhiteTimer_Tick;
        blackTimer.Tick += BlackTimer_Tick;
    }

    private void WhiteTimer_Tick(object sender, EventArgs e)
    {
        whiteTimeRemaining = whiteTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
        WhiteTimer.Text = whiteTimeRemaining.ToString(@"mm\:ss");

        if (whiteTimeRemaining.TotalSeconds <= 0)
        {
            //EndGame();
            whiteTimer.Stop();
        }
    }

    private void BlackTimer_Tick(object sender, EventArgs e)
    {
        blackTimeRemaining = blackTimeRemaining.Subtract(TimeSpan.FromSeconds(1));

        BlackTimer.Text = blackTimeRemaining.ToString(@"mm\:ss");

        if (blackTimeRemaining.TotalSeconds <= 0)
        {
            //EndGame();
            blackTimer.Stop();
        }
    }

    private void PrepareImages()
    {
        validBitmapMove = new BitmapImage();
        var uriSource = new Uri(@"Assets/ValidMove.png", UriKind.Relative);
        validBitmapMove.BeginInit();
        validBitmapMove.UriSource = uriSource;
        validBitmapMove.EndInit();
        validMoveHighlight = new Image();
        validMoveHighlight.Tag = "ValidMoveHighlight";
        validMoveHighlight.Source = validBitmapMove;

        currentBitmapPiece = new BitmapImage();
        uriSource = new Uri(@"Assets/CurrentPiece.png", UriKind.Relative);
        currentBitmapPiece.BeginInit();
        currentBitmapPiece.UriSource = uriSource;
        currentBitmapPiece.EndInit();
        currentPieceHighlight = new Image();
        currentPieceHighlight.Tag = "CurrentPieceHighlight";
        currentPieceHighlight.Source = currentBitmapPiece;
    }

    private void UpdateChessboard()
    {
        board = Chessboard;
        Chessboard.Children.Clear();
        bKing = (Piece)listOfPieces.ToList().FirstOrDefault(p => (p as Piece).Tag == "BlackKing1");
        wKing = (Piece)listOfPieces.ToList().FirstOrDefault(p => (p as Piece).Tag == "WhiteKing1");

        if (whitePromotion)
        {
            ShowPromotionPanel("White");
            Grid.SetRow(promotionPanel, 0);
            Chessboard.Children.Add(promotionPanel);
        }
        if (blackPromotion)
        {
            ShowPromotionPanel("Black");
            Grid.SetRow(promotionPanel, 7);
            Chessboard.Children.Add(promotionPanel);
        }

        if (turn == 0)
        {
            WhiteTurn.Visibility = Visibility.Visible;
            BlackTurn.Visibility = Visibility.Hidden;
            whiteTimer.Start();
            blackTimer.Stop();
        }
        else
        {
            WhiteTurn.Visibility = Visibility.Hidden;
            BlackTurn.Visibility = Visibility.Visible;
            whiteTimer.Stop();
            blackTimer.Start();
        }

        for(var i = 0; i < 8; i++)
        {
            for(var j = 0; j < 8; j++)
            {
                Border border = new Border();
                border.Background = new SolidColorBrush(Colors.Transparent);
                border.MouseDown += Chessboard_MouseDown;
                Grid.SetRow(border, i);
                Grid.SetColumn(border, j);
                Chessboard.Children.Add(border);
            }
        }

        // Wyświetlenie wszystkich figur zapisanych w tablicy na odpowiednich miejscach na szachownicy
        //
        foreach (Piece piece in listOfPieces.ToList())
        {
            //Pobranie źródła obrazu
            var bitmapPieceImage = new BitmapImage();
            var path = $"Assets/{piece.Color}{piece.Type}.png";
            bitmapPieceImage.BeginInit();
            bitmapPieceImage.UriSource = new Uri(path, UriKind.Relative);
            bitmapPieceImage.EndInit();

            //Zapisanie źródła w postaci obrazka wyświetlanego na szachownicy
            //Obrazek ten reprezentuje odpowiednią figurę zapisaną w list_of_pieces
            Image pieceImage = new Image();
            pieceImage.Tag = piece.Tag;
            pieceImage.Source = bitmapPieceImage;
            Grid.SetRow(pieceImage, (int)piece.Current_pos.Y);
            Grid.SetColumn(pieceImage, (int)piece.Current_pos.X);
            Chessboard.Children.Add(pieceImage);
        }

        RotateGrid();

        if (lastActivePiece == null)
            return;

        Grid.SetRow(currentPieceHighlight, (int)lastActivePiece.Current_pos.Y);
        Grid.SetColumn(currentPieceHighlight, (int)lastActivePiece.Current_pos.X);
        Grid.SetZIndex(currentPieceHighlight, -1);
        Chessboard.Children.Add(currentPieceHighlight);

        lastActivePiece.SetValidMoves();

        if (lastActivePiece.Valid_moves == null)
            return;

        //Wyświetlenie wszystkich możliwych ruchów figury
        //
        foreach (var valid_move in lastActivePiece.Valid_moves.ToList())
        {
            validMoveHighlight = new Image();
            validMoveHighlight.Tag = "ValidMoveHighlight";
            validMoveHighlight.Source = validBitmapMove;

            Grid.SetRow(validMoveHighlight, (int)valid_move.Y);
            Grid.SetColumn(validMoveHighlight, (int)valid_move.X);
            Chessboard.Children.Add(validMoveHighlight);
        }
    }

    private void Chessboard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var pos = e.GetPosition(Chessboard);
        var clickedUIElement = Chessboard.InputHitTest(pos);

        var promo = turn == 0 ? whitePromotion : blackPromotion;

        var piece = clickedUIElement as Image;

        //Jeśli nie ma promocji pionka, to gra jest kontynuowana, czyli ruch jest zapisywany, tura jest zmieniana
        if (!promo)
        {
            if (clickedUIElement == null || clickedUIElement is Border)
            {
                lastActivePiece = null;
                UpdateChessboard();
                return;
            }

            if(piece == null)
            {
                lastActivePiece = null;
                UpdateChessboard();
                return;
            }

        
            if ((string)piece.Tag == "ValidMoveHighlight")
            {
                var target = (Piece)listOfPieces.FirstOrDefault(p => (p as Piece).Current_pos == new Point(Grid.GetColumn(piece), Grid.GetRow(piece)));
                var targetPos = new Point(Grid.GetColumn(piece), Grid.GetRow(piece));

                move = Moves.MoveNotation(lastActivePiece, targetPos);

                if (target != null)
                    listOfPieces.Remove(target);

                //Wykonanie ruchu
                lastActivePiece.MakeMove(new Point(Grid.GetColumn(piece), Grid.GetRow(piece)));
                var index = listOfPieces.IndexOf(lastActivePiece);
                lastActivePiece.SetPieceMoved(true);

                //Jeśli ruszyliśmy się pionem na ostatnią pozycję, aktywujemy promocję pionka na figurę
                if (lastActivePiece.Type == "Pawn" && (int)targetPos.Y == turn * 7)
                {
                    (whitePromotion, blackPromotion) = lastActivePiece.Color == "White" ? (true, false) : (false, true);
                    Grid.SetColumn(promotionPanel, (int)targetPos.X);
                }
                //Jeśli nie, gra jest kontynuowana
                else
                {
                    //Wykonanie roszady
                    if (lastActivePiece.Type == "King" && lastActivePiece.Current_pos.Y == (turn+1)%2 * 7 && (lastActivePiece.Current_pos.X == 2 || lastActivePiece.Current_pos.X == 6))
                    {
                        //sprawdza kierunek wykonywania roszady
                        var dir = Grid.GetColumn(piece);
                        int ind = 0;
                        Piece rook;

                        switch (dir)
                        {
                            //długa roszada
                            case 2:
                                rook = (Piece)listOfPieces.FirstOrDefault(p => (p as Piece).Current_pos == new Point(0, Grid.GetRow(piece)));
                                ind = listOfPieces.IndexOf(rook);
                                rook.MakeMove(new Point(3, lastActivePiece.Current_pos.Y));
                                listOfPieces[ind] = rook;
                                ScoreTextBlock.Text += turn == 0 ? $"{(ScoreTextBlock.Text.Length) / 10 + 1}.\t O-O-O" : $"\t\tO-O-O";
                                break;
                            //krótka roszada
                            case 6:
                                rook = (Piece)listOfPieces.FirstOrDefault(p => (p as Piece).Current_pos == new Point(7, Grid.GetRow(piece)));
                                ind = listOfPieces.IndexOf(rook);
                                rook.MakeMove(new Point(5, lastActivePiece.Current_pos.Y));
                                listOfPieces[ind] = rook;
                                ScoreTextBlock.Text += turn == 0 ? $"{(ScoreTextBlock.Text.Length) / 10 + 1}.\t O-O" : $"\t\tO-O";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    //Zapisywanie notacji ruchu
                        ScoreTextBlock.Text += turn == 0 ? $"{(ScoreTextBlock.Text.Length) / 10 + 1}.\t {move}" : $"\t\t{move}";

                    //Aktualizacja przesuniętej figury w tablicy
                    listOfPieces[index] = lastActivePiece;
                    //Usunięcie figury z pamięci
                    lastActivePiece = null;
                        
                    //Zmiana tury
                    turn = (turn + 1) % 2;

                    //Jeśli następna tura jest białych, na tablicy z notacją tworzy się nowa linia, gdzie będzie zapisywany kolejny ruch
                    ScoreTextBlock.Text += turn == 0 ? $"{Environment.NewLine}" : "";
                }
            }
            else
            {
                if ((turn == 0 ? piece.Tag.ToString().Contains("White") : piece.Tag.ToString().Contains("Black")))
                    lastActivePiece = listOfPieces.FirstOrDefault(p => (p as Piece)?.Tag == piece.Tag) as Piece;
            }
        }
        //Jeśli nie, to gracz musi wybrać jakąś figurę 
        else
        {
            if(piece == null || !piece.Name.Contains("Promo"))
            {
                return;
            }

            //Zastąpienie pionka wybraną figurą
            listOfPieces.Remove(lastActivePiece);
            var newPiece = new Piece($"{lastActivePiece.Color}{piece.Tag}{promoID++}", lastActivePiece.Current_pos);
            listOfPieces.Add(newPiece);
            lastActivePiece = null;

            ScoreTextBlock.Text += turn == 0 ? $"{(ScoreTextBlock.Text.Length) / 10 + 1}.\t {move}={Moves.PieceSymbol(newPiece)}" : $"\t\t{move}={Moves.PieceSymbol(newPiece)}";

            //Zmiana tury
            turn = (turn + 1) % 2;

            //Ukrycie panelu i odznaczenie promocji pionka (została już zrealizowana)
            promotionPanel.Visibility = Visibility.Hidden;
            (whitePromotion, blackPromotion) = (false, false);

            //Po zmianie tury, jeśli jest tura białego, na tablicy z notacją tworzy się nowa linia, gdzie będzie zapisywany kolejny ruch
            ScoreTextBlock.Text += turn == 0 ? $"{Environment.NewLine}" : "";
        }

        UpdateChessboard();
    }

    private void RotateGrid()
    {
        Chessboard.RenderTransform = new RotateTransform((turn == 0 ? 0 : 180), 256, 256);
        foreach (var child in Chessboard.Children)
        {
            if (child.GetType() == typeof(Image))
            {
                (child as Image).RenderTransform = new RotateTransform((turn == 0 ? 0 : -180), 32, 32);
            }
        }
    }

    private void ShowPromotionPanel(string color)
    {
        promotionPanel.RenderTransform = new RotateTransform(turn * 180, 0, 0);

        string[] imagePaths = new string[]
        {
            $"Assets/{color}Bishop.png",
            $"Assets/{color}Knight.png",
            $"Assets/{color}Rook.png",
            $"Assets/{color}Queen.png"
        };

        var i = 0;

        var promotionPieces = PromotionGrid.Children.Cast<Image>().ToList();

        if(promotionPieces != null)
        {
            foreach (var piece in promotionPieces)
            {
                if (piece != null)
                {
                    var bitImage = new BitmapImage();
                    var uri = new Uri(imagePaths[i++], UriKind.Relative);
                    bitImage.BeginInit();
                    bitImage.UriSource = uri;
                    bitImage.EndInit();

                    piece.Source = bitImage;
                }
            }
        }

        promotionPanel.Visibility = Visibility.Visible;
    }
}