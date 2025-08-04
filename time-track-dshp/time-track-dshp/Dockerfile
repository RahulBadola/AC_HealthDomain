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
ENV SERVICENAME time-track-dshp
ENV MEDCOMPASSRELEASE Undefined
ENV LOGGING__LOGLEVEL__DEFAULT "Information"
ENV LOGGING__LOGLEVEL__SYSTEM "Warning"
ENV LOGGING__LOGLEVEL__MICROSOFT "Warning"

ENV MESSAGING__KAFKA__CONNECTIONTYPE ""
ENV MESSAGING__KAFKA__BOOTSTRAPSERVERS ""
ENV MESSAGING__KAFKA__SASLMECHANISM ""
ENV MESSAGING__KAFKA__SASLUSERNAME ""
ENV MESSAGING__KAFKA__SASLPASSWORD ""
ENV MESSAGING__KAFKA__CACERTLOCATION ""
ENV MESSAGING__KAFKA__AUTOOFFSETRESET ""

ENV MESSAGING__KAFKA__HYDRATIONCONSUMERGROUPID ""
ENV MESSAGING__KAFKA__HYDRATIONCONSUMERTOPIC ""
ENV MESSAGING__KAFKA__HYDRATIONCONSUMERSCHEMAFILELOCATION ""
ENV MESSAGING__KAFKA__HYDRATIONCONSUMERSCHEMAKATTRIBUTESNAME ""

ENV MESSAGING__KAFKA__SYNCBACKPRODUCERTOPIC ""
ENV MESSAGING__KAFKA__SYNCBACKPRODUCERSCHEMAFILELOCATION ""

ENV MESSAGING__KAFKA__SYNCBACKCONSUMERGROUPID ""
ENV MESSAGING__KAFKA__SYNCBACKCONSUMERTOPIC ""
ENV MESSAGING__KAFKA__SYNCBACKCONSUMERSCHEMAFILELOCATION ""

ENV MESSAGING__KAFKA__HYDRATIONBULKCONSUMERTOPIC ""
ENV MESSAGING__KAFKA__TARGETCOMMITBATCHSIZE ""

ENV CONNECTIONSTRINGS__DEFAULTCONNECTION ""

ENV DOMAINMICROSERVICE__PROTOCOL ""
ENV DOMAINMICROSERVICE__HOST ""
ENV DOMAINMICROSERVICE__PORT ""
ENV DOMAINMICROSERVICE__MEMBERSECURITYPROFILEENDPOINT ""
ENV DOMAINMICROSERVICE__SECURITYFIELDENDPOINT ""
ENV DOMAINMICROSERVICE__SECURITYROLEENDPOINT ""
ENV DOMAINMICROSERVICE__SECURITYFIELDSECURITYROLEMAPENDPOINT ""
ENV DOMAINMICROSERVICE__TENANTID ""
ENV DOMAINMICROSERVICE__HYDRATIONSYNCKEY ""

ENV RULEEXECUTIONSERVICE__PROTOCOL ""
ENV RULEEXECUTIONSERVICE__HOST ""
ENV RULEEXECUTIONSERVICE__PORT ""
ENV RULEEXECUTIONSERVICE__ENDPOINT ""
ENV RULEEXECUTIONSERVICE__TENANTID ""
ENV RULEEXECUTIONSERVICE__APIKEY ""

ENV AUTH__CLIENTID ""
ENV AUTH__CLIENTSECRET ""
ENV AUTH__CLIENTUSERNAME ""
ENV AUTH__CLIENTPASSWORD ""
ENV AUTH__OAUTHENDPOINT ""

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG APP_VERSION
ARG APP_BUILD
ARG PERFORM_SONARSCAN
ARG SONAR_TOKEN
ARG PR_BRANCH
ARG PR_NUMBER

WORKDIR /src
COPY ["time-track-dshp/time-track-dshp.csproj", "time-track-dshp/"]
COPY ["time-track-dshp.Tests/time-track-dshp.Tests.csproj", "time-track-dshp.Tests/"] 
RUN dotnet restore "time-track-dshp/time-track-dshp.csproj"
RUN dotnet restore "time-track-dshp.Tests/time-track-dshp.Tests.csproj" 
COPY . .

ENV SONAR_URL "https://sonarcloud.io" 
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN echo PR_NUMBER=${PR_NUMBER} PR_BRANCH=${PR_BRANCH} PERFORM_SONARSCAN=${PERFORM_SONARSCAN}

RUN if [ $PERFORM_SONARSCAN = 1 ]; then \
	  apt-get update && apt-get install -y default-jre; \
	  dotnet tool install --global dotnet-sonarscanner --version 5.0.4; \
	fi
    
