namespace Task1Tests
{
    public class Tests
    {
        [Test]
        public void SimpleGetPathToIntTest()
        {
            var gp = Task1.PathGetter.GetPath<Task1.A, int>("B[5].C[6].Content[8]");
            Task1.A a = new(10);
            Assert.That(gp(a), Is.EqualTo(8));
        }
        [Test]
        public void SimpleGetPathToCharTest()
        {
            var gp = Task1.PathGetter.GetPath<Task1.A, int>("C[6].Hello[1]");
            Task1.A a = new(10);
            Assert.That(gp(a), Is.EqualTo('E'));
        }
        [Test]
        public void SimpleGetPathToClassTest()
        {
            var gp = Task1.PathGetter.GetPath<Task1.A, Task1.C>("B[8].C[6]");
            Task1.A a = new(10);
            Assert.That(gp(a), Is.EqualTo(a.B[8].C[6]));
        }
        [Test]
        public void SimpleGetPathToIntMatrixTest()
        {
            var gp = Task1.PathGetter.GetPath<Task1.A, int>("C[0].Matrix[3, 7]");
            Task1.A a = new(10);
            Assert.That(gp(a), Is.EqualTo(37));
        }
    }
}