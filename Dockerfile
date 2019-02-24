FROM microsoft/dotnet:2-runtime
RUN mkdir -p /usr/compiler-runtime
RUN mkdir -p /usr/workspace
WORKDIR /usr/workspace
COPY ./src/DaedalusCompiler/bin/Release/netcoreapp2.1/publish /usr/compiler-runtime
RUN echo 'alias daedalus-compiler="dotnet /usr/compiler-runtime/DaedalusCompiler.dll"' >> ~/.bashrc
ENTRYPOINT ["dotnet", "/usr/compiler-runtime/DaedalusCompiler.dll"]