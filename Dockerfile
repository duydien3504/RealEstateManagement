# ==============================================================================
# GIAI ĐOẠN 1: BUILD (SDK Image)
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

# Sao chép các file cấu hình project (.csproj) trước để tối ưu hóa bộ nhớ cache của Docker
COPY ["RealEstateSystem.Api/RealEstateSystem.Api.csproj", "RealEstateSystem.Api/"]
COPY ["RealEstateSystem.Application/RealEstateSystem.Application.csproj", "RealEstateSystem.Application/"]
COPY ["RealEstateSystem.Domain/RealEstateSystem.Domain.csproj", "RealEstateSystem.Domain/"]
COPY ["RealEstateSystem.Infrastructure/RealEstateSystem.Infrastructure.csproj", "RealEstateSystem.Infrastructure/"]

# Restore toàn bộ NuGet package của ứng dụng
RUN dotnet restore "RealEstateSystem.Api/RealEstateSystem.Api.csproj"

# Sao chép toàn bộ mã nguồn còn lại của dự án vào Docker
COPY . .

# Biên dịch ứng dụng ra thư mục /app/publish ở chế độ Release
WORKDIR "/src/RealEstateSystem.Api"
RUN dotnet publish "RealEstateSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==============================================================================
# GIAI ĐOẠN 2: RUNTIME (Runtime Image siêu nhẹ)
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy sản phẩm đã build hoàn thiện từ Giai đoạn 1 sang Giai đoạn 2
COPY --from=build-env /app/publish .

# Mở cổng 8080 cho ứng dụng .NET 8 Web API
EXPOSE 8080

# Cài đặt lệnh khởi động ứng dụng Web API
ENTRYPOINT ["dotnet", "RealEstateSystem.Api.dll"]
