FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

ENV \
    # Connection string
    CONNECTION_STRING="" \
    # Packages version
    PACKAGES_VERSION=1.0.0-build03

# VOLUME [ "/data" ]


# Download Packages binaries
RUN wget https://packages.czompi.cloud/products/czsoft/packages/$PACKAGES_VERSION.tar.gz -O packages.tar.gz \
    && tar -ozxf packages.tar.gz -C /bin \
    && rm packages.tar.gz
WORKDIR /bin
EXPOSE 80
ENTRYPOINT ["dotnet", "Packages.dll", "--connectionString", "$CONNECTION_STRING"]