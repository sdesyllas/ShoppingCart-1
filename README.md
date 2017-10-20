# ShoppingCart
Simple web API developed with ASP.NET Core for handling shopping baskets.

Assumptions
- Static list of baskets (cart1, cart2) with no option to create, delete
- Product definitions and stock provided from csv file
- In memory "database" - no data persistence, reload after application restart
- Cart item can contain multiple entries for same product. Each entry cannot exceed current stock. Quantity aggregation and validation is made on checkout 

Tools
- AutoMapper
- SwaggerUI
- CsvHelper
- FluentAssertions
- Moq
- SimpleFixture 

Further development
- Data persistence
- Thread safe stock operations
- Builder pattern for stubs creation to avoid code duplication in unit tests 


Working version could be obtained via docker:
https://hub.docker.com/r/zaniu/test/

docker pull zaniu/test:latest

docker run -p 64580:64580 -e "ASPNETCORE_URLS=http://+:64580" -it --rm zaniu/test:latest

and go to:

http://localhost:64580/swagger/

