using ShoppingCart.Shared.Dto;
using CartItemModel = ShoppingCart.Shared.Model.CartItem;
using System.Collections;

namespace ShoppingCart.UnitTests.Controllers.Validators
{
    internal class AddCartItemToCartItemModelComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if(!(x is AddCartItemDto && y is CartItemModel))
            {
                return -1;
            }

            AddCartItemDto body = (AddCartItemDto)x;
            CartItemModel collectionElement = (CartItemModel)y;
            
            if(body.ID != collectionElement.ID)
            {
                return -1;
            }

            if(body.Quantity != collectionElement.Quantity)
            {
                return -1;
            }

            return 0;
        }
    }
}
