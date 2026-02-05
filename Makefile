SOLUTION=FluxOps.DotNet.sln
IMAGE=fluxops-api:local

.PHONY: restore build test docker-build docker-run docker-stop

restore:
	dotnet restore $(SOLUTION)

build:
	dotnet build $(SOLUTION) -c Release --no-restore

test:
	dotnet test $(SOLUTION) -c Release --no-build --verbosity normal

docker-build:
	docker build -f FluxOps.Api/Dockerfile -t $(IMAGE) .

docker-run:
	docker run --rm -d -p 8080:8080 --name fluxops-api $(IMAGE)

docker-stop:
	docker stop fluxops-api || true
