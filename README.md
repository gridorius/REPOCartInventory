# 📦 Enhanced Gameplay Mod / Мод Расширенного Геймплея

## 🌟 Description / Описание

**EN:**  
This mod improves and expands gameplay by adding a customizable HUD, scaling systems, an internet shop interface, and
detailed control over enemy spawns and item interactions. It introduces a “Careful Mode” for progressive difficulty and
balances immersion with powerful configuration options.

**RU:**  
Этот мод улучшает и расширяет игровой процесс, добавляя настраиваемый HUD, системы масштабирования, интерфейс
интернет-магазина и подробный контроль над спавном врагов и взаимодействием с предметами. Вводит “Осторожный режим” для
постепенного усложнения и предлагает гибкую систему настроек.

---

# ⚠️ Осторожно | ⚠️ Warning

**[RU]**  
Мод находится на стадии разработки и в многопользовательской игре работает нормально только у хоста.  
Если у нескольких пользователей установлен этот мод — возможны баги.

**[EN]**  
This mod is under development and currently only works properly for the host in multiplayer.  
If multiple players have the mod installed, bugs may occur.

| ![Preview](https://raw.githubusercontent.com/gridorius/REPOCartInventory/master/gifs/interface.gif) Интерфейс                | ![Preview](https://raw.githubusercontent.com/gridorius/REPOCartInventory/master/gifs/dollars_bag.gif)Конвертация предметов           | ![Preview](https://raw.githubusercontent.com/gridorius/REPOCartInventory/master/gifs/save_in_truck%20.gif) Сохранение денег в грузовике |
|------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------|
| ![new_shop.gif](https://raw.githubusercontent.com/gridorius/REPOCartInventory/master/gifs/new_shop.gif)     Интернет магазин | ![Preview](https://raw.githubusercontent.com/gridorius/REPOCartInventory/master/gifs/extruct_truck.gif)Извлечение денег из грузовика |                                                                                                                                         |

## ⚙️ Configuration / Конфигурация

> All options are configurable via BepInEx config files.  
> Все параметры можно настроить через конфигурационные файлы BepInEx.

---

### 🛒 Cart / Тележка

| Key (EN)                  | RU Описание                           | Default |
|---------------------------|---------------------------------------|---------|
| Enable valuable convert   | Включить конвертацию ценных предметов | true    |
| Cart vacuum cleaner scale | Масштаб пылесоса тележки (0.5–10.0)   | 0.6     |

---

### 🛍 Internet Shop / Интернет-магазин

| Key (EN)                  | RU Описание                       | Default |
|---------------------------|-----------------------------------|---------|
| Enable new shop interface | Включить новый интерфейс магазина | true    |
| Skip shop level           | Пропуск уровня магазина           | true    |
| Shop open button          | Клавиша открытия магазина         | "l"     |

---

### 🧾 HUD / HUD-интерфейс

| Key (EN)                             | RU Описание                                 | Default |
|--------------------------------------|---------------------------------------------|---------|
| Show hud                             | Показывать HUD                              | true    |
| Hud position x                       | Положение по X                              | -240    |
| Hud position y                       | Положение по Y                              | 30      |
| Show level                           | Показывать уровень                          | true    |
| Show time                            | Показывать время                            | true    |
| Show saved dollars                   | Показывать сохранённые доллары              | true    |
| Show collected valuables             | Показывать собранные ценности               | true    |
| Show collected valuables percent     | Показывать % собранных ценностей            | true    |
| Show level dollars                   | Показывать доллары за уровень               | true    |
| Show level dollars collected percent | Показывать % собранных долларов             | true    |
| Show lost dollars                    | Показывать потерянные доллары               | true    |
| Show amount of explored modules      | Показывать количество исследованных модулей | true    |
| Show immortal enemies                | Показывать бессмертных врагов               | true    |
| Show kills                           | Показывать количество убийств               | true    |
| Show carts info                      | Показывать информацию о тележках            | true    |

---

### 🚚 Truck / Грузовик

| Key (EN)                        | RU Описание                                     | Default |
|---------------------------------|-------------------------------------------------|---------|
| Enable save items in truck      | Сохранять предметы в грузовике                  | true    |
| Button to extract truck dollars | Клавиша извлечения денег из грузовика в тележку | Slash   |
| Button to split grabbed bag     | Клавиша разделения захваченного мешка           | Period  |

---

### 🔒 Careful Mode / Осторожный режим

| Key (EN)                                     | RU Описание                                                         | Default |
|----------------------------------------------|---------------------------------------------------------------------|---------|
| Enable                                       | Включить режим                                                      | true    |
| Register orb damage                          | Учитывать урон от сфер                                              | false   |
| Chance skip spawn, damage saved to next iter | Шанс пропустить спавн, урон переносится на следующую волну (0–100%) | 40      |
| Damage to spawn enemy tier 3                 | Урон для появления врага 3 уровня                                   | 5000    |
| Damage to spawn enemy tier 2                 | Урон для появления врага 2 уровня                                   | 3000    |
| Damage to spawn enemy tier 1                 | Урон для появления врага 1 уровня                                   | 2000    |

---

### 📈 Scaling / Масштабирование

| Key (EN)                                 | RU Описание                                             | Default |
|------------------------------------------|---------------------------------------------------------|---------|
| Enable module amount scaling             | Включить масштабирование количества модулей             | true    |
| Enable valuable amount and price scaling | Включить масштабирование количества/стоимости ценностей | true    |
| Enable enemy amount scaling              | Включить масштабирование количества врагов              | true    |
| Enable immortal enemies                  | Включить бессмертных врагов                             | true    |
| Immortal spawn chance                    | Шанс появления бессмертных врагов (0–100%)              | 30      |
| Enemy scaling skip levels                | Пропуск уровней до масштабирования врагов               | 1       |
| Enemy tier 3 amount multiplier           | Множитель количества врагов 3 уровня (1.0–10.0)         | 1.2     |
| Enemy tier 2 amount multiplier           | Множитель количества врагов 2 уровня (1.0–10.0)         | 1.4     |
| Enemy tier 1 amount multiplier           | Множитель количества врагов 1 уровня (1.0–10.0)         | 1.8     |
| Enable extraction amount scaling         | Включить масштабирование извлекаемых предметов          | true    |

---

## 💬 Feedback / Обратная связь

Feel free to open issues or pull requests.  
Открывайте issue или присылайте предложения через pull request.

