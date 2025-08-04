ARG APP_VERSION=0.0.0
ARG APP_BUILD=7777
ARG PERFORM_SONARSCAN=0
ARG SONAR_TOKEN
ARG PR_BRANCH
ARG PR_NUMBER

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
ARG APP_VERSION
ARG APP_BUILD
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

ENV CONTEXTROOT=""
ENV ELASTICSEARCHURL http://localhost:9200
ENV NAMESPACE docker
ENV SERVICENAME type-lookup-service
ENV MEDCOMPASSRELEASE Undefined
ENV LOGGING__LOGLEVEL__DEFAULT "Information"
ENV LOGGING__LOGLEVEL__SYSTEM "Warning"
ENV LOGGING__LOGLEVEL__MICROSOFT "Warning"
ENV RESILIENTMESSAGING__MAXTIMEBETWEENRETRIESMS "300000"

RUN apt-get update && apt-get install git -y
RUN apt-get install jq -y

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG APP_VERSION
ARG APP_BUILD
ARG PERFORM_SONARSCAN
ARG SONAR_TOKEN
ARG PR_BRANCH
ARG PR_NUMBER

WORKDIR /src
COPY ["type-lookup-service/type-lookup-service.csproj", "type-lookup-service/"]
COPY ["type-lookup-service.Tests/type-lookup-service.Tests.csproj", "type-lookup-service.Tests/"] 
RUN dotnet restore "type-lookup-service/type-lookup-service.csproj"
RUN dotnet restore "type-lookup-service.Tests/type-lookup-service.Tests.csproj" 
COPY . .

ENV SONAR_URL "https://sonarcloud.io" 
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN echo PR_NUMBER=${PR_NUMBER} PR_BRANCH=${PR_BRANCH} PERFORM_SONARSCAN=${PERFORM_SONARSCAN}

RUN if [ $PERFORM_SONARSCAN = 1 ]; then \
	  apt-get update && apt-get install -y default-jre; \
	  dotnet tool install --global dotnet-sonarscanner --version 4.8.0; \
	fi
    
RUN if [ $PERFORM_SONARSCAN = 1 ] && [ -z $PR_NUMBER ]; then \
	dotnet sonarscanner begin \
		/k:type-lookup-service \
		/o:assurecare \
		/d:sonar.host.url=${SONAR_URL} \
		/d:sonar.login=${SONAR_TOKEN} \
		/d:projectVersion='${APP_VERSION}.${APP_BUILD}' \
		/d:sonar.cs.opencover.reportsPaths="/src/type-lookup-service.Tests/coverage.opencover.xml" \
		/d:sonar.cs.xunit.reportsPaths="/src/type-lookup-service.Tests/TestResults/*.xml" \
		/d:sonar.coverage.exclusions="**Tests*.cs" \
		/d:sonar.verbose=false; \
elif [ $PERFORM_SONARSCAN = 1 ]; then \
	dotnet sonarscanner begin \
		/k:type-lookup-service \
		/o:assurecare \
		/d:sonar.host.url=${SONAR_URL} \
		/d:sonar.login=${SONAR_TOKEN} \
		/d:projectVersion='${APP_VERSION}.${APP_BUILD}' \
		/d:sonar.pullrequest.key=${PR_NUMBER} \
		/d:sonar.pullrequest.branch=${PR_BRANCH} \ 
		/d:sonar.pullrequest.provider=vsts \
		/d:sonar.pullrequest.vsts.instanceUrl="https://assurecaresoftware.visualstudio.com/" \
		/d:sonar.pullrequest.vsts.project="MedCompass" \
		/d:sonar.pullrequest.vsts.repository="type-lookup-service" \
        /d:sonar.cs.opencover.reportsPaths="/src/type-lookup-service.Tests/coverage.opencover.xml" \
		/d:sonar.cs.xunit.reportsPaths="/src/type-lookup-service.Tests/TestResults/*.xml" \
		/d:sonar.coverage.exclusions="**Tests*.cs" \
		/d:sonar.verbose=false; \
fi

RUN dotnet build "type-lookup-service.sln" --no-restore -c Release -o /app

WORKDIR "/src/type-lookup-service.Tests"
RUN dotnet test "type-lookup-service.Tests.csproj" --logger "xunit;LogFileName=/app/type-lookup-service-tests-results.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover; exit 0 

WORKDIR /src
RUN if [ $PERFORM_SONARSCAN = 1 ]; then dotnet sonarscanner end /d:sonar.login=${SONAR_TOKEN}; fi

WORKDIR "/src/type-lookup-service"

FROM build AS publish
ARG APP_VERSION
ARG APP_BUILD
RUN  echo "Buidling Version ${APP_VERSION}.${APP_BUILD}" 
RUN dotnet publish "type-lookup-service.csproj" /p:Version="${APP_VERSION}.${APP_BUILD}" /p:InformationalVersion="$APP_VERSION.$APP_BUILD" -c Release -o /app

FROM base AS final
ARG APP_VERSION
ARG APP_BUILD
WORKDIR /app
COPY --from=publish /app .
COPY ./*.sh /app/
RUN chmod +x /app/entrypoint.sh
RUN printf "${APP_VERSION}.${APP_BUILD}" > version.txt
ENTRYPOINT ["/app/entrypoint.sh"]
