ARG APP_VERSION=0.0.0
ARG APP_BUILD=7777
ARG PERFORM_SONARSCAN=0
ARG SONAR_TOKEN
ARG PR_BRANCH
ARG PR_NUMBER

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ARG APP_VERSION
ARG APP_BUILD
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

ENV CONTEXTROOT=""
ENV NAMESPACE docker
ENV SERVICENAME time-track-service
ENV MEDCOMPASSRELEASE Undefined
ENV LOGGING__LOGLEVEL__DEFAULT "Information"
ENV LOGGING__LOGLEVEL__SYSTEM "Warning"
ENV LOGGING__LOGLEVEL__MICROSOFT "Warning"
ENV RESILIENTMESSAGING__MAXTIMEBETWEENRETRIESMS ""
ENV MONGOCLIENTCONNECTIONSETTINGS__CONNECTIONURL "unsetDBConnection"
ENV MONGOCLIENTCONNECTIONSETTINGS__COLLECTION "mongoCollection"
ENV MONGOCLIENTCONNECTIONSETTINGS__PUBLICKEYPASSWORD PublicKeyPassword
ENV MONGOCLIENTCONNECTIONSETTINGS__USETLS "false"
ENV MONGOCLIENTCONNECTIONSETTINGS__USEPUBLICKEYCONNECTION "false"



ENV SERVICES__SYNCBACKURI ""
ENV SERVICES__HYDRATIONSECURITYKEY ""
ENV SERVICES__TYPELOOKUPURI ""
ENV SERVICES__ALLOWLOADTESTACTIONS "false"

ENV DATAAUTHORIZATION__FIELD__URL "http://data-authorization-micro-service/api/DataAuthorization/fields"
ENV DATAAUTHORIZATION__OPERATION__URL "http://data-authorization-micro-service/api/DataAuthorization/operations"
ENV DATAAUTHORIZATION__ROW__URL "http://data-authorization-micro-service/api/DataAuthorization/rows"
# Mongo Client Connection Settings

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG APP_VERSION
ARG APP_BUILD
ARG PERFORM_SONARSCAN
ARG SONAR_TOKEN
ARG PR_BRANCH
ARG PR_NUMBER

ARG PAT

# Install the nuget credential provider
RUN apt-get update && apt-get install -y locales \
# Download the artifact credential provider
&& wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash

WORKDIR /src
COPY ["nuget.config", "."]
COPY ["time-track-service/time-track-service.csproj", "time-track-service/"]
COPY ["time-track-service.Tests/time-track-service.Tests.csproj", "time-track-service.Tests/"] 

# Use the secret to set the credential provider variable and run restore.
RUN export ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://assurecaresoftware.pkgs.visualstudio.com/_packaging/MedCompass_Web/nuget/v3/index.json\", \"username\":\"build\", \"password\":\"${PAT}\"}]}" \
&& dotnet restore "time-track-service/time-track-service.csproj" \
&& dotnet restore "time-track-service.Tests/time-track-service.Tests.csproj"  \
&& unset VSS_NUGET_EXTERNAL_FEED_ENDPOINTS
COPY . .

ENV SONAR_URL "https://sonarcloud.io" 
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN echo PR_NUMBER=${PR_NUMBER} PR_BRANCH=${PR_BRANCH} PERFORM_SONARSCAN=${PERFORM_SONARSCAN}

RUN if [ $PERFORM_SONARSCAN = 1 ]; then \
	  apt-get update && apt-get install -y default-jre; \
	  dotnet tool install --global dotnet-sonarscanner --version 5.11.0; \
	fi
    
RUN if [ $PERFORM_SONARSCAN = 1 ] && [ -z $PR_NUMBER ]; then \
	dotnet sonarscanner begin \
		/k:"assurecare:time-track-service" \
		/o:"assurecare" \
		/d:sonar.host.url=${SONAR_URL} \
		/d:sonar.login=${SONAR_TOKEN} \
		/d:projectVersion='${APP_VERSION}.${APP_BUILD}' \
		/d:sonar.cs.opencover.reportsPaths="/src/time-track-service.Tests/coverage.opencover.xml" \
		/d:sonar.cs.xunit.reportsPaths="/src/time-track-service.Tests/TestResults/*.xml" \
		/d:sonar.coverage.exclusions="**/Data/*.*,**/Controllers/*.*,**/Context/*.*,**/Startup.cs,**/Program.cs,**/Extensions/HealthReportExtensions.cs,**/Utils/Attributes/*.*,**/Utils/DeepCopy/*.*,**/Utils/ApplicationExceptionHandler.cs,**/Utils/AssureCareBindings.cs,**/Utils/ContextLogger.cs,**/Utils/RequestContextAccessor.cs,**/Utils/RetryHelper.cs,**/Model/Dto/MongoClientConnectionSettings.cs,**Tests*.cs" \
		/d:sonar.verbose=false; \
