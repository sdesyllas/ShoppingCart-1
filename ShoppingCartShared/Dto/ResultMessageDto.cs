namespace ShoppingCart.Shared.Dto
{
    public class ResultMessageDto
    {
        public string Message { get; private set; }
        
        public ResultMessageDto(string message)
        {
            Message = message;
        }
    }
}
