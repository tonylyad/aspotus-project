$ErrorActionPreference = 'Stop'

$msoTextOrientationHorizontal = 1
$msoShapeRectangle = 1
$msoShapeRoundedRectangle = 5
$msoShapeOval = 9
$msoConnectorStraight = 1
$ppLayoutBlank = 12
$ppSaveAsOpenXMLPresentation = 24
$ppSaveAsPDF = 32

function Color([string]$hex) {
    $value = $hex.TrimStart('#')
    $r = [Convert]::ToInt32($value.Substring(0, 2), 16)
    $g = [Convert]::ToInt32($value.Substring(2, 2), 16)
    $b = [Convert]::ToInt32($value.Substring(4, 2), 16)
    return $r + ($g * 256) + ($b * 65536)
}

$C = @{
    Bg = Color '#081018'
    Surface = Color '#101C27'
    Surface2 = Color '#172735'
    Text = Color '#F6F8FA'
    Muted = Color '#9FB0BF'
    Blue = Color '#4DA3FF'
    Cyan = Color '#37D5D5'
    Green = Color '#55D68B'
    Yellow = Color '#FFC857'
    Red = Color '#FF6B6B'
    Purple = Color '#A78BFA'
    Line = Color '#294052'
    Black = Color '#000000'
}

function Add-Text($slide, [string]$text, [double]$x, [double]$y, [double]$w, [double]$h,
    [double]$size = 18, [int]$color = $C.Text, [bool]$bold = $false, [int]$align = 1,
    [string]$font = 'Aptos') {
    $shape = $slide.Shapes.AddTextbox($msoTextOrientationHorizontal, $x, $y, $w, $h)
    $shape.TextFrame.MarginLeft = 0
    $shape.TextFrame.MarginRight = 0
    $shape.TextFrame.MarginTop = 0
    $shape.TextFrame.MarginBottom = 0
    $shape.TextFrame.WordWrap = -1
    $shape.TextFrame.TextRange.Text = $text
    $shape.TextFrame.TextRange.Font.Name = $font
    $shape.TextFrame.TextRange.Font.Size = $size
    $shape.TextFrame.TextRange.Font.Color.RGB = $color
    $shape.TextFrame.TextRange.Font.Bold = $(if ($bold) { -1 } else { 0 })
    $shape.TextFrame.TextRange.ParagraphFormat.Alignment = $align
    return $shape
}

function Add-Box($slide, [string]$title, [string]$body, [double]$x, [double]$y, [double]$w, [double]$h,
    [int]$accent = $C.Blue, [int]$fill = $C.Surface, [double]$titleSize = 18, [double]$bodySize = 12) {
    $box = $slide.Shapes.AddShape($msoShapeRoundedRectangle, $x, $y, $w, $h)
    $box.Fill.ForeColor.RGB = $fill
    $box.Line.ForeColor.RGB = $accent
    $box.Line.Weight = 1.5
    Add-Text $slide $title ($x + 14) ($y + 12) ($w - 28) 25 $titleSize $accent $true | Out-Null
    if ($body) {
        $body = $body.Replace('`n', "`n")
        Add-Text $slide $body ($x + 14) ($y + 43) ($w - 28) ($h - 52) $bodySize $C.Muted $false | Out-Null
    }
    return $box
}

function Add-Pill($slide, [string]$text, [double]$x, [double]$y, [double]$w, [int]$fill, [int]$textColor = $C.Bg) {
    $shape = $slide.Shapes.AddShape($msoShapeRoundedRectangle, $x, $y, $w, 28)
    $shape.Fill.ForeColor.RGB = $fill
    $shape.Line.Visible = 0
    Add-Text $slide $text $x ($y + 5) $w 18 10 $textColor $true 2 | Out-Null
}

function Add-Arrow($slide, [double]$x1, [double]$y1, [double]$x2, [double]$y2, [int]$color = $C.Line, [double]$weight = 2) {
    $line = $slide.Shapes.AddConnector($msoConnectorStraight, $x1, $y1, $x2, $y2)
    $line.Line.ForeColor.RGB = $color
    $line.Line.Weight = $weight
    $line.Line.EndArrowheadStyle = 3
    return $line
}

