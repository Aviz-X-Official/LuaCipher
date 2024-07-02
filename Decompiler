using System;
using System.Text;

namespace RobloxExecutor
{
    class Program
    {
        private const int MOVE = 0;      // R[A] := R[B]
        private const int LOADK = 1;     // R[A] := Kst[Bx]
        private const int LOADBOOL = 2;  // R[A] := (bool)B; if (C) pc++
        private const int LOADNIL = 3;   // R[A], R[A+1], ..., R[A+B] := nil
        private const int GETUPVAL = 4;  // R[A] := UpValue[B]
        private const int GETGLOBAL = 5; // R[A] := Gbl[Kst[Bx]]
        private const int GETTABLE = 6;  // R[A] := R[B][RK[C]]
        private const int SETGLOBAL = 7; // Gbl[Kst[Bx]] := R[A]
        private const int SETUPVAL = 8;  // UpValue[B] := R[A]
        private const int SETTABLE = 9;  // R[A][RK[B]] := RK[C]
        private const int NEWTABLE = 10; // R[A] := {} (size = B,C)
        private const int SELF = 11;     // R[A+1] := R[B]; R[A] := R[B][RK[C]]
        private const int ADD = 12;      // R[A] := RK[B] + RK[C]
        private const int SUB = 13;      // R[A] := RK[B] - RK[C]
        private const int MUL = 14;      // R[A] := RK[B] * RK[C]
        private const int DIV = 15;      // R[A] := RK[B] / RK[C]
        private const int MOD = 16;      // R[A] := RK[B] % RK[C]
        private const int POW = 17;      // R[A] := RK[B] ^ RK[C]
        private const int UNM = 18;      // R[A] := -R[B]
        private const int NOT = 19;      // R[A] := not R[B]
        private const int LEN = 20;      // R[A] := length of R[B]
        private const int CONCAT = 21;   // R[A] := R[B] .. ... .. R[C]
        private const int JMP = 22;      // pc+=sBx
        private const int EQ = 23;       // if ((RK[B] == RK[C]) ~= A) then pc++
        private const int LT = 24;       // if ((RK[B] <  RK[C]) ~= A) then pc++
        private const int LE = 25;       // if ((RK[B] <= RK[C]) ~= A) then pc++
        private const int TEST = 26;     // if not (R[A] <=> C) then pc++
        private const int TESTSET = 27;  // if (R[B] <=> C) then R[A] := R[B] else pc++
        private const int CALL = 28;     // R[A], ... ,R[A+C-2] := R[A](R[A+1], ... ,R[A+B-1])
        private const int RETURN = 29;   // return R[A], ... ,R[A+B-2]
        private const int FORLOOP = 30;  // R[A]+=-1; if R[A] <= R[A+2] then { pc+=sBx; R[A+3]=R[A] }
        private const int FORPREP = 31;  // R[A]-=R[A+2]; pc+=sBx
        private const int TFORCALL = 32; // R[A+3], ... ,R[A+2+C] := R[A](R[A+1], R[A+2])
        private const int TFORLOOP = 33; // if R[A+1] ~= nil then { R[A]=R[A+1]; pc += sBx }

        static void Main(string[] args)
        {
            string obfuscatedScript = "AAAAAAAABgAAAAAABQAAAAABAAAAAAo="; // Example Base64-encoded Lua bytecode
            string decompiledScript = DecompileScript(obfuscatedScript);

            Console.WriteLine("Decompiled Script:");
            Console.WriteLine(decompiledScript);
        }

        static string DecompileScript(string bytecode)
        {
            try
            {
                byte[] bytecodeBytes = Convert.FromBase64String(bytecode);

                string decompiledScript = BytecodeToLua(bytecodeBytes);

                return decompiledScript;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decompilation failed: {ex.Message}");
                return bytecode;
            }
        }

        static string BytecodeToLua(byte[] bytecode)
        {
            StringBuilder luaSource = new StringBuilder();
            luaSource.Append("-- Decompiled Lua script\n");

            int index = 0;
            while (index < bytecode.Length)
            {
                if (index + 3 >= bytecode.Length)
                    break;

                // Each Lua instruction is 4 bytes
                uint instruction = BitConverter.ToUInt32(bytecode, index);
                luaSource.Append(ParseInstruction(instruction));
                index += 4;
            }

            return luaSource.ToString();
        }

