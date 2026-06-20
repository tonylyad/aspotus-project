# План работ: Admin Frontend (Aspotus.Admin.Frontend)

## 1. Базовый каркас
- Добавить `react-router-dom` — роутинг для страниц админки
- Настроить Vite proxy в `vite.config.js` для разработки (`/api` → `http://localhost:5230`), чтобы не было CORS-проблем
- Создать Layout — сайдбар + хедер + контентная область
- Добавить UI-кит (или писать на чистом CSS). Варианты: использовать простой CSS-фреймворк (Pico, Water.css), либо базовые компоненты самописные

## 2. Аутентификация
- Страница логина — форма email/password → `POST /api/auth/login`
- Страница регистрации (опционально)
- JWT-менеджмент — сохранять токен в `localStorage`, прикреплять `Authorization: Bearer <token>` ко всем запросам
- Auth-контекст — `AuthProvider` + `useAuth()` хук с информацией о текущем пользователе и ролях
- ProtectedRoute — компонент-обёртка, проверяющая авторизацию и роль (Admin)

## 3. Управление пользователями (только Admin)
- Страница списка пользователей — `GET /api/users`, таблица с email/name/roles
- Создание пользователя — форма → `POST /api/users`
- Назначение ролей — `POST /api/users/{id}/roles`

## 4. CRUD-страницы для Catalog API
Для каждой из 7 сущностей — одинаковый паттерн:
- Страница списка (таблица с поиском/фильтрацией)
- Страница создания (форма)
- Страница редактирования (форма, предзаполненная данными)
- Удаление (кнопка с подтверждением)

Сущности:
1. **Brands** (`/catalog/api/brands`) — Name
2. **Models** (`/catalog/api/models`) — Name, BrandId (dropdown)
3. **Generations** (`/catalog/api/generations`) — Name, YearFrom, YearTo, ModelId (dropdown)
4. **Cars** (`/catalog/api/cars`) — BrandId → ModelId → GenerationId каскадные dropdown, Year, Mileage, BodyType и т.д.
5. **Categories** (`/catalog/api/categories`) — Name, ParentCategoryId
6. **Manufacturers** (`/catalog/api/manufacturers`) — Name
7. **Parts** (`/catalog/api/parts`) — Name, Article, Price, StockQuantity, CategoryId, ManufacturerId, ConditionType и т.д.

## 5. HTTP-клиент (API-слой)
- **`api/client.js`** — обёртка над `fetch` с:
  - Базовым URL (из Vite proxy — `/api`, в production — полный URL Gateway)
  - Автоматическим прикреплением JWT
  - Обработкой ошибок (401 → redirect на логин, 403 → «нет прав», 409/400 → показ ошибки)
- **Сервисы-модули** (`api/auth.js`, `api/brands.js`, `api/models.js` и т.д.) — функции для каждого endpoint

## 6. Архитектура компонентов
```
src/
├── api/              # HTTP-клиент и сервисы
│   ├── client.js
│   ├── auth.js
│   ├── brands.js
│   ├── models.js
│   ├── generations.js
│   ├── cars.js
│   ├── categories.js
│   ├── manufacturers.js
│   ├── parts.js
│   └── users.js
├── components/       # Переиспользуемые компоненты
│   ├── Layout.jsx (Sidebar + Header)
│   ├── ProtectedRoute.jsx
│   ├── LoadingSpinner.jsx
│   ├── ConfirmDialog.jsx
│   ├── DataTable.jsx
│   └── FormField.jsx
├── contexts/
│   └── AuthContext.jsx
├── pages/
│   ├── Login.jsx
│   ├── Dashboard.jsx
│   ├── users/ (List, Create, Edit)
│   ├── brands/ (List, Create, Edit)
│   ├── models/ (List, Create, Edit)
│   ├── generations/ (List, Create, Edit)
│   ├── cars/ (List, Create, Edit)
│   ├── categories/ (List, Create, Edit)
│   ├── manufacturers/ (List, Create, Edit)
│   └── parts/ (List, Create, Edit)
├── App.jsx           # Роутер
├── App.css           # Глобальные стили админки
└── main.jsx          # Точка входа
```

## 7. Маршруты
```
/login                → Login
/                     → Dashboard (защищён)
/users                → Users list (Admin)
/users/new            → Create user (Admin)
/users/:id/edit       → Edit user (Admin)
/brands               → Brands list
...аналогично для всех сущностей
```

## 8. Порядок реализации (рекомендуемый)
1. Vite proxy, `react-router-dom`, Layout, базовые стили
2. API-клиент, Auth-контекст, логин, ProtectedRoute
3. Dashboard (пустая заглушка)
4. CRUD для Brand (самая простая сущность — один Name)
5. CRUD для остальных сущностей Catalog API
6. Управление пользователями