function Add-Title($slide, [string]$title, [string]$kicker = 'ASPOTUS · АРХИТЕКТУРА') {
    Add-Text $slide $kicker 42 22 500 18 9 $C.Cyan $true | Out-Null
    Add-Text $slide $title 42 48 875 48 28 $C.Text $true | Out-Null
    $line = $slide.Shapes.AddShape($msoShapeRectangle, 42, 101, 70, 4)
    $line.Fill.ForeColor.RGB = $C.Cyan
    $line.Line.Visible = 0
}

function Add-Footer($slide, [int]$number) {
    Add-Text $slide 'aspotus-project' 42 515 180 14 8 $C.Muted | Out-Null
    Add-Text $slide ([string]$number) 885 513 30 15 9 $C.Muted $true 3 | Out-Null
}

function Add-Bullets($slide, [string[]]$items, [double]$x, [double]$y, [double]$w, [double]$h,
    [double]$size = 16, [int]$color = $C.Text, [double]$gap = 10) {
    $itemHeight = ($h - (($items.Count - 1) * $gap)) / $items.Count
    for ($i = 0; $i -lt $items.Count; $i++) {
        Add-Text $slide ("•  " + $items[$i]) $x ($y + $i * ($itemHeight + $gap)) $w $itemHeight $size $color | Out-Null
    }
}

function New-Slide($presentation, [string]$title) {
    $slide = $presentation.Slides.Add($presentation.Slides.Count + 1, $ppLayoutBlank)
    $slide.FollowMasterBackground = 0
    $slide.Background.Fill.Solid()
    $slide.Background.Fill.ForeColor.RGB = $C.Bg
    Add-Title $slide $title
    Add-Footer $slide $slide.SlideIndex
    return $slide
}

$powerPoint = $null
$presentation = $null
$scriptDirectory = if ([string]::IsNullOrWhiteSpace($PSScriptRoot)) {
    Join-Path (Get-Location) 'docs\presentation'
} else {
    $PSScriptRoot
}
$outputDirectory = Split-Path -Parent $scriptDirectory
$pptxPath = Join-Path $outputDirectory 'Aspotus-Architecture.pptx'
$pdfPath = Join-Path $outputDirectory 'Aspotus-Architecture.pdf'

