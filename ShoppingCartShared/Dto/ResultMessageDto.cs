namespace ShoppingCart.Shared.Dto
{
    /// <summary>
    /// Dto for representing response message
    /// </summary>
    public class ResultMessageDto
    {
        public string Message { get; private set; }
        
        public ResultMessageDto(string message)
        {
            Message = message;
        }
    }
}
