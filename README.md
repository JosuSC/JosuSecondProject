# Skyrim Gwent: Dovahkiin Edition 🐉

![Unity](https://img.shields.io/badge/Unity-2022+-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-Level%2099-blueviolet?style=for-the-badge&logo=csharp)
![MATCOM](https://img.shields.io/badge/MATCOM-2024-blue?style=for-the-badge)

Welcome to **Skyrim Gwent**, a digital card game project developed for the Programming course at **MATCOM, University of Havana**. This repository contains a fully functional implementation of a card game inspired by the mechanics of Gwent, expanded with a custom Domain-Specific Language (DSL) and a mini-compiler, all set in the frozen lands of *The Elder Scrolls V: Skyrim*.

---

## 📖 Project Overview

The project was developed in two critical phases that integrate game development with programming language theory:

### Phase 1: Gwent-pro (Core Game)
Development of the main engine using Unity. Implementation of core rules:
* Three-row battlefield (Melee, Ranged, Siege).
* Card types: Silver, Gold (Heroes), and Leaders.
* Round-based scoring system and turn management.

### Phase 2: GWENT++ (Compiler & DSL)
The advanced phase where the game expands. A **mini-compiler** was created to interpret and load new cards and effects dynamically via an in-game text editor.

---

## ❄️ Theme: The Elder Scrolls V: Skyrim

While the mechanics are based on Gwent, the soul of this version is dedicated to Skyrim. Every element has been personalized to reflect the lore of the Dragonborn:

* **Factions:** Choose your side in the civil war (**Stormcloaks vs. Imperials**) or play as the ancient **Dragons**.
* **Cards & Characters:** Iconic units such as the Dovahkiin, Alduin, Serana, and Cicero.
* **Thu'um (Dragon Shouts):** Special abilities and `OnActivation` effects are themed as shouts (e.g., *Unrelenting Force* for clearing rows or *Dragonrend* to ground powerful units).
* **Visuals:** UI inspired by the Skyrim HUD and backgrounds featuring legendary locations like Whiterun, Riften, and Blackreach.

---

## 🛠️ Technical Features

### Core Mechanics
- **Board Layout:** Two players with 3 distinct rows, including slots for weather and boost cards.
- **Game Loop:** 10-card initial draw, hand limits, and best-of-three rounds victory system.
- **Card Types:** Units (Silver/Gold), Leaders with unique abilities, Decoys, Clear Weather, and Weather effects.

### The Gwent++ DSL & Compiler
The heart of Phase 2 is the custom compiler that processes the language designed for card creation:
- **Effect Declaration:** Define complex behaviors using `Params` and `Action`.
- **Advanced Selectors:** Precise card filtering from the `Board`, `Hand`, `Deck`, or `Graveyard` using `Source`, `Single`, and `Predicate`.
- **Context Management:** Real-time access to the game state (`TriggerPlayer`, `Power`, `Owner`, `Faction`).
- **Post-Actions:** Chain effects together to create powerful combos.

---

## 🧪 Testing & Validation

A comprehensive test suite is included to ensure the compilation pipeline works correctly:
1.  **Lexical Analysis:** Correct tokenization of keywords and symbols.
2.  **Syntax Analysis:** Validation of card and effect structures.
3.  **Semantic Analysis:** Verification of references to declared parameters and context properties.

---

## 🚀 Installation & Usage

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/JosuSC/SkyrimGwent-Interprete.git](https://github.com/JosuSC/SkyrimGwent-Interprete.git)
    ```
2.  **Open in Unity:** Import the project using Unity 2022.3 LTS or higher.
3.  **Run:** Launch the `MainScene`.
4.  **DSL Editor:** Use the in-game editor to load your own `.gwent` files and expand your deck.

---

> *"I used to be an adventurer like you, then I took an arrow in the knee."*