RUN if [ $PERFORM_SONARSCAN = 1 ] && [ -z $PR_NUMBER ]; then \
	dotnet sonarscanner begin \
		/k:"assurecare:time-track-dshp" \
		/o:"assurecare" \
		/d:sonar.host.url=${SONAR_URL} \
		/d:sonar.login=${SONAR_TOKEN} \
		/d:projectVersion='${APP_VERSION}.${APP_BUILD}' \
		/d:sonar.cs.opencover.reportsPaths="/src/time-track-dshp.Tests/coverage.opencover.xml" \
		/d:sonar.cs.xunit.reportsPaths="/src/time-track-dshp.Tests/TestResults/*.xml" \
		/d:sonar.coverage.exclusions="**/Data/*.*,**/Controllers/*.*,**/Context/*.*,**/Startup.cs,**/Program.cs,**/Extensions/HealthReportExtensions.cs,**/Utils/Attributes/*.*,**/Utils/DeepCopy/*.*,**/Utils/ApplicationExceptionHandler.cs,**/Utils/AssureCareBindings.cs,**/Utils/ContextLogger.cs,**/Utils/RequestContextAccessor.cs,**/Utils/RetryHelper.cs,**/Model/Dto/MongoClientConnectionSettings.cs,**Tests*.cs" \
		/d:sonar.verbose=false; \
elif [ $PERFORM_SONARSCAN = 1 ]; then \
	dotnet sonarscanner begin \
		/k:"assurecare:time-track-dshp" \
		/o:"assurecare" \
		/d:sonar.host.url=${SONAR_URL} \
		/d:sonar.login=${SONAR_TOKEN} \
		/d:projectVersion='${APP_VERSION}.${APP_BUILD}' \
		/d:sonar.pullrequest.key=${PR_NUMBER} \
		/d:sonar.pullrequest.branch=${PR_BRANCH} \ 
		/d:sonar.pullrequest.provider=vsts \
		/d:sonar.pullrequest.vsts.instanceUrl="https://assurecaresoftware.visualstudio.com/" \
		/d:sonar.pullrequest.vsts.project="MedCompassMicro" \
		/d:sonar.pullrequest.vsts.repository="time-track-dshp" \
        /d:sonar.cs.opencover.reportsPaths="/src/time-track-dshp.Tests/coverage.opencover.xml" \
		/d:sonar.cs.xunit.reportsPaths="/src/time-track-dshp.Tests/TestResults/*.xml" \
		/d:sonar.coverage.exclusions="**/Data/*.*,**/Controllers/*.*,**/Context/*.*,**/Startup.cs,**/Program.cs,**/Extensions/HealthReportExtensions.cs,**/Utils/Attributes/*.*,**/Utils/DeepCopy/*.*,**/Utils/ApplicationExceptionHandler.cs,**/Utils/AssureCareBindings.cs,**/Utils/ContextLogger.cs,**/Utils/RequestContextAccessor.cs,**/Utils/RetryHelper.cs,**/Model/Dto/MongoClientConnectionSettings.cs,**Tests*.cs" \
		/d:sonar.verbose=false; \
fi

RUN dotnet build "time-track-dshp.sln" --no-restore -c Release -o /app

WORKDIR "/src/time-track-dshp.Tests"
RUN dotnet test "time-track-dshp.Tests.csproj" --collect:"XPlat Code Coverage" --logger "xunit;LogFileName=/app/time-track-dshp-tests-results.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=\"opencover,cobertura\" /p:ExcludeByFile=\"**/Data/*.*,**/Controllers/*.*,**/Context/*.*,**/Startup.cs,**/Program.cs,**/Extensions/HealthReportExtensions.cs,**/Utils/Attributes/*.*,**/Utils/DeepCopy/*.*,**/Utils/ApplicationExceptionHandler.cs,**/Utils/AssureCareBindings.cs,**/Utils/ContextLogger.cs,**/Utils/RequestContextAccessor.cs,**/Utils/RetryHelper.cs,**/Model/Dto/MongoClientConnectionSettings.cs\" /p:SkipAutoProps=true; exit 0 

WORKDIR /src
RUN if [ $PERFORM_SONARSCAN = 1 ]; then dotnet sonarscanner end /d:sonar.login=${SONAR_TOKEN}; fi

WORKDIR "/src/time-track-dshp"

FROM build AS publish
ARG APP_VERSION
ARG APP_BUILD
RUN  echo "Buidling Version ${APP_VERSION}.${APP_BUILD}" 
RUN dotnet publish "time-track-dshp.csproj" /p:Version="${APP_VERSION}.${APP_BUILD}" /p:InformationalVersion="$APP_VERSION.$APP_BUILD" -c Release -o /app

FROM base AS final
ARG APP_VERSION
ARG APP_BUILD
WORKDIR /app
COPY --from=publish /app .
COPY ./*.sh /app/
RUN ["chmod", "+x", "/app/entrypoint.sh"]
RUN printf "${APP_VERSION}.${APP_BUILD}" > version.txt
ENTRYPOINT ["bash", "/app/entrypoint.sh"]
