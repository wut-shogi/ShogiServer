﻿using ShogiServer.EngineWrapper;

namespace ShogiServer.UnitTests.EngineWrapper
{
    internal class EngineWrapperTests
    {
        [Test]
        public void GetAllMoves_Test()
        {
            var result = Engine.GetAllMoves(Engine.InitialPosition());
            result.Length.Should().Be(30);
        }

        [Test]
        public void IsBlackTurn_Test()
        {
            var position = Engine.InitialPosition();
            var blackTurn = Engine.IsBlackTurn(position);
            blackTurn.Should().BeTrue();
        }
    }
}
