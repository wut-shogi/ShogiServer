using static ShogiServer.EngineWrapper.EngineWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShogiServer.UnitTests.EngineWrapper
{
    internal class EngineWrapperTests
    {
        [Test]
        public void GetAllMoves_Test()
        {
            for (int i = 0; i < 1000; i++)
            {
                var initialPosition = "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1";
                var result = GetAllMoves(initialPosition);
                result.Length.Should().Be(30);
            }
        }
    }
}
