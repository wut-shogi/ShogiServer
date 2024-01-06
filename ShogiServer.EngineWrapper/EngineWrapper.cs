using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ShogiServer.EngineWrapper
{
    public static partial class Engine
    {
        [LibraryImport("shogi_engine")]
        private static partial void getAllLegalMoves(byte[] input, byte[] output);

        [LibraryImport("shogi_engine")]
        private static partial void getBestMove(byte[] input, uint maxDepth, uint maxTime, byte[] output);

        public static string[] GetAllMoves(string SFENstring)
        {
            var input = Encoding.ASCII.GetBytes(SFENstring).Append((byte) 0).ToArray();
            var outputBuffer = new byte[4096];
            getAllLegalMoves(input, outputBuffer);
            var output = Encoding.ASCII.GetString(outputBuffer.TakeWhile(c => c != '\0').ToArray());
            return output.ToString().Split('|');
        }

        public static string GetBestMove(string SFENstring)
        {
            var input = Encoding.ASCII.GetBytes(SFENstring);
            var outputBuffer = new byte[4096];
            getBestMove(input, 1, 1, outputBuffer);
            var output = Encoding.ASCII.GetString(outputBuffer.TakeWhile(c => c != '\0').ToArray());
            return output;
        }

        public static bool IsMoveValid(string SFENstring, string move)
        {
            var moves = GetAllMoves(SFENstring);
            return moves.Contains(move);
        }

        public static string InitialPosition()
        {
            return "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1";
        }

        public static bool IsBlackTurn(string SFENstring)
        {
            var regex = new Regex("[bw] - [0-9]+$");
            var tail = regex.Match(SFENstring).Value;
            return tail.Length > 0 && tail[0] == 'b';
        }

        public static string ApplyMove(string SFENstring, string move)
        {
            // TODO
            return move;
        }
    }
}