try {
    $powerPoint = New-Object -ComObject PowerPoint.Application
    $powerPoint.Visible = -1
    $presentation = $powerPoint.Presentations.Add()
    $presentation.PageSetup.SlideWidth = 960
    $presentation.PageSetup.SlideHeight = 540

    # 1 — Title
    $s = $presentation.Slides.Add(1, $ppLayoutBlank)
    $s.FollowMasterBackground = 0
    $s.Background.Fill.Solid()
    $s.Background.Fill.ForeColor.RGB = $C.Bg
    $orb = $s.Shapes.AddShape($msoShapeOval, 665, -90, 390, 390)
    $orb.Fill.ForeColor.RGB = $C.Blue
    $orb.Fill.Transparency = 0.72
    $orb.Line.Visible = 0
    Add-Pill $s '.NET 10 · REACT · DOCKER' 55 58 210 $C.Cyan
    Add-Text $s 'ASPOTUS' 55 130 610 80 48 $C.Text $true | Out-Null
    Add-Text $s 'Архитектура и технологическое устройство системы' 58 220 670 82 25 $C.Muted | Out-Null
    Add-Text $s 'Каталог автомобилей и запчастей · Заказы · Резервирование · Уведомления' 58 326 760 42 15 $C.Text | Out-Null
    Add-Text $s 'Команда: Антон Лядов · Максим Сафронов · Алексей Силаев · Дмитрий Шадчнев · Елена Карлштейн' 58 463 820 28 10 $C.Muted | Out-Null
    Add-Footer $s 1

    # 2 — Users
    $s = New-Slide $presentation 'Что решает система и для кого'
    $roles = @(
        @('ПОКУПАТЕЛЬ', 'Каталог, поиск, карточка товара, корзина, оформление и история заказов', $C.Blue),
        @('МОДЕРАТОР', 'Управление брендами, моделями, поколениями, автомобилями и запчастями', $C.Cyan),
        @('ОПЕРАТОР', 'Работа с реальными заказами и перевод по допустимым статусам', $C.Yellow),
        @('АДМИНИСТРАТОР', 'Пользователи, роли, каталог, заказы и общие показатели', $C.Purple)
    )
    for ($i = 0; $i -lt 4; $i++) {
        $x = 42 + (($i % 2) * 450)
        $y = 132 + ([math]::Floor($i / 2) * 172)
        Add-Box $s $roles[$i][0] $roles[$i][1] $x $y 420 140 $roles[$i][2] | Out-Null
    }

    # 3 — Glossary
    $s = New-Slide $presentation 'Как читать архитектурные схемы'
    Add-Text $s 'Короткий словарь: что означает каждый технический термин и зачем он нужен системе' 42 113 875 22 14 $C.Muted | Out-Null
    $terms = @(
        @('API GATEWAY', 'Единая «входная дверь». Проверяет токен и роль, затем отправляет запрос нужному сервису.', $C.Cyan),
        @('СЕРВИС / API', 'Отдельный модуль для одной бизнес-области: каталог, заказы, файлы или уведомления.', $C.Blue),
        @('БАЗА ДАННЫХ', 'Собственная долговременная память сервиса. Общей базы между сервисами нет.', $C.Green),
        @('КЭШ / REDIS', 'Быстрая копия часто читаемых данных. Ускоряет ответы, но не является источником истины.', $C.Red),
        @('БРОКЕР / RABBITMQ', 'Очередь доставки событий. Получатель может обработать сообщение позже и повторить попытку.', $C.Yellow),
        @('WORKER', 'Фоновый процесс без интерфейса: читает очередь, выполняет работу и защищается от дублей.', $C.Purple)
    )
    for ($i = 0; $i -lt $terms.Count; $i++) {
        $x = 42 + (($i % 3) * 294)
        $y = 150 + ([math]::Floor($i / 3) * 170)
        Add-Box $s $terms[$i][0] $terms[$i][1] $x $y 265 140 $terms[$i][2] $C.Surface 15 11 | Out-Null
    }

    # 4 — System map
    $s = New-Slide $presentation 'Карта системы: от браузера до данных'
    Add-Box $s 'Customer Web' 'Витрина покупателя`nКаталог · корзина · заказы' 35 145 155 90 $C.Blue $C.Surface 15 9 | Out-Null
    Add-Box $s 'Admin Web' 'Рабочее место команды`nКонтент · пользователи · заказы' 35 285 155 90 $C.Purple $C.Surface 15 9 | Out-Null
    Add-Box $s 'API Gateway' 'Единая точка входа`nПроверяет JWT и роли`nНаправляет запрос' 245 207 165 112 $C.Cyan $C.Surface 15 9 | Out-Null
    Add-Box $s 'Catalog API' 'Хранит товары и цены`nПроверяет наличие`nСоздаёт резервы' 468 115 170 105 $C.Green $C.Surface 15 9 | Out-Null
    Add-Box $s 'Orders API' 'Фиксирует заказ`nМеняет статусы`nПишет событие в Outbox' 468 240 170 105 $C.Yellow $C.Surface 15 9 | Out-Null
    Add-Box $s 'Files API' 'Принимает фотографии`nХранит их вне базы каталога' 468 370 170 90 $C.Blue $C.Surface 15 9 | Out-Null
    Add-Box $s 'SQLite + Redis' 'Основные данные +`nбыстрая копия брендов' 720 115 180 90 $C.Green $C.Surface 15 9 | Out-Null
    Add-Box $s 'SQLite / RabbitMQ' 'Заказ хранится в БД;`nсобытие уходит в очередь' 720 240 180 90 $C.Yellow $C.Surface 14 9 | Out-Null
    Add-Box $s 'Notifications' 'Фоновая обработка`nбез повторных уведомлений' 720 365 180 90 $C.Purple $C.Surface 15 9 | Out-Null
    Add-Arrow $s 190 186 245 240 $C.Blue | Out-Null
    Add-Arrow $s 190 326 245 270 $C.Purple | Out-Null
    Add-Arrow $s 410 240 468 166 $C.Green | Out-Null
    Add-Arrow $s 410 260 468 290 $C.Yellow | Out-Null
    Add-Arrow $s 410 280 468 410 $C.Blue | Out-Null
    Add-Arrow $s 638 166 720 156 $C.Green | Out-Null
    Add-Arrow $s 638 290 720 280 $C.Yellow | Out-Null
    Add-Arrow $s 638 305 720 406 $C.Purple | Out-Null

    # 5 — Containers
    $s = New-Slide $presentation 'Контейнерная топология Docker Compose'
    Add-Text $s '9 контейнеров · одна backend-сеть · постоянные данные вне жизненного цикла контейнеров' 42 118 875 26 15 $C.Muted | Out-Null
    $containers = @(
        @('customer-web', 'Сайт покупателя · порт 5173', $C.Blue), @('admin-web', 'Панель команды · порт 5174', $C.Purple), @('gateway', 'Общий вход · порт 5230', $C.Cyan),
        @('catalog-api', 'Товары и резервы · порт 5299', $C.Green), @('orders-api', 'Заказы и статусы · порт 5115', $C.Yellow), @('files-api', 'Фотографии · порт 5133', $C.Blue),
        @('notifications', 'Фоновая обработка событий', $C.Purple), @('redis', 'Ускоряющий кэш · порт 6379', $C.Red), @('rabbitmq', 'Очередь · порты 5672 / 15672', $C.Yellow)
    )
    for ($i = 0; $i -lt $containers.Count; $i++) {
        $x = 42 + (($i % 3) * 294)
        $y = 165 + ([math]::Floor($i / 3) * 102)
        Add-Box $s $containers[$i][0] $containers[$i][1] $x $y 265 75 $containers[$i][2] $C.Surface 16 10 | Out-Null
    }
    Add-Text $s 'Volumes: gateway.db · catalog.db · orders.db · notifications.db · RabbitMQ data' 42 478 875 22 12 $C.Muted 0 2 | Out-Null

    # 6 — Security
    $s = New-Slide $presentation 'Gateway, аутентификация и авторизация'
    Add-Box $s '1 · ВХОД' 'Identity проверяет логин и пароль, затем получает роли пользователя.' 48 150 190 115 $C.Blue $C.Surface 15 9 | Out-Null
    Add-Box $s '2 · JWT' 'Подписанный пропуск. Браузер прикладывает его к каждому защищённому запросу.' 280 150 190 115 $C.Cyan $C.Surface 15 9 | Out-Null
    Add-Box $s '3 · RBAC' 'Роль определяет, какие действия доступны покупателю, модератору, оператору или админу.' 512 150 190 115 $C.Yellow $C.Surface 15 9 | Out-Null
    Add-Box $s '4 · YARP' 'По префиксу URL выбирает нужный API и удаляет служебную часть пути.' 744 150 170 115 $C.Green $C.Surface 15 9 | Out-Null
    Add-Arrow $s 238 200 280 200 $C.Blue | Out-Null
    Add-Arrow $s 470 200 512 200 $C.Cyan | Out-Null
    Add-Arrow $s 702 200 744 200 $C.Yellow | Out-Null
    Add-Bullets $s @(
        'Gateway централизованно применяет правила доступа к proxied endpoints.',
        'В downstream передаются X-User-Id, X-User-Email и X-User-Roles.',
        'Служебный вызов Orders → Catalog защищён X-Internal-Api-Key.',
        'Изменение каталога разрешено ContentModerator/Admin; статусов заказов — Operator/Admin.'
    ) 70 315 820 145 14 $C.Text 7

    # 7 — Catalog
    $s = New-Slide $presentation 'Catalog API: владелец товарных данных'
    $layers = @(
        @('Controllers', 'Принимают HTTP-запросы, проверяют их форму и возвращают коды ответа.', $C.Blue),
        @('Services', 'Применяют бизнес-правила: нормализацию, валидацию и проверку наличия.', $C.Cyan),
        @('Repositories', 'Изолируют EF-запросы и детали чтения или записи данных.', $C.Green),
        @('EF Core + SQLite', 'Сохраняют товары, изображения, остатки и резервы в catalog.db.', $C.Yellow)
    )
    for ($i = 0; $i -lt 4; $i++) {
        Add-Box $s $layers[$i][0] $layers[$i][1] 55 (130 + $i * 84) 405 68 $layers[$i][2] $C.Surface 16 10 | Out-Null
        if ($i -lt 3) { Add-Arrow $s 258 (195 + $i * 84) 258 (212 + $i * 84) $C.Line | Out-Null }
    }
    Add-Box $s 'Redis cache' 'Кэш списка и отдельных брендов`nTTL по умолчанию: 10 минут`nПри сбое — работа напрямую с SQLite' 535 142 360 120 $C.Red | Out-Null
    Add-Box $s 'Files API' 'Загрузка изображений в S3-совместимое хранилище`nКаталог хранит file key, URL и порядок' 535 290 360 105 $C.Blue | Out-Null
    Add-Pill $s 'Источник истины: цена + наличие + резерв' 568 430 294 $C.Green

    # 8 — Checkout sequence
    $s = New-Slide $presentation 'Оформление заказа: синхронный путь'
    $actors = @('Customer Web', 'Gateway', 'Orders API', 'Catalog API', 'SQLite')
    $colors = @($C.Blue, $C.Cyan, $C.Yellow, $C.Green, $C.Purple)
    for ($i = 0; $i -lt $actors.Count; $i++) {
        $x = 36 + $i * 185
        Add-Pill $s $actors[$i] $x 125 145 $colors[$i]
        $line = $s.Shapes.AddLine(($x + 72), 158, ($x + 72), 465)
        $line.Line.ForeColor.RGB = $C.Line
        $line.Line.DashStyle = 4
    }
    $steps = @(
        @(108, 200, 293, 200, '1  POST /orders', $C.Blue),
        @(293, 240, 478, 240, '2  route + identity', $C.Cyan),
        @(478, 285, 663, 285, '3  reserve(orderId)', $C.Yellow),
        @(663, 325, 848, 325, '4  price + snapshot', $C.Green),
        @(663, 360, 478, 360, '5  trusted data', $C.Green),
        @(478, 405, 848, 405, '6  order + outbox', $C.Yellow)
    )
    foreach ($step in $steps) {
        Add-Arrow $s $step[0] $step[1] $step[2] $step[3] $step[5] | Out-Null
        Add-Text $s $step[4] ([math]::Min($step[0], $step[2]) + 12) ($step[1] - 19) 160 16 9 $C.Muted | Out-Null
    }
    Add-Text $s 'Цена из браузера не считается доверенной: заказ получает стоимость из Catalog API.' 80 478 800 22 13 $C.Text $true 2 | Out-Null

    # 9 — State machine
    $s = New-Slide $presentation 'Заказ и резервирование: единая модель состояния'
    Add-Box $s 'Created' 'Резерв создан' 55 190 170 90 $C.Blue | Out-Null
    Add-Box $s 'Processing' 'Оператор работает' 305 190 170 90 $C.Yellow | Out-Null
    Add-Box $s 'Completed' 'Продажа завершена' 555 120 180 90 $C.Green | Out-Null
    Add-Box $s 'Cancelled' 'Резерв освобождён' 555 300 180 90 $C.Red | Out-Null
    Add-Arrow $s 225 235 305 235 $C.Blue 3 | Out-Null
    Add-Arrow $s 475 220 555 165 $C.Green 3 | Out-Null
    Add-Arrow $s 475 255 555 345 $C.Red 3 | Out-Null
    Add-Arrow $s 225 255 555 345 $C.Red 2 | Out-Null
    Add-Box $s 'АВТОМОБИЛЬ' 'Один активный резерв.`nПосле Completed скрыт от покупателя, но доступен модератору.' 770 120 155 130 $C.Cyan | Out-Null
    Add-Box $s 'ЗАПЧАСТИ' 'Свободный остаток =`nсклад − резервы' 770 300 155 95 $C.Purple | Out-Null
    Add-Text $s 'Reserve / Release / Complete идемпотентны по orderId' 245 450 470 28 14 $C.Muted $true 2 | Out-Null

    # 10 — Async
    $s = New-Slide $presentation 'Асинхронные уведомления: Outbox + Inbox'
    Add-Box $s 'Orders API' 'Создаёт заказ и событие OrderCreated.' 45 185 150 95 $C.Yellow $C.Surface 15 9 | Out-Null
    Add-Box $s 'Orders DB' 'Заказ и событие сохраняются вместе: либо обе записи, либо ни одной.' 245 170 190 125 $C.Purple $C.Surface 15 9 | Out-Null
    Add-Box $s 'Outbox Publisher' 'Находит неопубликованные события, повторяет отправку и отмечает успех.' 485 170 190 125 $C.Cyan $C.Surface 15 9 | Out-Null
    Add-Box $s 'RabbitMQ' 'Надёжно держит событие, пока получатель не сможет его обработать.' 725 170 180 125 $C.Yellow $C.Surface 15 9 | Out-Null
    Add-Arrow $s 195 232 245 232 $C.Yellow | Out-Null
    Add-Arrow $s 435 232 485 232 $C.Purple | Out-Null
    Add-Arrow $s 675 232 725 232 $C.Cyan | Out-Null
    Add-Box $s 'Notifications Worker' 'Читает очередь и создаёт уведомление в фоне, независимо от HTTP-запроса.' 545 345 220 105 $C.Green $C.Surface 15 9 | Out-Null
    Add-Box $s 'Inbox DB' 'Помнит eventId: повторно доставленное событие игнорируется.' 795 345 130 105 $C.Purple $C.Surface 14 8 | Out-Null
    Add-Arrow $s 815 290 680 350 $C.Yellow | Out-Null
    Add-Arrow $s 765 397 795 397 $C.Green | Out-Null
    Add-Text $s 'Результат: at-least-once доставка без повторной бизнес-обработки.' 90 475 780 22 14 $C.Text $true 2 | Out-Null

    # 11 — Data ownership
    $s = New-Slide $presentation 'Данные разделены по владельцам'
    $dbs = @(
        @('gateway.db', 'Кто пользователь и что ему разрешено: Identity, Users, Roles.', $C.Cyan),
        @('catalog.db', 'Что продаём, сколько доступно и что зарезервировано.', $C.Green),
        @('orders.db', 'Что заказал клиент, по какой цене и в каком статусе.', $C.Yellow),
        @('notifications.db', 'Какие события уже обработаны и не должны повторяться.', $C.Purple)
    )
    for ($i = 0; $i -lt 4; $i++) {
        $x = 42 + $i * 225
        Add-Box $s $dbs[$i][0] $dbs[$i][1] $x 155 195 145 $dbs[$i][2] $C.Surface 16 11 | Out-Null
    }
    Add-Pill $s 'НЕТ ОБЩЕЙ БАЗЫ ДАННЫХ' 327 345 305 $C.Red $C.Text
    Add-Bullets $s @(
        'Сервисы общаются через HTTP-контракты и события, а не через таблицы соседа.',
        'Заказ хранит снимок названия и цены — история не меняется вместе с каталогом.',
        'SQLite-файлы смонтированы в host volumes и переживают пересоздание контейнеров.'
    ) 105 400 750 84 12 $C.Muted 5

    # 12 — Frontends
    $s = New-Slide $presentation 'Два интерфейса — разные рабочие сценарии'
    Add-Box $s 'CUSTOMER WEB' 'React 19 · Vite 8 · React Bootstrap`n`nКаталог и поиск`nКарточка и фотогалерея`nКорзина со счётчиком`nCheckout и профиль заказов' 70 140 365 285 $C.Blue $C.Surface 22 15 | Out-Null
    Add-Box $s 'ADMIN WEB' 'React 19 · Vite 8 · Material UI`n`nРолевое меню`nCRUD каталога и изображений`nОператорская панель на реальных данных`nСтатусы заказов' 525 140 365 285 $C.Purple $C.Surface 22 15 | Out-Null
    Add-Text $s 'Общий принцип: API через Gateway · JWT · lazy routes · адаптивный UI' 130 460 700 24 13 $C.Cyan $true 2 | Out-Null

    # 13 — Reliability
    $s = New-Slide $presentation 'Надёжность и эксплуатационные свойства'
    $features = @(
        @('Автомиграции', 'При старте сервис приводит схему своей БД к ожидаемой версии.', $C.Green),
        @('Компенсация', 'Если заказ не сохранился, созданный резерв освобождается автоматически.', $C.Red),
        @('Идемпотентность', 'Повтор Reserve / Release / Complete не портит состояние и не дублирует результат.', $C.Cyan),
        @('Cache fallback', 'Если Redis недоступен, Catalog читает SQLite: медленнее, но продолжает работать.', $C.Red),
        @('Единые ошибки', 'Middleware превращает доменные исключения в понятные HTTP-коды для клиентов.', $C.Yellow),
        @('Диагностика', 'Swagger показывает контракты, логи объясняют сбои, health checks проверяют зависимости.', $C.Purple)
    )
    for ($i = 0; $i -lt 6; $i++) {
        $x = 42 + (($i % 3) * 294)
        $y = 135 + ([math]::Floor($i / 3) * 165)
        Add-Box $s $features[$i][0] $features[$i][1] $x $y 265 130 $features[$i][2] $C.Surface 16 10 | Out-Null
    }

    # 14 — Testing
    $s = New-Slide $presentation 'Тестовая стратегия'
    Add-Box $s '157' 'Backend tests`nxUnit · Moq`nAwesomeAssertions' 65 145 220 170 $C.Green $C.Surface 34 15 | Out-Null
    Add-Box $s '56' 'Customer Web tests`nVitest`nTesting Library' 370 145 220 170 $C.Blue $C.Surface 34 15 | Out-Null
    Add-Box $s '50' 'Admin Web tests`nVitest`nTesting Library' 675 145 220 170 $C.Purple $C.Surface 34 15 | Out-Null
    Add-Arrow $s 165 365 795 365 $C.Line 4 | Out-Null
    Add-Pill $s 'UNIT / CONTROLLER' 65 410 220 $C.Green
    Add-Pill $s 'COMPONENT / INTEGRATION' 370 410 220 $C.Blue
    Add-Pill $s 'PLAYWRIGHT E2E + DOCKER' 675 410 220 $C.Purple
    Add-Text $s 'E2E проверяет доступность, защиту маршрутов, вход администратора и полный цикл резерва автомобиля.' 90 465 780 32 12 $C.Muted 0 2 | Out-Null

    # 15 — Stack
    $s = New-Slide $presentation 'Технологический стек'
    $stack = @(
        @('BACKEND', '.NET 10 и ASP.NET Core реализуют бизнес-API; EF Core работает с SQLite.', $C.Green),
        @('EDGE & SECURITY', 'YARP направляет запросы; Identity, JWT и RBAC защищают операции.', $C.Cyan),
        @('DATA & MESSAGING', 'Redis ускоряет чтение; RabbitMQ и Outbox / Inbox доставляют события.', $C.Yellow),
        @('FRONTEND', 'React 19 + Vite 8; Material UI для admin, React Bootstrap для customer.', $C.Blue),
        @('QUALITY', 'xUnit и Vitest проверяют код; Playwright проходит реальные сценарии в браузере.', $C.Purple),
        @('DELIVERY', 'Dockerfiles собирают образы; Compose запускает систему и подключает volumes.', $C.Red)
    )
    for ($i = 0; $i -lt 6; $i++) {
        $x = 42 + (($i % 3) * 294)
        $y = 133 + ([math]::Floor($i / 3) * 177)
        Add-Box $s $stack[$i][0] $stack[$i][1] $x $y 265 145 $stack[$i][2] $C.Surface 16 11 | Out-Null
    }

    # 16 — Growth
    $s = New-Slide $presentation 'Как развивать архитектуру дальше'
    $now = @(
        'Изолированные сервисы и базы',
        'Gateway + RBAC',
        'Надёжные события Outbox / Inbox',
        'Docker Compose и автотесты'
    )
    $next = @(
        'Secrets manager и ротация ключей',
        'PostgreSQL и production migrations',
        'OpenTelemetry: traces, metrics, logs',
        'Retry / circuit breaker / RabbitMQ DLQ',
        'CI/CD, API versioning, container orchestration'
    )
    Add-Box $s 'УЖЕ ЕСТЬ' '' 55 135 375 310 $C.Green | Out-Null
    Add-Bullets $s $now 82 190 320 205 15 $C.Text 12
    Add-Box $s 'СЛЕДУЮЩИЙ УРОВЕНЬ' '' 530 135 375 310 $C.Cyan | Out-Null
    Add-Bullets $s $next 557 185 320 225 14 $C.Text 8
    Add-Arrow $s 445 290 515 290 $C.Cyan 4 | Out-Null

    # 17 — Final
    $s = $presentation.Slides.Add($presentation.Slides.Count + 1, $ppLayoutBlank)
    $s.FollowMasterBackground = 0
    $s.Background.Fill.Solid()
    $s.Background.Fill.ForeColor.RGB = $C.Bg
    $orb = $s.Shapes.AddShape($msoShapeOval, -120, 260, 420, 420)
    $orb.Fill.ForeColor.RGB = $C.Cyan
    $orb.Fill.Transparency = 0.80
    $orb.Line.Visible = 0
    Add-Pill $s 'ИТОГ' 55 70 90 $C.Green
    Add-Text $s 'Aspotus — система с ясными границами ответственности' 55 135 810 78 32 $C.Text $true | Out-Null
    Add-Text $s 'Раздельные данные · контролируемый доступ · доверенные цены · резервирование · надёжные события · автоматические проверки' 58 245 790 78 19 $C.Muted | Out-Null
    Add-Box $s 'Главная архитектурная идея' 'Каждый сервис владеет своей областью и данными, а согласованность достигается явными контрактами и идемпотентными процессами.' 505 365 390 105 $C.Cyan | Out-Null
    Add-Text $s 'Вопросы?' 58 438 300 44 24 $C.Text $true | Out-Null
    Add-Footer $s 17

    if (Test-Path -LiteralPath $pptxPath) { Remove-Item -LiteralPath $pptxPath -Force }
    if (Test-Path -LiteralPath $pdfPath) { Remove-Item -LiteralPath $pdfPath -Force }
    $presentation.SaveAs($pptxPath, $ppSaveAsOpenXMLPresentation)
    $presentation.SaveAs($pdfPath, $ppSaveAsPDF)
}
finally {
    if ($presentation) { $presentation.Close() }
    if ($powerPoint) { $powerPoint.Quit() }
    if ($presentation) { [void][Runtime.InteropServices.Marshal]::ReleaseComObject($presentation) }
    if ($powerPoint) { [void][Runtime.InteropServices.Marshal]::ReleaseComObject($powerPoint) }
    [GC]::Collect()
    [GC]::WaitForPendingFinalizers()
}

Write-Output $pptxPath
Write-Output $pdfPath
