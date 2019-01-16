if [ -z "$1" ]
  then
    echo "Please specify tag for release"
else
	dotnet publish -c Release
	docker build -t dziejekhorinis/daedalus-compiler:$1 .
fi