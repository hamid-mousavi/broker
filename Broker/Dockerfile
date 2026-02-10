# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src

# COPY Broker/Broker.csproj Broker/
# RUN dotnet restore Broker/Broker.csproj

# COPY Broker/ Broker/
# RUN dotnet publish Broker/Broker.csproj -c Release -o /app/publish

# FROM mcr.microsoft.com/dotnet/aspnet:8.0
# WORKDIR /app
# COPY --from=build /app/publish .

# ENV ASPNETCORE_URLS=http://0.0.0.0:5000
# EXPOSE 5000

# ENTRYPOINT ["dotnet", "Broker.dll"]
# =========================
# مرحله ساخت (Build Stage)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# فقط فایل پروژه رو کپی کن و restore انجام بده
COPY Broker.csproj . 
RUN dotnet restore Broker.csproj

# بقیه سورس کد رو کپی کن و publish کن
COPY . .
RUN dotnet publish Broker.csproj -c Release -o /app/publish

# =========================
# مرحله اجرا (Runtime Stage)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# کپی خروجی از مرحله build
COPY --from=build /app/publish .

# تنظیم پورت و URL
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

# اجرای اپلیکیشن
ENTRYPOINT ["dotnet", "Broker.dll"]
