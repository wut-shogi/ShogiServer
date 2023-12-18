using System.Runtime.InteropServices;
using System.Text;

namespace ShogiServer.EngineWrapper
{
    public static partial class EngineWrapper
    {
        [LibraryImport("shogi_engine.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        private static partial void getAllLegalMoves(byte[] input, byte[] output);

        [LibraryImport("shogi_engine.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
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
    }
}