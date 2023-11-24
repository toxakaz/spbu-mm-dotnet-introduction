namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new A(10);
            var pg = PathGetter.GetPath<A, int>("B[1].C[3].Content[6]");
            Console.WriteLine(pg(a));
        }
    }
}