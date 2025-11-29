# 🚀 Frontend Features - Backlog

## 1. Saved Searches (Smart Collections) 🧠
**Cel:** Pozwolenie użytkownikowi na zapisywanie aktualnych filtrów jako "Kolekcji".
**Priorytet:** High (Killer Feature)

- [ ] **UI:** Dodać przycisk "Save Search" (ikonka dyskietki/bookmark) w nagłówku sekcji Filtrów (obok "Filters").
- [ ] **Modal:** Po kliknięciu modal z inputem na nazwę wyszukiwania (np. "Czerwone Modele 3D").
- [ ] **API Integration:**
    - `POST /api/saved-searches` -> Wysyła obecny obiekt `filters` (JSON).
    - `GET /api/saved-searches` -> Pobiera listę do wyświetlenia w Sidebarze.
- [ ] **Sidebar:** Nowa sekcja "Saved Searches" nad lub pod "Collections". Kliknięcie ładuje filtry do Store.

## 2. Top Toolbar & Chips Sync 🔍
**Cel:** Synchronizacja paska wyszukiwania i filtrów, lepszy feedback wizualny.
**Priorytet:** Medium (UX Polish)

- [ ] **Chips (Tagi) na górze:**
    - Wyświetlanie aktywnych filtrów (np. "Rating: 4+", "Color: #F00") jako usuwalnych "Chipsów" pod Top Toolbarem.
    - Kliknięcie 'X' na chipsie usuwa konkretny filtr ze Store.
- [ ] **Search Bar behavior:**
    - Wpisanie tekstu w SearchBar powinno albo resetować inne filtry, albo działać addytywnie (decyzja UX).
- [ ] **Clear All:** Przycisk "Clear All" widoczny, gdy cokolwiek jest pofiltrowane.
