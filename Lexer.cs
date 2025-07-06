using System.Globalization;
using System.Numerics;

namespace Cliex
{
    [Flags]
    public enum Tokens
    {
        Identifier,
        String,
        Char,
        Byte,
        SByte,
        UShort,
        Short,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        Decimal,
        Boolean
    }

    public class Token
    {
        public Tokens Type { get; private set; }
        public object? Value { get; private set; }

        public Token(Tokens type, object? value)
        {
            Type = type;
            Value = value;
        }

        public bool IsInteger()
        {
            return Type.HasFlag(Tokens.Byte | Tokens.SByte | Tokens.UShort | Tokens.Short | Tokens.Int | Tokens.UInt | Tokens.Long | Tokens.ULong);
        }

        public bool IsFloat()
        {
            return Type.HasFlag(Tokens.Float | Tokens.Double | Tokens.Decimal);
        }

        public bool IsNumber()
        {
            return IsInteger() || IsFloat();
        }

        public override string ToString()
        {
            return $"\"{Type}\" : {Value}";
        }
    }

    public class Lexer
    {
        private static string Alphas { get; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string Digits { get; } = "0123456789";
        
        public string Source { get; private set; }
        public char Current { get; private set; }
        public int Index { get; private set; }

        public Lexer(string source)
        {
            Source = source;
            Current = '\0';
            Index = -1;
            
            Advance();
        }

        private void Advance()
        {
            Index++;
            Current = Index < Source.Length ? Source[Index] : '\0';
        }

        public List<Token> Analyse()
        {
            List<Token> result = new();

            while (Current != '\0')
            {
                char next = Index + 1 < Source.Length ? Source[Index + 1] :'\0';

                if ((Alphas + "-_").Contains(Current))
                {
                    result.Add(GenerateIdentifier());
                }
                else if (Digits.Contains(Current) || (Current == '.' && Digits.Contains(next)))
                {
                    result.Add(GenerateNumber());
                }
                else if (Current == '\"')
                {
                    Advance();
                    result.Add(GenerateString());
                }
                else if (Current == '\'')
                {
                    Advance();
                    result.Add(GenerateChar());
                }
                else Advance();
            }

            return result;
        }

        private Token GenerateIdentifier()
        {
            string result = "";

            while ((Alphas + "-_").Contains(Current))
            {
                result += Current;
                
                Advance();
            }

            if (result == "true") return new(Tokens.Boolean, true);

            if (result == "false") return new(Tokens.Boolean, false);

            return new(Tokens.Identifier, result);
        }

        private Token GenerateString()
        {
            string result = "";
            bool escaping = false;

            while (Current != '\0' && Current != '\"')
            {
                if (Current == '\\' && !escaping)
                {
                    escaping = true;
                    
                    Advance();
                    
                    continue;
                }
                
                if (escaping)
                {
                    switch (Current)
                    {
                        case '0': result += '\0'; Advance(); continue;
                        case 'n': result += '\n'; Advance(); continue;
                        case 't': result += '\t'; Advance(); continue;
                        case 'b': result = result[..^1]; Advance(); continue;
                        case '\\': result += '\\'; Advance(); continue;
                        case '\'': result += '\''; Advance(); continue;
                        case '\"': result += '\"'; Advance(); continue;
                        default: throw new NotImplementedException($"Unknown escape character: \'{Current}\'.");
                    }
                    
                    escaping = false;
                }

                result += Current;
                
                Advance();
            }
            
            Advance();

            return new(Tokens.String, result);
        }

        private Token GenerateChar()
        {
            char result = '\0';

            if (Current == '\\')
            {
                Advance();
                
                switch (Current)
                {
                    case '0': result = '\0'; Advance(); break;
                    case 'n': result = '\n'; Advance(); break;
                    case 't': result = '\t'; Advance(); break;
                    case 'b': result = '\0'; Advance(); break;
                    case '\\': result = '\\'; Advance(); break;
                    case '\'': result = '\''; Advance(); break;
                    case '\"': result = '\"'; Advance(); break;
                    default: throw new NotImplementedException($"Unknown escape character: \'{Current}\'.");
                }
            }
            else result = Current;
            
            Advance();

            if (Current != '\'') throw new ArgumentOutOfRangeException($"Character can only contains one character.");
            
            Advance();

            return new(Tokens.Char, result);
        }

        private Token GenerateNumber()
        {
            string result = "";
            bool dotted = false;

            while ((Digits + ".bsilfd").Contains(char.ToLower(Current)))
            {
                if (Current == '.')
                {
                    if (dotted) break;

                    dotted = true;
                }

                if ("bsilfd".Contains(char.ToLower(Current)))
                {
                    result += Current;
                    
                    Advance();
                    
                    break;
                }

                result += Current;
                
                Advance();
            }

            if (result[0] == '.') result = '0' + result;

            object number;
            Tokens type;
            
            switch (result[^1])
            {
                case 'b':
                    if (!byte.TryParse(result[..^1], out byte byteValue))
                        throw new OverflowException($"Invalid byte value: {byteValue}, a byte can only be between from 0 to 255.");

                    type = Tokens.Byte;
                    number = byteValue;
                    
                    break;
                case 'B':
                    if (!sbyte.TryParse(result[..^1], out sbyte sbyteValue))
                        throw new OverflowException($"Invalid sbyte value: {sbyteValue}, an sbyte can only be between from -128 to 127.");

                    type = Tokens.SByte;
                    number = sbyteValue;
                    
                    break;
                case 's':
                    if (!ushort.TryParse(result[..^1], out ushort ushortValue))
                        throw new OverflowException($"Invalid ushort value: {ushortValue}, a ushort can only be between from 0 to 65,535.");

                    type = Tokens.UShort;
                    number = ushortValue;
                    
                    break;
                case 'S':
                    if (!short.TryParse(result[..^1], out short shortValue))
                        throw new OverflowException($"Invalid short value: {shortValue}, a short can only be between from -32,768 to 32,767.");

                    type = Tokens.Short;
                    number = shortValue;
                    
                    break;
                case 'i':
                    if (!uint.TryParse(result[..^1], out uint uintValue))
                        throw new OverflowException($"Invalid uint value: {uintValue}, a uint can only be between from 0 to 4,294,967,295.");

                    type = Tokens.UInt;
                    number = uintValue;
                    
                    break;
                case 'I':
                    if (!int.TryParse(result[..^1], out int intValue))
                        throw new OverflowException($"Invalid int value: {intValue}, an int can only be between from -2,147,483,648 to 2,147,483,647.");

                    type = Tokens.Int;
                    number = intValue;
                    
                    break;
                case 'l':
                    if (!ulong.TryParse(result[..^1], out ulong ulongValue))
                        throw new OverflowException($"Invalid ulong value: {ulongValue}, a ulong can only be between from 0 to 18,446,744,073,709,551,615.");

                    type = Tokens.ULong;
                    number = ulongValue;
                    
                    break;
                case 'L':
                    if (!long.TryParse(result[..^1], out long longValue))
                        throw new OverflowException($"Invalid long value: {longValue}, a long can only be between from -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.");

                    type = Tokens.Long;
                    number = longValue;
                    
                    break;
                case 'f':
                    type = Tokens.Float;
                    number = float.Parse(result[..^1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    
                    break;
                case 'F': goto case 'f';
                case 'd': 
                    type = Tokens.Double;
                    number = double.Parse(result[..^1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    
                    break;
                case 'D': 
                    type = Tokens.Decimal;
                    number = decimal.Parse(result[..^1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    
                    break;
                default:
                    if (!dotted)
                    {
                        if (!int.TryParse(result[..^1], out intValue))
                            throw new OverflowException($"Invalid int value: {intValue}, an int can only be between from -2,147,483,648 to 2,147,483,647.");

                        type = Tokens.Int;
                        number = intValue;
                        
                        break;
                    }
                    
                    type = Tokens.Float;
                    number = float.Parse(result, NumberStyles.Float, CultureInfo.InvariantCulture);
                    
                    break;
            }

            return new(type, number);
        }
    }
}