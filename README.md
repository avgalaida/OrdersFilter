# OrdersFilter


**OrdersFilter** — консольное приложение на C#, предназначенное фильтрации заказов сервиса доставки.

## Установка

 ```bash
 git clone https://github.com/avgalaida/OrdersFilter.git
 cd OrdersFilter/OrdersFilter
 dotnet restore
 dotnet build
 ```

## Использование
 ```
dotnet run --project OrdersFilter.csproj 1 "2024-10-22 14:30:00"
 ```

## Данные:
 ```
input/orders.txt - заказы в формате НомерЗаказа,Вес,РайонГорода,ВремяДоставки.
output/result.txt — отфильтрованные заказы.
output/log.txt — журнал операций.
 ```


