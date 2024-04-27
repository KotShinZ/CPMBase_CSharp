using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SkiaSharp;
using Xunit.Abstractions;
using System.IO;

namespace CPMBase.Tests
{
    public class DrawAxis
    {
        private readonly ITestOutputHelper _output;

        public DrawAxis(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Draw()
        {
            RangePosition rangePosition = new RangePosition(100, 0, 100, 0, 100, 0);
            var bitmap = new SKBitmap(256, 256);
            var canvas = new SKCanvas(bitmap);
            rangePosition.DrawAxis(canvas, 256, 256);
            var path = new PathObject("/workspaces/CPMBase_CSharp/CPMBase.Tests", "image", extention: ".png");
            path.WriteToImgFile(bitmap);

            _output.WriteLine(path.fullPath);
            Assert.True(File.Exists(path.fullPath));
        }
    }
}