        static string ParseInstruction(uint instruction)
        {
            int opcode = (int)(instruction & 0x3F);        // 6 bits
            int a = (int)((instruction >> 6) & 0xFF);      // 8 bits
            int b = (int)((instruction >> 23) & 0x1FF);    // 9 bits
            int c = (int)((instruction >> 14) & 0x1FF);    // 9 bits
            int bx = (int)((instruction >> 14) & 0x3FFFF); // 18 bits
            int sbx = bx - 131071; // signed Bx

            switch (opcode)
            {
                case MOVE:
                    return $"R[{a}] = R[{b}]\n";
                case LOADK:
                    return $"R[{a}] = K[{bx}]\n";
                case LOADBOOL:
                    return $"R[{a}] = {(b != 0 ? "true" : "false")}; if ({c} != 0) then pc++ end\n";
                case LOADNIL:
                    return $"for i = {a}, {a + b} do R[i] = nil end\n";
                case GETUPVAL:
                    return $"R[{a}] = UpValue[{b}]\n";
                case GETGLOBAL:
                    return $"R[{a}] = G[{bx}]\n";
                case GETTABLE:
                    return $"R[{a}] = R[{b}][RK[{c}]]\n";
                case SETGLOBAL:
                    return $"G[{bx}] = R[{a}]\n";
                case SETUPVAL:
                    return $"UpValue[{b}] = R[{a}]\n";
                case SETTABLE:
                    return $"R[{a}][RK[{b}]] = RK[{c}]\n";
                case NEWTABLE:
                    return $"R[{a}] = {{}} -- size = {b},{c}\n";
                case SELF:
                    return $"R[{a + 1}] = R[{b}]; R[{a}] = R[{b}][RK[{c}]]\n";
                case ADD:
                    return $"R[{a}] = RK[{b}] + RK[{c}]\n";
                case SUB:
                    return $"R[{a}] = RK[{b}] - RK[{c}]\n";
                case MUL:
                    return $"R[{a}] = RK[{b}] * RK[{c}]\n";
                case DIV:
                    return $"R[{a}] = RK[{b}] / RK[{c}]\n";
                case MOD:
                    return $"R[{a}] = RK[{b}] % RK[{c}]\n";
                case POW:
                    return $"R[{a}] = RK[{b}] ^ RK[{c}]\n";
                case UNM:
                    return $"R[{a}] = -R[{b}]\n";
                case NOT:
                    return $"R[{a}] = not R[{b}]\n";
                case LEN:
                    return $"R[{a}] = #R[{b}]\n";
                case CONCAT:
                    return $"R[{a}] = R[{b}] .. R[{c}]\n";
                case JMP:
                    return $"pc += {sbx}\n";
                case EQ:
                    return $"if R[{b}] == R[{c}] then pc++ end\n";
                case LT:
                    return $"if R[{b}] < R[{c}] then pc++ end\n";
                case LE:
                    return $"if R[{b}] <= R[{c}] then pc++ end\n";
                case TEST:
                    return $"if not R[{a}] then pc++ end\n";
                case TESTSET:
                    return $"if R[{b}] then R[{a}] = R[{b}] else pc++ end\n";
                case CALL:
                    return $"R[{a}], ... ,R[{a+c-2}] = R[{a}]({GetArguments(a + 1, b - 1)})\n";
                case RETURN:
                    return $"return {GetArguments(a, b - 1)}\n";
                case FORLOOP:
                    return $"R[{a}] += -1; if R[{a}] <= R[{a + 2}] then pc += {sbx}; R[{a + 3}] = R[{a}] end\n";
                case FORPREP:
                    return $"R[{a}] -= R[{a + 2}]; pc += {sbx}\n";
                case TFORCALL:
                    return $"R[{a + 3}], ... ,R[{a + 2 + c}] = R[{a}]({GetArguments(a + 1, 2)})\n";
                case TFORLOOP:
                    return $"if R[{a + 1}] ~= nil then R[{a}] = R[{a + 1}]; pc += {sbx} end\n";
                default:
                    return $"-- Unknown instruction: {instruction}\n";
            }
        }

        static string GetArguments(int start, int count)
        {
            StringBuilder args = new StringBuilder();
            for (int i = start; i < start + count; i++)
            {
                if (i > start) args.Append(", ");
                args.Append($"R[{i}]");
            }
            return args.ToString();
        }
    }
}
