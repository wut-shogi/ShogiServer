using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ShogiServer.EngineWrapper
{
    public static partial class Engine
    {
        [DllImport("shogi_engine", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool init();

        [DllImport("shogi_engine", CallingConvention = CallingConvention.Cdecl)]
        private static extern void cleanup();

        [DllImport("shogi_engine", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getAllLegalMoves(string SFENstring, byte[] output);

        [DllImport("shogi_engine", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getBestMove(string SFENstring, uint maxDepth, uint maxTime, bool useGPU, byte[] output);

        [DllImport("shogi_engine", CallingConvention = CallingConvention.Cdecl)]
        private static extern int makeMove(string SFENstring, string moveString, byte[] output);


        public static bool Init()
        {
            return init();
        }

        public static void CleanUp()
        {
            cleanup();
        }

        public static string[] GetAllMoves(string SFENstring)
        {
            Init();
            var outputBuffer = new byte[4096];
            int size = getAllLegalMoves(SFENstring, outputBuffer);
            string movesString = Encoding.UTF8.GetString(outputBuffer, 0, size);
            CleanUp();
            return movesString.Split('|');
        }

        public static string GetBestMove(string SFENstring, uint maxDepth, uint maxTime, bool useGPU)
        {
            Init();
            var outputBuffer = new byte[4096];
            int size = getBestMove(SFENstring, maxDepth, maxTime, useGPU, outputBuffer);
            CleanUp();
            return Encoding.UTF8.GetString(outputBuffer, 0, size);
        }

        public static string MakeMove(string SFENString, string moveString)
        {
            Init();
            var outputBuffer = new byte[4096];
            int size = makeMove(SFENString, moveString, outputBuffer);
            CleanUp();
            return Encoding.UTF8.GetString(outputBuffer, 0, size);
        }

        public static bool IsMoveValid(string SFENstring, string move)
        {
            Init();
            var moves = GetAllMoves(SFENstring);
            CleanUp();
            return moves.Contains(move);
        }

        public static string InitialPosition()
        {
            return "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1";
        }

        public static bool IsBlackTurn(string SFENstring)
        {
            var regex = new Regex("[bw] [-lLnNgsSgGkKrRbBpP+]+ [0-9]+$");
            var tail = regex.Match(SFENstring).Value;
            return tail.Length > 0 && tail[0] == 'b';
        }
    }
}