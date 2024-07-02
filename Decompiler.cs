using System;
using System.Collections.Generic;
using System.Text;

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

        public string DecompileScript(string bytecode)
        {
            try
            {
                byte[] bytecodeBytes = Convert.FromBase64String(bytecode);

                string decompiledScript = BytecodeToLua(bytecodeBytes);

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
            int opcode = (int)(instruction & 0x3F);        // 6 bits
            int a = (int)((instruction >> 6) & 0xFF);      // 8 bits
            int b = (int)((instruction >> 23) & 0x1FF);    // 9 bits
            int c = (int)((instruction >> 14) & 0x1FF);    // 9 bits
            int bx = (int)((instruction >> 14) & 0x3FFFF); // 18 bits
            int sbx = bx - 131071; // signed Bx

            switch ((Opcode)opcode)
            {
                case Opcode.MOVE:
                    return FormatInstruction("MOVE", $"R[{a}] = R[{b}]");
                case Opcode.LOADK:
                    return FormatInstruction("LOADK", $"R[{a}] = K[{bx}]");
                case Opcode.LOADBOOL:
                    return FormatInstruction("LOADBOOL", $"R[{a}] = {(b != 0 ? "true" : "false")}; if ({c} != 0) then pc++ end");
                case Opcode.LOADNIL:
                    return FormatInstruction("LOADNIL", $"for i = {a}, {a + b} do R[i] = nil end");
                case Opcode.GETUPVAL:
                    return FormatInstruction("GETUPVAL", $"R[{a}] = UpValue[{b}]");
                case Opcode.GETGLOBAL:
                    return FormatInstruction("GETGLOBAL", $"R[{a}] = G[{bx}]");
                case Opcode.GETTABLE:
                    return FormatInstruction("GETTABLE", $"R[{a}] = R[{b}][RK[{c}]]");
                case Opcode.SETGLOBAL:
                    return FormatInstruction("SETGLOBAL", $"G[{bx}] = R[{a}]");
                case Opcode.SETUPVAL:
                    return FormatInstruction("SETUPVAL", $"UpValue[{b}] = R[{a}]");
                case Opcode.SETTABLE:
                    return FormatInstruction("SETTABLE", $"R[{a}][RK[{b}]] = RK[{c}]");
                case Opcode.NEWTABLE:
                    return FormatInstruction("NEWTABLE", $"R[{a}] = {{}} -- size = {b},{c}");
                case Opcode.SELF:
                    return FormatInstruction("SELF", $"R[{a + 1}] = R[{b}]; R[{a}] = R[{b}][RK[{c}]]");
                case Opcode.ADD:
                    return FormatInstruction("ADD", $"R[{a}] = RK[{b}] + RK[{c}]");
                case Opcode.SUB:
                    return FormatInstruction("SUB", $"R[{a}] = RK[{b}] - RK[{c}]");
                case Opcode.MUL:
                    return FormatInstruction("MUL", $"R[{a}] = RK[{b}] * RK[{c}]");
                case Opcode.DIV:
                    return FormatInstruction("DIV", $"R[{a}] = RK[{b}] / RK[{c}]");
                case Opcode.MOD:
                    return FormatInstruction("MOD", $"R[{a}] = RK[{b}] % RK[{c}]");
                case Opcode.POW:
                    return FormatInstruction("POW", $"R[{a}] = RK[{b}] ^ RK[{c}]");
                case Opcode.UNM:
                    return FormatInstruction("UNM", $"R[{a}] = -R[{b}]");
                case Opcode.NOT:
                    return FormatInstruction("NOT", $"R[{a}] = not R[{b}]");
                case Opcode.LEN:
                    return FormatInstruction("LEN", $"R[{a}] = #R[{b}]");
                case Opcode.CONCAT:
                    return FormatInstruction("CONCAT", $"R[{a}] = R[{b}] .. R[{c}]");
                case Opcode.JMP:
                    return FormatInstruction("JMP", $"pc += {sbx}");
                case Opcode.EQ:
                    return FormatInstruction("EQ", $"if R[{b}] == R[{c}] then pc++ end");
                case Opcode.LT:
                    return FormatInstruction("LT", $"if R[{b}] < R[{c}] then pc++ end");
                case Opcode.LE:
                    return FormatInstruction("LE", $"if R[{b}] <= R[{c}] then pc++ end");
                case Opcode.TEST:
                    return FormatInstruction("TEST", $"if not R[{a}] then pc++ end");
                case Opcode.TESTSET:
                    return FormatInstruction("TESTSET", $"if R[{b}] then R[{a}] = R[{b}] else pc++ end");
                case Opcode.CALL:
                    return FormatInstruction("CALL", $"R[{a}], ... ,R[{a + c - 2}] = R[{a}]({GetArguments(a + 1, b - 1)})");
                case Opcode.TAILCALL:
                    return FormatInstruction("TAILCALL", $"return R[{a}]({GetArguments(a + 1, b - 1)})");
                case Opcode.RETURN:
                    return FormatInstruction("RETURN", $"return {GetArguments(a, b - 1)}");
                case Opcode.FORLOOP:
                    return FormatInstruction("FORLOOP", $"R[{a}] += 1; if R[{a}] <= R[{a + 2}] then pc += {sbx}; R[{a + 3}] = R[{a}] end");
                case Opcode.FORPREP:
                    return FormatInstruction("FORPREP", $"R[{a}] -= R[{a + 2}]; pc += {sbx}");
                case Opcode.TFORCALL:
                    return FormatInstruction("TFORCALL", $"R[{a + 3}], ... ,R[{a + 2 + c}] = R[{a}](R[{a + 1}], R[{a + 2}])");
                case Opcode.TFORLOOP:
                    return FormatInstruction("TFORLOOP", $"if R[{a + 1}] ~= nil then R[{a}] = R[{a + 1}]; pc += {sbx} end");
                case Opcode.SETLIST:
                    return FormatInstruction("SETLIST", $"for i = 1, {b} do R[{a}][(c - 1) * 50 + i] = R[{a + i}] end");
                case Opcode.CLOSE:
                    return FormatInstruction("CLOSE", $"-- Close all variables in the stack above R[{a}]");
                case Opcode.CLOSURE:
                    return FormatInstruction("CLOSURE", $"R[{a}] = closure({bx})");
                case Opcode.VARARG:
                    return FormatInstruction("VARARG", $"R[{a}], ... ,R[{a + b - 1}] = ...");
                case Opcode.EXTRAARG:
                    return FormatInstruction("EXTRAARG", $"-- Extra argument {instruction >> 6}");
                default:
                    return FormatInstruction("UNKNOWN", $"Unknown opcode {opcode}");
            }
        }

        private string FormatInstruction(string opcode, string arguments)
        {
            return $"{opcode} {arguments}\n";
        }

        private string GetArguments(int start, int count)
        {
            List<string> args = new List<string>();
            for (int i = 0; i < count; i++)
            {
                args.Add($"R[{start + i}]");
            }
            return string.Join(", ", args);
        }
    }

    public class LuaUDecompilerException : Exception
    {
        public LuaUDecompilerException(string message) : base(message) { }

        public LuaUDecompilerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
