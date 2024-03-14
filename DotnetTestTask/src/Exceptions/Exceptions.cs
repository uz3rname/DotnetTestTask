namespace DotnetTestTask.Exceptions
{
    public class InvalidAmount : Exception
    {
        public InvalidAmount(string message) : base(message)
        {
        }
        public InvalidAmount(string message, Exception ex) : base(message, ex)
        {
        }
    }
}