using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotVVM.Samples.Tests
{
    [TestClass]
    public class BasicSamplesRootDomainTests : BasicSamplesTests
    {
        protected override string BaseUrl
        {
            get { return "http://localhost:8628/"; }
        }


        [TestMethod]
        public void Sample1Test_FullDomain() { Sample1Test(); }
        [TestMethod]
        public void Sample2Test_FullDomain() { Sample2Test(); }
        [TestMethod]
        public void Sample3Test_FullDomain() { Sample3Test(); }
        [TestMethod]
        public void Sample4Test_FullDomain() { Sample4Test(); }
        [TestMethod]
        public void Sample5Test_FullDomain() { Sample5Test(); }
        [TestMethod]
        public void Sample6Test_FullDomain() { Sample6Test(); }
        [TestMethod]
        public void Sample8Test_FullDomain() { Sample8Test(); }
        [TestMethod]
        public void Sample9Test_FullDomain() { Sample9Test(); }
        [TestMethod]
        public void Sample10Test_FullDomain() { Sample10Test(); }
        [TestMethod]
        public void Sample11Test_FullDomain() { Sample11Test(); }
        [TestMethod]
        public void Sample12Test_FullDomain() { Sample12Test(); }
        [TestMethod]
        public void Sample13Test_FullDomain() { Sample13Test(); }
        [TestMethod]
        public void Sample14Test_FullDomain() { Sample14Test(); }
        [TestMethod]
        public void Sample15Test_FullDomain() { Sample15Test(); }
        [TestMethod]
        public void Sample16Test_FullDomain() { Sample16Test(); }
        [TestMethod]
        public void Sample17Test_FullDomain() { Sample17Test(); }
        [TestMethod]
        public void Sample18Test_FullDomain() { Sample18Test(); }
        [TestMethod]
        public void Sample20Test_FullDomain() { Sample20Test(); }
        [TestMethod]
        public void Sample22Test_FullDomain() { Sample22Test(); }
        [TestMethod]
        public void Sample24Test_FullDomain() { Sample24Test(); }

    }
}