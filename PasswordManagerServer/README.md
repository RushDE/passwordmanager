# Server Start
Falls es noch nicht installiert ist `dotnet tool install --global dotnet-ef` ausführen.
Zuerst den Ordner `databases`erstellen dann `dotnet ef database update` im Projektordner ausführen, um die Datenbank zu erstellen.
Dann, das Projekt in VisualStudio einfach starten, dann wird der Swagger-UI gelauncht, zum API testen.

# API Testen
Zuerst unter `/Register` einen account erstellen.
Statt den `string`s einfach was eigenes reinschreiben,
Dann unter `/Login` die `string`s durch die gleichen daten wie bei `/Register` ersetzen.
Nun den Token kopieren und oben bei `Authorize`hineinkopieren und absenden.
Dann kannst du alle Endpunkte verwenden die ein Schloss daneben haben.

Wenn du in die Datenbank schauen willst, kann ich dafür `DB Browser (SQLite)` dafür empfehlen.

# Database Scheme
## `Users`
(Der `PasswordHash` sollte auch bereits Clientseitig einmal mit sha512 mit dem Usernamen als Salz gehasht werden. (Auf Zero-Knoweledge basis und so...))
| Uuid | Username | PasswordHash |
| - | - | - |

## `PasswordEntries`
(Alles bis auf die `UserUuid` wird natürlich Clientseitig verschlüsselt.)
| UserUuid | Name | Link | Username | Password |
| - | - | - | - | - |
