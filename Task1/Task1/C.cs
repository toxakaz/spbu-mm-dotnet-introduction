namespace Task1
{
    public class C
    {
        public int Size { get; }
        public int[] Content { get; }
        public char[] Hello { get; }
        public int[,] Matrix { get; }

        public C(int size)
        {
            Size = size;
            Content = new int[Size];
            Hello = [.. "HELLO"];
            Matrix = new int[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                Content[i] = i;
            }
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Matrix[i, j] = i * Size + j;
                }
            }
        }
    }
}