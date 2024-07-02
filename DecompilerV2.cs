using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LuaCipher
{
    public class LuaUDecompiler
    {
        private enum Opcode
        {
            MOVE = 0,
            LOADK = 1,
            LOADBOOL = 2,
            LOADNIL = 3,
            GETUPVAL = 4,
            GETGLOBAL = 5,
            GETTABLE = 6,
            SETGLOBAL = 7,
            SETUPVAL = 8,
            SETTABLE = 9,
            NEWTABLE = 10,
            SELF = 11,
            ADD = 12,
            SUB = 13,
            MUL = 14,
            DIV = 15,
            MOD = 16,
            POW = 17,
            UNM = 18,
            NOT = 19,
            LEN = 20,
            CONCAT = 21,
            JMP = 22,
            EQ = 23,
            LT = 24,
            LE = 25,
            TEST = 26,
            TESTSET = 27,
            CALL = 28,
            TAILCALL = 29,
            RETURN = 30,
            FORLOOP = 31,
            FORPREP = 32,
            TFORCALL = 33,
            TFORLOOP = 34,
            SETLIST = 35,
            CLOSURE = 36,
            VARARG = 37,
            EXTRAARG = 38,
            LOADKX = 39,
            GETTABUP = 40,
            SETTABUP = 41,
            TAILCALLI = 42,
            GETUPVALI = 43,
            GETGLOBALI = 44,
            GETTABLEI = 45,
            SETUPVALI = 46,
            SETGLOBALI = 47,
            SETTABLEI = 48,
            NEWTABLEI = 49,
            SELF2 = 50,
            ADD2 = 51,
            SUB2 = 52,
            MUL2 = 53,
            DIV2 = 54,
            MOD2 = 55,
            POW2 = 56,
            UNM2 = 57,
            NOT2 = 58,
            LEN2 = 59,
            CONCAT2 = 60,
            JMP2 = 61,
            EQ2 = 62,
            LT2 = 63,
            LE2 = 64,
            TEST2 = 65,
            TESTSET2 = 66,
            CALL2 = 67,
            RETURN2 = 68,
            FORLOOP2 = 69,
            FORPREP2 = 70,
            TFORCALL2 = 71,
            TFORLOOP2 = 72,
            SETLIST2 = 73,
            CLOSE2 = 74,
            CLOSURE2 = 75,
            VARARG2 = 76
        }

        public LuaUDecompilerSettings Settings { get; set; }

        public LuaUDecompiler(LuaUDecompilerSettings settings = null)
        {
            Settings = settings ?? new LuaUDecompilerSettings();
        }

        public string DecompileScript(string bytecode)
        {
            try
            {
                byte[] bytecodeBytes = Convert.FromBase64String(bytecode);
                string decompiledScript = BytecodeToLua(bytecodeBytes);

                if (Settings.EnableOptimizations)
                {
                    decompiledScript = OptimizeScript(decompiledScript);
                }

                return decompiledScript;
            }
            catch (FormatException ex)
            {
                throw new LuaUDecompilerException("Failed to decode Base64 bytecode", ex);
            }
            catch (Exception ex)
            {
                throw new LuaUDecompilerException("Decompilation failed", ex);
            }
        }

        private string BytecodeToLua(byte[] bytecode)
        {
            StringBuilder luaSource = new StringBuilder();
            luaSource.Append("-- Decompiled LuaU script\n");

            int index = 0;
            while (index < bytecode.Length)
            {
                if (index + 3 >= bytecode.Length)
                    break;

                uint instruction = BitConverter.ToUInt32(bytecode, index);
                luaSource.Append(ParseInstruction(instruction));
                index += 4;
            }

            return luaSource.ToString();
        }

        private string ParseInstruction(uint instruction)
        {
            int opcode = (int)(instruction & 0x3F);
            int a = (int)((instruction >> 6) & 0xFF);
            int b = (int)((instruction >> 23) & 0x1FF);
            int c = (int)((instruction >> 14) & 0x1FF);
            int bx = (int)((instruction >> 14) & 0x3FFFF);
            int sbx = bx - 131071;

            switch ((Opcode)opcode)
{
    case Opcode.MOVE:
        return FormatInstruction($"R[{a}] = R[{b}]");
    case Opcode.LOADK:
        return FormatInstruction($"R[{a}] = K[{bx}]");
    case Opcode.LOADBOOL:
        return FormatInstruction($"R[{a}] = {(b != 0 ? "true" : "false")}; if ({c} != 0) then pc++ end");
    case Opcode.LOADNIL:
        return FormatInstruction($"for i = {a}, {a + b} do R[i] = nil end");
    case Opcode.GETUPVAL:
        return FormatInstruction($"R[{a}] = UpValue[{b}]");
    case Opcode.GETGLOBAL:
        return FormatInstruction($"R[{a}] = _G[{bx}]");
    case Opcode.GETTABLE:
        return FormatInstruction($"R[{a}] = R[{b}][RK[{c}]]");
    case Opcode.SETGLOBAL:
        return FormatInstruction($"_G[{bx}] = R[{a}]");
    case Opcode.SETUPVAL:
        return FormatInstruction($"UpValue[{b}] = R[{a}]");
    case Opcode.SETTABLE:
        return FormatInstruction($"R[{a}] = R[{b}][RK[{c}]]");
    case Opcode.NEWTABLE:
        return FormatInstruction($"R[{a}] = {{}} -- size = {b},{c}");
    case Opcode.SELF:
        return FormatInstruction($"R[{a + 1}] = R[{b}]; R[{a}] = R[{b}][RK[{c}]]");
    case Opcode.ADD:
        return FormatInstruction($"R[{a}] = RK[{b}] + RK[{c}]");
    case Opcode.SUB:
        return FormatInstruction($"R[{a}] = RK[{b}] - RK[{c}]");
    case Opcode.MUL:
        return FormatInstruction($"R[{a}] = RK[{b}] * RK[{c}]");
    case Opcode.DIV:
        return FormatInstruction($"R[{a}] = RK[{b}] / RK[{c}]");
    case Opcode.MOD:
        return FormatInstruction($"R[{a}] = RK[{b}] % RK[{c}]");
    case Opcode.POW:
        return FormatInstruction($"R[{a}] = RK[{b}] ^ RK[{c}]");
    case Opcode.UNM:
        return FormatInstruction($"R[{a}] = -R[{b}]");
    case Opcode.NOT:
        return FormatInstruction($"R[{a}] = not R[{b}]");
    case Opcode.LEN:
        return FormatInstruction($"R[{a}] = #R[{b}]");
    case Opcode.CONCAT:
        return FormatInstruction($"R[{a}] = R[{b}] .. R[{c}]");
    case Opcode.JMP:
        return FormatInstruction($"pc += {sbx}");
    case Opcode.EQ:
        return FormatInstruction($"if R[{b}] == R[{c}] then pc++ end");
    case Opcode.LT:
        return FormatInstruction($"if R[{b}] < R[{c}] then pc++ end");
    case Opcode.LE:
        return FormatInstruction($"if R[{b}] <= R[{c}] then pc++ end");
    case Opcode.TEST:
        return FormatInstruction($"if not R[{a}] then pc++ end");
    case Opcode.TESTSET:
        return FormatInstruction($"if R[{b}] then R[{a}] = R[{b}] else pc++ end");
    case Opcode.CALL:
        return FormatInstruction($"R[{a}] = R[{a}]({GetArguments(a + 1, b - 1)}); R[{a + b - 1}] = {GetArguments(a, c - 1)}");
    case Opcode.TAILCALL:
        return FormatInstruction($"return R[{a}]({GetArguments(a + 1, b - 1)})");
    case Opcode.RETURN:
        return FormatInstruction($"return {GetArguments(a, b - 1)}");
    case Opcode.FORLOOP:
        return FormatInstruction($"R[{a}] += R[{a + 2}]; if R[{a}] <= R[{a + 1}] then pc += {sbx}; R[{a + 3}] = R[{a}]");
    case Opcode.FORPREP:
        return FormatInstruction($"R[{a}] -= R[{a + 2}]; pc += {sbx}");
    case Opcode.TFORCALL:
        return FormatInstruction($"R[{a + 3}] = R[{a}]({GetArguments(a + 1, c)})");
    case Opcode.TFORLOOP:
        return FormatInstruction($"if R[{a + 1}] then R[{a + 3}] = R[{a + 1}]; pc += {sbx} end");
    case Opcode.SETLIST:
        return FormatInstruction($"for i = 1, {b} do R[{a}][i + {(c - 1) * 50}] = R[{a + i}] end");
    case Opcode.CLOSURE:
        return FormatInstruction($"R[{a}] = function(K[{bx}]) end");
    case Opcode.VARARG:
        return FormatInstruction($"R[{a}] = ...");
    case Opcode.EXTRAARG:
        return FormatInstruction($"-- EXTRAARG with value {bx}");
    case Opcode.LOADKX:
        return FormatInstruction($"R[{a}] = K[EXTRAARG]");
    case Opcode.GETTABUP:
        return FormatInstruction($"R[{a}] = UpValue[{b}][RK[{c}]]");
    case Opcode.SETTABUP:
        return FormatInstruction($"UpValue[{a}][RK[{b}]] = RK[{c}]");
    case Opcode.TAILCALLI:
        return FormatInstruction($"return R[{a}]({GetArguments(a + 1, b - 1)})");
    case Opcode.GETUPVALI:
        return FormatInstruction($"R[{a}] = UpValue[{b}]");
    case Opcode.GETGLOBALI:
        return FormatInstruction($"R[{a}] = _G[{bx}]");
    case Opcode.GETTABLEI:
        return FormatInstruction($"R[{a}] = R[{b}][RK[{c}]]");
    case Opcode.SETUPVALI:
        return FormatInstruction($"UpValue[{b}] = R[{a}]");
    case Opcode.SETGLOBALI:
        return FormatInstruction($"_G[{bx}] = R[{a}]");
    case Opcode.SETTABLEI:
        return FormatInstruction($"R[{a}] = R[{b}][RK[{c}]]");
    case Opcode.NEWTABLEI:
        return FormatInstruction($"R[{a}] = {{}} -- size = {b},{c}");
    case Opcode.SELF2:
        return FormatInstruction($"R[{a + 1}] = R[{b}]; R[{a}] = R[{b}][RK[{c}]]");
    case Opcode.ADD2:
        return FormatInstruction($"R[{a}] = RK[{b}] + RK[{c}]");
    case Opcode.SUB2:
        return FormatInstruction($"R[{a}] = RK[{b}] - RK[{c}]");
    case Opcode.MUL2:
        return FormatInstruction($"R[{a}] = RK[{b}] * RK[{c}]");
    case Opcode.DIV2:
        return FormatInstruction($"R[{a}] = RK[{b}] / RK[{c}]");
    case Opcode.MOD2:
        return FormatInstruction($"R[{a}] = RK[{b}] % RK[{c}]");
    case Opcode.POW2:
        return FormatInstruction($"R[{a}] = RK[{b}] ^ RK[{c}]");
    case Opcode.UNM2:
        return FormatInstruction($"R[{a}] = -R[{b}]");
    case Opcode.NOT2:
        return FormatInstruction($"R[{a}] = not R[{b}]");
    case Opcode.LEN2:
        return FormatInstruction($"R[{a}] = #R[{b}]");
    case Opcode.CONCAT2:
        return FormatInstruction($"R[{a}] = R[{b}] .. R[{c}]");
    case Opcode.JMP2:
        return FormatInstruction($"pc += {sbx}");
    case Opcode.EQ2:
        return FormatInstruction($"if R[{b}] == R[{c}] then pc++ end");
    case Opcode.LT2:
        return FormatInstruction($"if R[{b}] < R[{c}] then pc++ end");
    case Opcode.LE2:
        return FormatInstruction($"if R[{b}] <= R[{c}] then pc++ end");
    case Opcode.TEST2:
        return FormatInstruction($"if not R[{a}] then pc++ end");
    case Opcode.TESTSET2:
        return FormatInstruction($"if R[{b}] then R[{a}] = R[{b}] else pc++ end");
    case Opcode.CALL2:
        return FormatInstruction($"R[{a}] = R[{a}]({GetArguments(a + 1, b - 1)}); R[{a + b - 1}] = {GetArguments(a, c - 1)}");
    case Opcode.RETURN2:
        return FormatInstruction($"return {GetArguments(a, b - 1)}");
    case Opcode.FORLOOP2:
        return FormatInstruction($"R[{a}] += R[{a + 2}]; if R[{a}] <= R[{a + 1}] then pc += {sbx}; R[{a + 3}] = R[{a}]");
    case Opcode.FORPREP2:
        return FormatInstruction($"R[{a}] -= R[{a + 2}]; pc += {sbx}");
    case Opcode.TFORCALL2:
        return FormatInstruction($"R[{a + 3}] = R[{a}]({GetArguments(a + 1, c)})");
    case Opcode.TFORLOOP2:
        return FormatInstruction($"if R[{a + 1}] then R[{a + 3}] = R[{a + 1}]; pc += {sbx} end");
    case Opcode.SETLIST2:
        return FormatInstruction($"for i = 1, {b} do R[{a}][i + {(c - 1) * 50}] = R[{a + i}] end");
    case Opcode.CLOSE2:
        return FormatInstruction($"-- CLOSE2 operation");
    case Opcode.CLOSURE2:
        return FormatInstruction($"R[{a}] = function(K[{bx}]) end");
    case Opcode.VARARG2:
        return FormatInstruction($"R[{a}] = ...");

                default:
                    return $"-- Unhandled opcode {opcode}\n";
            }
        }

        private string FormatInstruction(string keyword, string args)
        {
            return $"{keyword} {args}\n";
        }

        private string GetArguments(int start, int count)
        {
            if (count <= 0)
                return "";

            List<string> args = new List<string>();
            for (int i = 0; i < count; i++)
            {
                args.Add($"R[{start + i}]");
            }

            return string.Join(", ", args);
        }

        private string OptimizeScript(string script)
        {
            script = Regex.Replace(script, @"\bpc\s*\+=\s*1\b", "-- pc++ (removed)");
            script = Regex.Replace(script, @"\bfor\s+i\s*=\s*(\d+),\s*(\d+)\s*do\s*R\[i\]\s*=\s*nil\s*end\b", "R[$1] = nil -- optimized nil assignment");

            return script;
        }
    }

    public class LuaUDecompilerSettings
    {
        public bool EnableOptimizations { get; set; } = true;
    }

    public class LuaUDecompilerException : Exception
    {
        public LuaUDecompilerException(string message) : base(message) { }
        public LuaUDecompilerException(string message, Exception innerException) : base(message, innerException) { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string bytecode = args.Length > 0 ? args[0] : "";
                LuaUDecompiler decompiler = new LuaUDecompiler();

                string decompiledScript = decompiler.DecompileScript(bytecode);
                Console.WriteLine(decompiledScript);
            }
            catch (LuaUDecompilerException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
