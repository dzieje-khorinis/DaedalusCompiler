FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
RUN mkdir -p /usr/compiler-runtime
RUN mkdir -p /usr/workspace
WORKDIR /usr/workspace
COPY ./src/DaedalusCompiler/bin/Release/net8.0/publish /usr/compiler-runtime
RUN echo 'alias daedalus-compiler="dotnet /usr/compiler-runtime/DaedalusCompiler.dll"' >> ~/.bashrc
ENTRYPOINT ["dotnet", "/usr/compiler-runtime/DaedalusCompiler.dll"]