namespace DotnetTestTask.Exceptions
{
    public class InvalidAmount : Exception
    {
        public InvalidAmount(string message) : base(message)
        {
        }
    }
}