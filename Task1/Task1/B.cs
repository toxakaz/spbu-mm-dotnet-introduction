namespace Task1
{
    public class B
    {
        public C[] C { get; }
        public int Size { get; }

        public B(int size)
        {
            Size = size;
            C = new C[Size];
            for (int i = 0; i < Size; i++)
            {
                C[i] = new C(Size);
            }
        }
    }
}