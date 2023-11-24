namespace Task1
{
    public class A
    {
        public B[] B { get; }
        public C[] C { get; }
        public int Size { get; }

        public A(int size)
        {
            Size = size;
            B = new B[Size];
            C = new C[Size];
            for (int i = 0; i < Size; i++)
            {
                B[i] = new B(Size);
                C[i] = new C(Size);
            }
        }
    }
}