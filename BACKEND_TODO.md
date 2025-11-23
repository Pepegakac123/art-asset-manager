### 11. File System Watcher

Automatyczne wykrywanie zmian.

- [ ] **Implementacja:** `FileSystemWatcher` w `ScannerService`.
- [ ] **Debouncing:** Logika opóźniająca skanowanie o X sekund po wykryciu zmiany, aby nie zabić bazy.

### 12. Duplicate Management

- [ ] **Endpoint:** `GET /api/assets/duplicates` – Znajduje assety z tym samym `FileHash`.
