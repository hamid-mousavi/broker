# سیستم مدیریت ترخیص کالای گمرکی

یک پلتفرم آنلاین جامع برای ارتباط بین ترخیص‌کاران گمرک و صاحبان کالا

## تکنولوژی‌های استفاده شده

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- PostgreSQL
- JWT Authentication
- AutoMapper
- Swagger/OpenAPI

## ویژگی‌ها

1. ثبت‌نام و احراز هویت
2. مدیریت پروفایل حرفه‌ای برای ترخیص‌کاران
3. سیستم جستجو و فیلتر پیشرفته
4. سیستم ارتباطی و پیام‌رسانی امن
5. مدیریت درخواست‌ها و قرار ملاقات‌ها
6. سیستم امتیازدهی و اعتبارسنجی

## راه‌اندازی

1. نصب PostgreSQL و ایجاد دیتابیس
2. تنظیم ConnectionString در `appsettings.json`
3. اجرای دستورات migration
4. اجرای پروژه

## ساختار پروژه

```
Broker/
├── Controllers/      # API Controllers
├── Services/         # Business Logic Services
├── Data/            # DbContext and Data Access
├── Models/          # Entity Models
├── DTOs/            # Data Transfer Objects
├── Helpers/         # Helper Classes
└── Mappings/        # AutoMapper Profiles
```

