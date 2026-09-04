# Aspotus Customer Web

Клиентское React-приложение Aspotus.

## Запуск

```bash
npm ci
npm run dev
```

Приложение открывается на `http://localhost:5173`. В режиме разработки Vite проксирует запросы `/api`, `/catalog`, `/orders` и `/files` на Gateway `http://localhost:5230`.

Переопределить адрес Gateway можно переменной окружения `VITE_GATEWAY_URL`.
