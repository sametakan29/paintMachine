# 🤖 paintMachine — Automated Paint Mixing & Tank Management System

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)
![C# WinForms](https://img.shields.io/badge/C%23-WinForms-239120?style=flat-square&logo=c-sharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=flat-square&logo=postgresql)
![Arduino](https://img.shields.io/badge/Arduino-UNO-00979D?style=flat-square&logo=arduino)
[![Related Management System](https://img.shields.io/badge/Management-paint--machine--maintenance-green?style=flat-square&logo=github)](https://github.com/sametakan29/paint-machine-maintenance)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

> **[TR]** Türkçe açıklama için [aşağıya kaydırın](#-türkçe).  
> **[EN]** Scroll down for the [English documentation](#-english).

---

<a name="-türkçe"></a>
## 🇹🇷 Türkçe

Bu proje, Arduino UNO ve Servo motorlar (COM7) kullanarak gerçek zamanlı mililitre hassasiyetinde boya dozajlama, renk reçeteleri (Kırmızı, Sarı, Turuncu) hazırlama ve tank seviyelerini PostgreSQL altyapısıyla takip etmek için geliştirilmiş **C# Windows Forms (.NET 10.0) + IoT Donanım Entegrasyonu** uygulamasıdır.

Sistem renk reçetelerine göre hassas boya miktarlarını dozajlar, fiziksel servo motorlarla Seri Port (COM7) üzerinden haberleşir, tank seviyelerini taşma korumasıyla (`Math.Clamp`) anlık takip eder ve tüm dozajlama işlemlerini loglar.

### 🔗 İlgili Projeler (Eko-Sistem)

- 🏢 **[paint-machine-maintenance (Yönetim & Bakım Otomasyonu)](https://github.com/sametakan29/paint-machine-maintenance):** Bu projenin çoklu şube yönetimi, personel rolleri, makine parçaları (karıştırıcı motor, pompa, filtre) bakım periyotları ve geçmiş sipariş raporlama boyutudur.

### 🌟 Temel Özellikler

* **🎨 Reçete Yönetimi & Önizleme:** PostgreSQL'den renk reçetelerini yükler ve UI üzerinde anlık HEX renk önizlemesi sağlar.
* **🤖 Donanım Entegrasyonu (Arduino & Servolar):** Servo vanaları kontrol etmek için Seri Port (COM7 @ 9600 Baud) üzerinden milisaniye komutları iletir.
* **🛢️ Gerçek Zamanlı Stok & Tank Dolumu:** 1000ml kapasiteli boya tanklarını izler, dozajlamada seviyeleri otomatik düşer ve taşma korumalı manuel dolumu destekler.
* **📊 Geçmiş Loglama:** Tüm dozajlama işlemlerini (Zaman damgası, Renk adı, Servo miktarları) PostgreSQL'e kaydeder ve `DataGridView` üzerinde sunar.
* **💡 UI LED Simülasyonu & Güvenli Mod:** Donanım gecikmeleriyle senkronize animasyonlu LED zamanlayıcıları içerir, Arduino bağlı değilse "Test Modu"na geçer.

### 🔌 Donanım Kurulumu ve Seri Protokol

* **Mikrodenetleyici:** Arduino UNO
* **Aktüatörler:** 2 x SG90 / MG996R Servo Motorlar (Kırmızı Tank: Dijital Pin 9, Sarı Tank: Dijital Pin 10)
* **Güç Kaynağı:** Harici 5V/2A Güç Kaynağı / Powerbank (Arduino ile Ortak GND)

<img width="900" height="1600" alt="WhatsApp Image 2026-08-11 at 09 47 46" src="https://github.com/user-attachments/assets/771f30e7-0a45-424a-8279-1b5b5295c119" />
<img width="900" height="1600" alt="image" src="https://github.com/user-attachments/assets/3fd7a7d4-195f-44c2-95ad-df1b22eeab5a" />

---

<a name="-english"></a>
## 🇬🇧 English

An automated paint mixing, dispensing, and real-time inventory tracking system built with **.NET 10.0 Windows Forms**, **PostgreSQL**, and **Arduino UNO**.

The system dispenses precise paint volumes according to color recipes (Red, Yellow, Orange), communicates with physical servo motors via Serial Port (COM7), tracks tank levels in real time with overflow protection, and logs all dispensing activities.

### 🔗 Related Projects (Ecosystem)

* 🏢 **[paint-machine-maintenance (Management & Maintenance System)](https://github.com/sametakan29/paint-machine-maintenance):** The enterprise management counterpart of this hardware system. Features multi-store management, staff role assignments, machine component maintenance scheduling (mixer motor, pump, filter), and historical analytics.

### 🌟 Key Features

* **🎨 Recipe Management & Preview:** Loads color recipes from PostgreSQL and provides instant HEX color preview in the UI.
* **🤖 Hardware Integration (Arduino & Servos):** Transmits pulse duration commands over Serial Port (COM7 @ 9600 Baud) to control servo valves.
* **🛢️ Real-time Inventory & Tank Refill:** Monitors 1000ml capacity paint tanks, updates levels automatically on dispense, and supports manual refilling with overflow protection (`Math.Clamp`).
* **📊 Historical Logging:** Records all dispensing operations (Timestamp, Color Name, Servo Amounts) in PostgreSQL and presents them in a `DataGridView`.
* **💡 UI LED Simulation & Fail-Safe Mode:** Features animated LED timers synchronized with hardware delays, and falls back to "Test Mode" if Arduino is disconnected.
* **🛡️ Secure Data Access:** Utilizes parameterized SQL queries (`NpgsqlCommand.Parameters`) to prevent SQL Injection.

### 🏗️ System Architecture

The project follows a **Layered Architecture** pattern separating Presentation, Business Logic, Data Access, and Hardware Integration.

```
+-------------------------------------------------------------+
|                      PRESENTATION LAYER                     |
|                         (Form1.cs)                          |
+------------------------------+------------------------------+
                               |
            +------------------+------------------+
            |                                     |
+-----------v-----------------+     +-------------v-----------+
|      BUSINESS LOGIC         |     |    HARDWARE INTEGRATION |
|     (TankManager.cs)        |     |     (SerialService.cs)  |
+-----------+-----------------+     +-------------+-----------+
            |                                     |
+-----------v-----------------+                   |
|     DATA ACCESS LAYER       |                   |
|       (DbHelper.cs)         |                   |
+-----------+-----------------+                   |
            |                                     |
+-----------v-----------------+     +-------------v-----------+
|    POSTGRESQL DATABASE      |     |    ARDUINO / SERVOS     |
|         (PaintDb)           |     |     (COM7 / Pin 9-10)   |
+-----------------------------+     +-------------------------+
```

### 🔌 Hardware Setup & Serial Protocol

#### Hardware Components
* **Microcontroller:** Arduino UNO
* **Actuators:** 2 x SG90 / MG996R Servo Motors
  * **Servo 1 (Red Tank):** Digital Pin 9
  * **Servo 2 (Yellow Tank):** Digital Pin 10
* **Power Supply:** External 5V/2A Powerbank / Power Supply (Common GND with Arduino)

#### Servo Movement Logic
* **Step Angle (`PRESS_ANGLE`):** 15° per dispense action.
* **Max Angle (`MAX_ANGLE`):** Resets to 0° (`BASE_ANGLE`) after reaching 75°.
* **Inter-servo Delay:** 200 ms gap between servos to prevent power surges / brownouts.

#### Serial Protocol Format
Data packets are sent as ASCII string lines terminated with `\n`:

`"[Servo1_Ms],[Servo2_Ms]\n"`

* **Red (10ml Servo 1):** `"1000,0\n"`
* **Yellow (10ml Servo 2):** `"0,1000\n"`
* **Orange (10ml Both Servos):** `"1000,1000\n"`

### 🗄️ Database Schema (PostgreSQL)

```sql
-- Color Recipes
CREATE TABLE ColorRecipes (
    id SERIAL PRIMARY KEY,
    colorname VARCHAR(50) NOT NULL,
    hexcode VARCHAR(10) NOT NULL,
    servo1ml DECIMAL(5,2) NOT NULL,
    servo2ml DECIMAL(5,2) NOT NULL
);

-- Tank Levels (Single row tracking)
CREATE TABLE tanks (
    id INT PRIMARY KEY DEFAULT 1,
    servo1_ml DECIMAL(6,2) NOT NULL,
    servo2_ml DECIMAL(6,2) NOT NULL
);

-- Dispensing History Logs
CREATE TABLE PaintLogs (
    id SERIAL PRIMARY KEY,
    recipeid INT REFERENCES ColorRecipes(id),
    colorname VARCHAR(50) NOT NULL,
    servo1usedml DECIMAL(5,2) NOT NULL,
    servo2usedml DECIMAL(5,2) NOT NULL,
    createddate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### 🚀 Getting Started

#### Prerequisites
* **.NET 10.0 SDK** (or .NET 8.0+)
* **PostgreSQL Server 15+**
* **Arduino IDE** (for uploading the `.ino` sketch)

#### Database Setup
1. Create a database named `PaintDb`.
2. Execute the schema SQL script above to create `ColorRecipes`, `tanks`, and `PaintLogs`.
3. Insert default recipes (Red, Yellow, Orange) and initial tank levels (1000ml each).

#### Running the Application
```bash
# Clone the repository
git clone https://github.com/sametakan29/paintMachine.git

# Navigate into the project folder
cd paintMachine

# Build and run
dotnet build
dotnet run
```

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).