elif [ $PERFORM_SONARSCAN = 1 ]; then \
	dotnet sonarscanner begin \
		/k:"assurecare:time-track-service" \
		/o:"assurecare" \
		/d:sonar.host.url=${SONAR_URL} \
		/d:sonar.login=${SONAR_TOKEN} \
		/d:projectVersion='${APP_VERSION}.${APP_BUILD}' \
		/d:sonar.pullrequest.key=${PR_NUMBER} \
		/d:sonar.pullrequest.branch=${PR_BRANCH} \ 
		/d:sonar.pullrequest.provider=vsts \
		/d:sonar.pullrequest.vsts.instanceUrl="https://assurecaresoftware.visualstudio.com/" \
		/d:sonar.pullrequest.vsts.project="MedCompassMicro" \
		/d:sonar.pullrequest.vsts.repository="time-track-service" \
        /d:sonar.cs.opencover.reportsPaths="/src/time-track-service.Tests/coverage.opencover.xml" \
		/d:sonar.cs.xunit.reportsPaths="/src/time-track-service.Tests/TestResults/*.xml" \
		/d:sonar.coverage.exclusions="**/Data/*.*,**/Controllers/*.*,**/Context/*.*,**/Startup.cs,**/Program.cs,**/Extensions/HealthReportExtensions.cs,**/Utils/Attributes/*.*,**/Utils/DeepCopy/*.*,**/Utils/ApplicationExceptionHandler.cs,**/Utils/AssureCareBindings.cs,**/Utils/ContextLogger.cs,**/Utils/RequestContextAccessor.cs,**/Utils/RetryHelper.cs,**/Model/Dto/MongoClientConnectionSettings.cs,**Tests*.cs" \
		/d:sonar.verbose=false; \
fi

RUN dotnet build "time-track-service.sln" --no-restore -c Release -o /app

WORKDIR "/src/time-track-service.Tests"
RUN dotnet test "time-track-service.Tests.csproj" --collect:"XPlat Code Coverage" --logger "xunit;LogFileName=/app/time-track-service-tests-results.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=\"opencover,cobertura\" /p:ExcludeByFile=\"**/Data/*.*,**/Controllers/*.*,**/Context/*.*,**/Startup.cs,**/Program.cs,**/Extensions/HealthReportExtensions.cs,**/Utils/Attributes/*.*,**/Utils/DeepCopy/*.*,**/Utils/ApplicationExceptionHandler.cs,**/Utils/AssureCareBindings.cs,**/Utils/ContextLogger.cs,**/Utils/RequestContextAccessor.cs,**/Utils/RetryHelper.cs,**/Model/Dto/MongoClientConnectionSettings.cs\" /p:SkipAutoProps=true; exit 0 

WORKDIR /src
RUN if [ $PERFORM_SONARSCAN = 1 ]; then dotnet sonarscanner end /d:sonar.login=${SONAR_TOKEN}; fi

WORKDIR "/src/time-track-service"

FROM build AS publish
ARG APP_VERSION
ARG APP_BUILD
RUN  echo "Buidling Version ${APP_VERSION}.${APP_BUILD}" 
RUN dotnet publish "time-track-service.csproj" /p:Version="${APP_VERSION}.${APP_BUILD}" /p:InformationalVersion="$APP_VERSION.$APP_BUILD" -c Release -o /app

FROM base AS final
ARG APP_VERSION
ARG APP_BUILD
WORKDIR /app
COPY --from=publish /app .
RUN printf "${APP_VERSION}.${APP_BUILD}" > version.txt
ENTRYPOINT ["dotnet", "time-track-service.dll"]
