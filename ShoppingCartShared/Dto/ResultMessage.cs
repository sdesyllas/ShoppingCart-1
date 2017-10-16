namespace ShoppingCart.Shared.Dto
{
    public class ResultMessage
    {
        public string Message { get; private set; }
        
        public ResultMessage(string message)
        {
            Message = message;
        }
    }
}
