FROM microsoft/dotnet AS build-env
WORKDIR /

RUN git clone https://github.com/zaniu/ShoppingCart.git
WORKDIR /ShoppingCart/ShoppingCart
RUN dotnet restore
CMD ["dotnet", "run"]