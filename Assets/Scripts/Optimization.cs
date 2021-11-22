public static class Optimization{
    public static Tile[] jagg2dArray(Tile[,] board, int cols, int rows)
    {

        Tile[] newBoard = new Tile[cols * rows];

        int x = 0;
        for (int i = 0; i < cols; i++)
        {
            for (int k = 0; k < rows; k++)
            {
                newBoard[x] = board[i, k];

                x += 1;
            }
        }

        return newBoard;
    }
}