// This code is a simple implementation of Tic Tac Toe game using a binary tree to store the player's moves.
// The game is played by two players, and the game ends when one of the players has three equidistant moves in the tree.
// The tree is balanced after each insertion to ensure that the equidistant check is efficient.
// The game is played on a 3x3 board, and the players take turns to make a move by selecting a position on the board.
// The game checks for a win after each move and ends when one of the players wins or the board is full.
// logarithmic complexity in most of the operations is O(log n) where n is the board size.
// This code works really well for 3x3 board.
// For larger boards, the tree balancing logic will not work. It needs may be a b-tree implementation or an entirely different implementation. Something like a modified Minimax algorithm with heuristic pruning.
// 1. Instead of checking for equidistant children after every insertion, use heuristic evaluations to prune unnecessary checks.
// 2. Explore using more efficient data structures tailored for the specific checks required by the game rules.
// 3. If possible, parallelize parts of the computation, such as position checks and tree balancing.

var boardSize = 3;
var playerCount = 2;
Game b = new(boardSize, playerCount);
b.Play();

class Game
{
    int _boardSize;
    List<Player> _players = [];
    Dictionary<int, int> _positions = [];

    internal Game(int boardSize, int playerCount)
    {
        _boardSize = boardSize;
        for (int i = 1; i <= boardSize * boardSize; i++)
        {
            _positions.Add(i, 0); // 0 - not taken, 1 - taken
        }

        for (int i = 0; i < playerCount; i++)
        {
            Console.WriteLine($"Enter player {i + 1} name: ");
            var name = Console.ReadLine();
            _players.Add(new Player { Name = name });
        }
    }

    internal void Play()
    {
        bool hasAnyWin = false, positionsFilled = false;
        while (hasAnyWin == false && positionsFilled == false)
        {
            foreach (var p in _players)
            {
                int newPosition = 0;
                while (newPosition == 0)
                {
                    Console.WriteLine($"{p.Name}, select a position: ");
                    newPosition = int.Parse(Console.ReadLine());

                    if (_positions.ContainsKey(newPosition) && _positions[newPosition] != 0)
                    {
                        Console.WriteLine($"Position {newPosition} is not available. Select a different position.");
                        newPosition = 0;
                    }
                    else
                    {
                        _positions[newPosition] = 1;
                        p.Insert(newPosition);
                    }
                }

                if (p.HasWon())
                {
                    Console.WriteLine($"{p.Name} has won");
                    hasAnyWin = true;
                    break;
                }

                if (_positions.All(x => x.Value == 1))
                {
                    positionsFilled = true;
                    break;
                }
            }
        }

        if (positionsFilled & !hasAnyWin)
        {
            Console.WriteLine("Game over. No one won.");
        }
    }
}

class Player
{
    internal string Name { get; set; }
    PlayerPath Path { get; set; }
    internal Player()
    {
        Path = new PlayerPath();
    }

    internal void Insert(int positionValue)
    {
        Path.Insert(positionValue);
    }

    internal bool HasWon()
    {
        return Path.HasWon();
    }
}

class Node
{
    internal int Value { get; set; }
    internal Node Left { get; set; }
    internal Node Right { get; set; }

    internal Node(int value)
    {
        Value = value;
        Left = null;
        Right = null;
    }

    internal bool HasEquidistantChildren
    {
        get
        {
            if (Left == null || Right == null)
                return false;

            return Value - Left?.Value == Right?.Value - Value;
        }
    }
}

class PlayerPath
{
    internal Node _root;

    internal PlayerPath()
    {
        _root = null;
    }

    internal void Insert(int value)
    {
        _root = InsertRecursively(_root, value);
        BalanceTree();
    }

    private Node InsertRecursively(Node root, int value)
    {
        if (root == null)
        {
            root = new Node(value);
            return root;
        }

        if (value < root.Value)
        {
            root.Left = InsertRecursively(root.Left, value);
        }
        else if (value > root.Value)
        {
            root.Right = InsertRecursively(root.Right, value);
        }

        return root;
    }

    internal void BalanceTree()
    {
        _root = Balance(_root);
    }

    private Node Balance(Node root)
    {
        if (root == null) return root;

        root.Left = Balance(root.Left);
        root.Right = Balance(root.Right);

        if (root.Left != null && root.Right == null)
        {
            root = RightRotate(root);
        }
        else if (root.Right != null && root.Left == null)
        {
            root = LeftRotate(root);
        }
        else if (root.Left != null && root.Right != null)
        {
            if (root.Left.Value > root.Value)
            {
                root = RightRotate(root);
            }
            if (root.Right.Value < root.Value)
            {
                root = LeftRotate(root);
            }
        }

        return root;
    }

    private Node LeftRotate(Node node)
    {
        Node newRoot = node.Right;
        node.Right = newRoot.Left;
        newRoot.Left = node;
        return newRoot;
    }

    private Node RightRotate(Node node)
    {
        Node newRoot = node.Left;
        node.Left = newRoot.Right;
        newRoot.Right = node;
        return newRoot;
    }

    internal bool HasWon()
    {
        return CheckEquidistantChildren(_root);
    }

    private bool CheckEquidistantChildren(Node node)
    {
        if (node == null)
        {
            return false;
        }

        if (node.HasEquidistantChildren)
        {
            return true;
        }

        return CheckEquidistantChildren(node.Left) || CheckEquidistantChildren(node.Right);
    }

    //internal void PrintTree()
    //{
    //    PrintTreeRec(_root, "", true);
    //}

    //private void PrintTreeRec(Node root, string indent, bool last)
    //{
    //    if (root != null)
    //    {
    //        Console.WriteLine(indent + "+- " + root.Value);
    //        indent += last ? "   " : "|  ";

    //        PrintTreeRec(root.Left, indent, false);
    //        PrintTreeRec(root.Right, indent, true);
    //    }
    //}
}
