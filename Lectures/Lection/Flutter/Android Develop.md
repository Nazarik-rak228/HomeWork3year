**Все лекции по Flutter в удобном Markdown-формате** (с примерами кода). Можно копировать целиком или по разделам.

---

# 1. Структура проекта Flutter

- **android/** — код и файлы для связи Dart-приложения с Android
- **ios/** — код и файлы для связи с iOS
- **lib/** — основные файлы приложения на Dart (главная рабочая папка)
- **build/** — файлы, создаваемые при сборке
- **test/** — тесты
- **web/**, **linux/**, **macos/**, **windows/** — платформенные файлы
- **pubspec.yaml** — конфигурация проекта, зависимости, пакет
- **README.md** — описание проекта

---

# 2. runApp и MaterialApp

```dart
void main() {
  runApp(MyApp());
}
```

`runApp()` — точка входа. Принимает корневой виджет.

### MaterialApp

Основной контейнер для Material Design.

| Свойство | Описание |
|----------|----------|
| `home` | Первый экран |
| `title` | Имя приложения (в диспетчере задач) |
| `theme` | Тема |
| `initialRoute` | Стартовый маршрут при навигации |
| `routes` | Карта маршрутов |
| `navigatorKey` | Ключ навигатора |
| `onGenerateRoute` | Генерация маршрутов |
| `debugShowMaterialGrid` | Сетка отладки |
| `debugShowCheckedModeBanner` | Баннер DEBUG |

Альтернативы:
- `WidgetsApp` — базовый без Material
- `CupertinoApp` — стиль iOS
- `CustomApp` — полностью кастомный

---

# 3. StatelessWidget и StatefulWidget

### StatelessWidget
Неизменяемый виджет. Зависит только от параметров конструктора и родителя. Подходит для статического контента.

### StatefulWidget
Может менять состояние. Состоит из виджета + класса `State`.

**Жизненный цикл StatefulWidget:**

1. `createState()` — создаёт объект State
2. `initState()` — один раз при создании (инициализация, подписки)
3. `didChangeDependencies()` — при изменении зависимостей
4. `didUpdateWidget()` — когда родитель передал новые параметры
5. `build()` — построение UI
6. `setState()` — уведомление об изменении → вызывает `build()`
7. `dispose()` — очистка ресурсов

**Рекомендация Flutter:** использовать `StatelessWidget` везде, где можно. `StatefulWidget` нагружает устройство сильнее.

### Пример setState (счётчик)

```dart
class CounterWidget extends StatefulWidget {
  @override
  State<CounterWidget> createState() => _CounterWidgetState();
}

class _CounterWidgetState extends State<CounterWidget> {
  int _count = 0;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text('$_count'),
        ElevatedButton(
          onPressed: () {
            setState(() {
              _count++;
            });
          },
          child: Text('Увеличить'),
        ),
      ],
    );
  }
}
```

---

# 4. Scaffold, Container и другие базовые виджеты

### Scaffold
Заполняет экран и даёт API для стандартных элементов:

| Свойство | Описание |
|----------|----------|
| `appBar` | Верхняя панель |
| `bottomNavigationBar` | Нижняя навигация |
| `floatingActionButton` | Плавающая кнопка |
| `drawer` | Боковое меню |
| `snackBar` | Временное уведомление снизу |
| `bottomSheet` | Панель снизу |

### Container

```dart
Container(
  width: double.infinity,
  height: 200,
  color: Colors.green,
  padding: EdgeInsets.all(50),
  margin: EdgeInsets.symmetric(horizontal: 10),
  decoration: BoxDecoration(
    color: Colors.pink,
    borderRadius: BorderRadius.circular(15),
    image: DecorationImage(
      image: NetworkImage('https://...'),
      fit: BoxFit.cover,
    ),
    shape: BoxShape.circle,
    boxShadow: [
      BoxShadow(
        color: Colors.black54,
        blurRadius: 5,
        spreadRadius: 5,
        offset: Offset(-5, 5),
      ),
    ],
  ),
  child: Text('Текст'),
)
```

Основные свойства: `alignment`, `child`, `color`, `constraints`, `decoration`, `foregroundDecoration`, `height`, `width`, `margin`, `padding`, `transform`.

### Center / Align / Padding

```dart
Center(child: Text('По центру'));

Align(
  alignment: Alignment.centerRight,
  child: Container(...),
);

Padding(
  padding: EdgeInsets.all(16), // или .only, .symmetric, .fromLTRB
  child: Text('С отступами'),
);
```

### Stack

```dart
Stack(
  alignment: Alignment.centerLeft,
  children: [
    Container(width: 290, height: 190, color: Colors.green),
    Container(width: 220, height: 130, color: Colors.yellow),
    Positioned(
      bottom: 10,
      right: 10,
      child: Container(width: 150, height: 90, color: Colors.red),
    ),
  ],
)
```

### Row / Column

```dart
Row(
  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
  crossAxisAlignment: CrossAxisAlignment.center,
  children: [
    ElevatedButton(onPressed: () {}, child: Text('BTN 1')),
    Icon(Icons.ac_unit, size: 64),
    // ...
  ],
);

Column(
  mainAxisAlignment: MainAxisAlignment.start,
  crossAxisAlignment: CrossAxisAlignment.stretch,
  children: [...],
);
```

`MainAxisAlignment`: `start`, `center`, `end`, `spaceBetween`, `spaceEvenly`, `spaceAround`.

---

# 5. TextEditingController и ListView

### TextEditingController

```dart
final TextEditingController _controller = TextEditingController();

// Связь с TextField
TextField(
  controller: _controller,
  decoration: InputDecoration(hintText: 'Введите текст'),
);

// Получить текст
String value = _controller.text;

// Установить текст
_controller.text = 'Новое значение';

// Слушатель
_controller.addListener(() {
  print('Текущий текст: ${_controller.text}');
});

// Обязательно в dispose
@override
void dispose() {
  _controller.dispose();
  super.dispose();
}
```

### Пример 1 — очистка и отображение

```dart
class FirstExampleWidget extends StatefulWidget {
  @override
  State<FirstExampleWidget> createState() => _FirstExampleWidgetState();
}

class _FirstExampleWidgetState extends State<FirstExampleWidget> {
  final TextEditingController _controller = TextEditingController();
  String _displayText = '';

  @override
  void initState() {
    super.initState();
    _controller.addListener(_updateDisplayText);
  }

  @override
  void dispose() {
    _controller.removeListener(_updateDisplayText);
    _controller.dispose();
    super.dispose();
  }

  void _updateDisplayText() {
    setState(() {
      _displayText = _controller.text;
    });
  }

  void _clearText() {
    _controller.clear();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        TextField(
          controller: _controller,
          decoration: InputDecoration(hintText: 'Введите текст'),
        ),
        ElevatedButton(
          onPressed: _clearText,
          child: Text('Очистить текст'),
        ),
        Text('Тут то что вы ввели: $_displayText'),
      ],
    );
  }
}
```

### ListView

**Статический:**

```dart
ListView(
  padding: EdgeInsets.all(16),
  children: [
    ListTile(title: Text('Элемент 1')),
    ListTile(title: Text('Элемент 2')),
    // ...
  ],
)
```

**Динамический (рекомендуется):**

```dart
ListView.builder(
  padding: EdgeInsets.all(16),
  itemCount: items.length,
  itemBuilder: (context, index) {
    return ListTile(title: Text(items[index]));
  },
)
```

Свойства: `scrollDirection`, `padding`, `reverse`, `physics`, `controller`.

---

# 6. GestureDetector

Невидимый контейнер, добавляющий обработку жестов.

### Основные типы жестов

1. **Тапы**
   - `onTap` — короткое касание
   - `onDoubleTap` — двойное
   - `onTapDown` / `onTapUp` / `onTapCancel`

2. **Долгое нажатие**
   - `onLongPressStart` / `onLongPressMoveUpdate` / `onLongPressEnd` / `onLongPressCancel`

3. **Свайпы / перетаскивания**
   - `onPanStart` / `onPanUpdate` / `onPanEnd` (любое направление)
   - `onHorizontalDragStart/Update/End`
   - `onVerticalDragStart/Update/End`

4. **Масштабирование и вращение**
   - `onScaleStart` / `onScaleUpdate` / `onScaleEnd`
   - В `onScaleUpdate`: `details.scale`, `details.rotation`, `details.focalPoint`, `details.localFocalPoint`, `details.focalPointDelta`

```dart
GestureDetector(
  onTap: () => print('Тап'),
  onLongPress: () => print('Долгое'),
  onPanUpdate: (details) {
    // details.delta
  },
  onScaleUpdate: (details) {
    // details.scale, details.rotation
  },
  behavior: HitTestBehavior.opaque, // увеличить область нажатия
  child: Container(...),
)
```

Специализированные виджеты:
- `InkWell` — с ripple-эффектом
- `Dismissible` — свайп для удаления
- `InteractiveViewer` — масштабирование + перемещение

---

# 7. Provider (простое управление состоянием)

Подходит для простых приложений (1–3 экрана). Для сложных — лучше Bloc/Cubit.

```bash
flutter pub add provider
```

### Пример смены темы

```dart
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

void main() {
  runApp(
    ChangeNotifierProvider(
      create: (_) => ThemeProvider(),
      child: const MyApp(),
    ),
  );
}

class ThemeProvider extends ChangeNotifier {
  bool _isDark = false;
  bool get isDark => _isDark;
  ThemeMode get themeMode => _isDark ? ThemeMode.dark : ThemeMode.light;

  void toggleTheme() {
    _isDark = !_isDark;
    notifyListeners();
  }
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    final themeProvider = context.watch<ThemeProvider>();

    return MaterialApp(
      theme: ThemeData(brightness: Brightness.light, colorSchemeSeed: Colors.blue),
      darkTheme: ThemeData(brightness: Brightness.dark, colorSchemeSeed: Colors.blue),
      themeMode: themeProvider.themeMode,
      home: const HomePage(),
    );
  }
}

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final isDark = context.watch<ThemeProvider>().isDark;

    return Scaffold(
      appBar: AppBar(title: const Text('Тема через Provider')),
      body: Center(
        child: ElevatedButton.icon(
          onPressed: () => context.read<ThemeProvider>().toggleTheme(),
          icon: Icon(isDark ? Icons.light_mode : Icons.dark_mode),
          label: Text(isDark ? 'Включить светлую тему' : 'Включить тёмную тему'),
        ),
      ),
    );
  }
}
```

- `watch` — подписаться на изменения
- `read` — получить без подписки (для вызова методов)

---

# 8. Bloc / Cubit

```bash
flutter pub add flutter_bloc
```

### Cubit (проще)

```dart
// theme_state.dart
part of 'theme_cubit.dart';

@immutable
sealed class ThemeState {}
class LightThemeState extends ThemeState {}
class DarkThemeState extends ThemeState {}

// theme_cubit.dart
import 'package:bloc/bloc.dart';
import 'package:meta/meta.dart';
part 'theme_state.dart';

class ThemeCubit extends Cubit<ThemeState> {
  ThemeCubit() : super(LightThemeState());

  void toggleTheme() {
    if (state is LightThemeState) {
      emit(DarkThemeState());
    } else {
      emit(LightThemeState());
    }
  }
}

// main.dart
BlocProvider(
  create: (_) => ThemeCubit(),
  child: BlocBuilder<ThemeCubit, ThemeState>(
    builder: (context, state) {
      return MaterialApp(
        theme: ThemeData.light(),
        darkTheme: ThemeData.dark(),
        themeMode: state is LightThemeState ? ThemeMode.light : ThemeMode.dark,
        home: ThemeSwitcherPage(),
      );
    },
  ),
);

// На кнопке
context.read<ThemeCubit>().toggleTheme();
```

### Bloc (с событиями)

```dart
// theme_event.dart
part of 'theme_bloc.dart';
@immutable
sealed class ThemeEvent {}
class ToggleThemeEvent extends ThemeEvent {}

// theme_state.dart
part of 'theme_bloc.dart';
@immutable
sealed class ThemeState {}
class LightThemeState extends ThemeState {}
class DarkThemeState extends ThemeState {}

// theme_bloc.dart
class ThemeBloc extends Bloc<ThemeEvent, ThemeState> {
  ThemeBloc() : super(LightThemeState()) {
    on<ToggleThemeEvent>(_toggleTheme);
  }

  void _toggleTheme(ToggleThemeEvent event, Emitter<ThemeState> emit) {
    if (state is LightThemeState) {
      emit(DarkThemeState());
    } else {
      emit(LightThemeState());
    }
  }
}

// На кнопке
context.read<ThemeBloc>().add(ToggleThemeEvent());
```

**Когда что использовать:**
- **Cubit** — небольшие/средние приложения, простая логика
- **Bloc** — сложная бизнес-логика, много событий, крупные проекты

---

# 9. Работа с сетью (Dio)

```bash
flutter pub add dio get_it pretty_dio_logger
```

### Регистрация (main.dart + GetIt)

```dart
final getIt = GetIt.instance;

void setup() {
  final dio = Dio(BaseOptions(
    baseUrl: 'https://api.api-ninjas.com/v1/',
    headers: {'X-Api-Key': 'ВАШ_КЛЮЧ'},
  ));

  dio.interceptors.addAll([
    InterceptorsApp(),          // свой интерсептор
    PrettyDioLogger(),
  ]);

  getIt.registerSingleton<Dio>(dio);
}
```

### Интерсептор

```dart
class InterceptorsApp extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    // добавить токен, логировать
    print('REQUEST[${options.method}] => PATH: ${options.path}');
    super.onRequest(options, handler);
  }

  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    print('RESPONSE[${response.statusCode}] => PATH: ${response.requestOptions.path}');
    super.onResponse(response, handler);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    print('ERROR[${err.response?.statusCode}] => PATH: ${err.requestOptions.path}');
    super.onError(err, handler);
  }
}
```

### Сервис

```dart
class ApiService {
  final Dio _dio = getIt<Dio>();

  Future<List<String>> fetchJokes() async {
    try {
      final response = await _dio.get('/dadjokes');
      return (response.data as List)
          .map((e) => e['joke'] as String)
          .toList();
    } catch (e) {
      // обработка ошибок
      rethrow;
    }
  }
}
```

### Отображение (ListView)

```dart
FutureBuilder<List<String>>(
  future: apiService.fetchJokes(),
  builder: (context, snapshot) {
    if (snapshot.connectionState == ConnectionState.waiting) {
      return CircularProgressIndicator();
    }
    if (snapshot.hasError) return Text('Ошибка');
    final jokes = snapshot.data!;
    return ListView.builder(
      itemCount: jokes.length,
      itemBuilder: (_, i) => ListTile(title: Text(jokes[i])),
    );
  },
)
```

---

# 10. Архитектура: Clean Architecture + MWWM

### Зачем нужна архитектура
- Легко менять UI без переписывания логики
- Легко менять источники данных
- Тестировать компоненты отдельно
- Распределять работу в команде

### Clean Architecture (3 слоя)

```
lib/
  features/
    data/
      data_sources/
      models/
      repositories/          # реализации
    domain/
      entities/
      repositories/          # интерфейсы
      usecases/
    presentation/
      pages/
      state/ (blocs/cubits)
      widgets/
  main.dart
```

- **Presentation** — UI + состояние экрана
- **Domain** — бизнес-объекты и сценарии (usecases)
- **Data** — получение/сохранение/преобразование данных

### MWWM (Model–Widget–WidgetModel)

| Компонент     | Ответственность                          | Пример |
|---------------|------------------------------------------|--------|
| **Widget**    | Построение UI + передача действий        | Список + кнопка «Обновить» |
| **WidgetModel** | Логика экрана и состояния отображения  | Загрузка / ошибка / данные |
| **Model**     | Связь с бизнес-слоем                     | Вызов usecase |

Папки ≠ архитектура. Важно разделение ответственности, а не просто наличие папок.

---

Этого достаточно, чтобы писать приложения «с нуля» по материалам лекций. Если нужно — могу вынести любой раздел в отдельный файл или добавить больше полных рабочих примеров.