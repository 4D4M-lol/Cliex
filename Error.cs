namespace Cliex
{
    public class CliexCharacterError : Exception
    {
        public CliexCharacterError(string message = "No reason provided.") : base(message)
        {
            
        }
    }
    
    public class CliexSyntaxError : Exception
    {
        public CliexSyntaxError(string message = "No reason provided.") : base(message)
        {
            
        }
    }
    
    public class CliexOverflowError : Exception
    {
        public CliexOverflowError(string message = "No reason provided.") : base(message)
        {
            
        }
    }
    
    public class CliexDuplicateKeyError : Exception
    {
        public CliexDuplicateKeyError(string message = "No reason provided.") : base(message)
        {
            
        }
    }
}