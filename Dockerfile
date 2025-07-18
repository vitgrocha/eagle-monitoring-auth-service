# Usando imagem oficial do .NET SDK para construir o projeto
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia o arquivo csproj e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o código e publica a aplicação
COPY . ./
RUN dotnet publish -c Release -o out

# Imagem para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expõe a porta 80 (HTTP)
EXPOSE 80

# Comando para rodar o microserviço
ENTRYPOINT ["dotnet", "MSAuthentication.dll"]
