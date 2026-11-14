from pathlib import Path
import random
import shutil


def split_yolo_dataset(dataset_path, train_ratio=0.8, seed=42):
    dataset_path = Path(dataset_path)

    images_dir = dataset_path / "images"
    labels_dir = dataset_path / "labels"

    print("=" * 60)
    print("ПРОВЕРКА YOLO DATASET")
    print("=" * 60)

    # Проверяем существование папок
    if not images_dir.exists():
        print(f"ОШИБКА: папка с изображениями не найдена:")
        print(f"       {images_dir}")
        return

    if not labels_dir.exists():
        print(f"ОШИБКА: папка с разметкой не найдена:")
        print(f"       {labels_dir}")
        return

    print(f"Папка изображений: {images_dir}")
    print(f"Папка разметки:     {labels_dir}")
    print()

    # ---------------------------------------------------------
    # Поиск изображений
    # ---------------------------------------------------------

    image_files = []

    for file in images_dir.iterdir():

        if not file.is_file():
            continue

        # .JPG, .jpg, .JpG и т.д. будут определяться одинаково
        extension = file.suffix.lower()

        if extension in [".jpg", ".jpeg", ".png", ".bmp", ".webp"]:
            image_files.append(file)

    print(f"Найдено изображений: {len(image_files)}")

    # Показываем расширения найденных изображений
    extensions = {}

    for file in image_files:
        ext = file.suffix
        extensions[ext] = extensions.get(ext, 0) + 1

    print("Расширения изображений:")

    for ext, count in extensions.items():
        print(f"  {ext}: {count}")

    print()

    if len(image_files) == 0:
        print("ОШИБКА: изображения не найдены!")
        print("Проверь путь к папке images.")
        print()
        print("Содержимое папки images:")

        for file in images_dir.iterdir():
            print(f"  {file.name}")

        return

    # ---------------------------------------------------------
    # Поиск TXT
    # ---------------------------------------------------------

    label_files = []

    for file in labels_dir.iterdir():

        if not file.is_file():
            continue

        # .txt, .TXT, .Txt и т.д.
        if file.suffix.lower() == ".txt":
            label_files.append(file)

    print(f"Найдено TXT-файлов: {len(label_files)}")

    # Показываем расширения TXT
    label_extensions = {}

    for file in label_files:
        ext = file.suffix
        label_extensions[ext] = label_extensions.get(ext, 0) + 1

    print("Расширения разметки:")

    for ext, count in label_extensions.items():
        print(f"  {ext}: {count}")

    print()

    if len(label_files) == 0:
        print("ВНИМАНИЕ: TXT-файлы не найдены!")
        print()
        print("Содержимое папки labels:")

        for file in labels_dir.iterdir():
            print(f"  {file.name}")

        return

    # ---------------------------------------------------------
    # Проверяем соответствие изображений и TXT
    # ---------------------------------------------------------

    # Создаём словарь:
    # имя файла -> путь
    #
    # Например:
    # DOG.TXT -> {"dog": Path("DOG.TXT")}

    labels_dict = {}

    for label in label_files:
        labels_dict[label.stem.lower()] = label

    matched_images = []
    missing_labels = []

    for img in image_files:

        image_name = img.stem.lower()

        if image_name in labels_dict:
            matched_images.append(img)
        else:
            missing_labels.append(img)

    print("=" * 60)
    print("ПРОВЕРКА СООТВЕТСТВИЯ")
    print("=" * 60)

    print(f"Всего изображений:          {len(image_files)}")
    print(f"Изображений с TXT:          {len(matched_images)}")
    print(f"Изображений без TXT:        {len(missing_labels)}")
    print()

    # Показываем изображения, для которых нет TXT
    if missing_labels:

        print("ИЗОБРАЖЕНИЯ БЕЗ РАЗМЕТКИ:")

        for img in missing_labels[:20]:
            expected_label = img.stem + ".txt"

            print(
                f"  {img.name} "
                f"-> ожидался {expected_label}"
            )

        if len(missing_labels) > 20:
            print(
                f"  ... и ещё "
                f"{len(missing_labels) - 20}"
            )

        print()

    # ---------------------------------------------------------
    # Используем только пары image + label
    # ---------------------------------------------------------

    if len(matched_images) == 0:

        print("=" * 60)
        print("ОШИБКА: НЕТ ПАР IMAGE + TXT")
        print("=" * 60)

        print()
        print("Примеры найденных изображений:")

        for img in image_files[:10]:
            print(f"  {img.name}")

        print()
        print("Примеры найденных TXT:")

        for label in label_files[:10]:
            print(f"  {label.name}")

        print()
        print("Сравниваются имена файлов без расширения.")
        print("Например:")
        print("  DOG.JPG  <->  DOG.txt")
        print("  cat.JPG  <->  cat.txt")

        return

    # ---------------------------------------------------------
    # Перемешивание
    # ---------------------------------------------------------

    random.seed(seed)
    random.shuffle(matched_images)

    split_idx = int(len(matched_images) * train_ratio)

    train_images = matched_images[:split_idx]
    val_images = matched_images[split_idx:]

    print("=" * 60)
    print("РАЗДЕЛЕНИЕ DATASET")
    print("=" * 60)

    print(f"Всего пар:     {len(matched_images)}")
    print(f"Train:         {len(train_images)}")
    print(f"Validation:    {len(val_images)}")
    print()

    # ---------------------------------------------------------
    # Создание папок
    # ---------------------------------------------------------

    folders = [
        images_dir / "train",
        images_dir / "val",
        labels_dir / "train",
        labels_dir / "val",
    ]

    for folder in folders:
        folder.mkdir(parents=True, exist_ok=True)

    # ---------------------------------------------------------
    # Функция перемещения
    # ---------------------------------------------------------

    def move_dataset(images, image_destination, label_destination):

        success = 0
        errors = 0

        for img in images:

            # Находим TXT
            label = labels_dict.get(img.stem.lower())

            if label is None:

                print(
                    f"ОШИБКА: для {img.name} "
                    f"не найден TXT-файл"
                )

                errors += 1
                continue

            try:

                # Перемещаем изображение
                shutil.move(
                    str(img),
                    str(image_destination / img.name)
                )

                # Перемещаем TXT
                shutil.move(
                    str(label),
                    str(label_destination / label.name)
                )

                success += 1

            except Exception as e:

                print()
                print("ОШИБКА ПРИ ПЕРЕМЕЩЕНИИ:")
                print(f"  Изображение: {img}")
                print(f"  Разметка:    {label}")
                print(f"  Причина:     {e}")
                print()

                errors += 1

        return success, errors

    # ---------------------------------------------------------
    # Train
    # ---------------------------------------------------------

    train_success, train_errors = move_dataset(
        train_images,
        images_dir / "train",
        labels_dir / "train"
    )

    # ---------------------------------------------------------
    # Validation
    # ---------------------------------------------------------

    val_success, val_errors = move_dataset(
        val_images,
        images_dir / "val",
        labels_dir / "val"
    )

    # ---------------------------------------------------------
    # Итог
    # ---------------------------------------------------------

    print()
    print("=" * 60)
    print("РЕЗУЛЬТАТ")
    print("=" * 60)

    print(f"Train успешно:      {train_success}")
    print(f"Train ошибок:       {train_errors}")
    print(f"Val успешно:        {val_success}")
    print(f"Val ошибок:         {val_errors}")
    print(f"Всего перемещено:   {train_success + val_success}")
    print()

    if train_errors + val_errors == 0:
        print("Dataset успешно разделён!")

    else:
        print("ВНИМАНИЕ: некоторые файлы не были перемещены.")
        print("Причины ошибок указаны выше.")


# =============================================================
# ЗАПУСК
# =============================================================

split_yolo_dataset(
    "dataset",
    train_ratio=0.8,
    seed=42
)
