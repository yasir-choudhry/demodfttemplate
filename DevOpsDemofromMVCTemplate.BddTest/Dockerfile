# Using the dotnet core sdk as our base image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
COPY . /app
WORKDIR /app/$safeprojectname$
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/wait_for_it.sh \
    && dotnet restore
#RUN curl https://dl.google.com/cloudsql/cloud_sql_proxy.linux.amd64 --create-dirs -o /app/RVSS.Web.BddTest/cloud_sql_proxy
#RUN chmod +x /app/RVSS.Web.BddTest/cloud_sql_proxy
#RUN chmod +x /app/RunSqlProxy.sh
#ENTRYPOINT ["sh","RunSqlProxy.sh"]
