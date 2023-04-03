# Testing
Zuerst den Ordner `databases`erstellen dann `dotnet ef database update` im Projektordner ausführen, um die Datenbank zu erstellen.
Dann, das Projekt einfach starten, dann wird der Swagger-UI gelauncht, zum API testen.

# Database Scheme
## `Users`
(Der `PasswordHash` sollte auch bereits Clientseitig einmal mit Bcrypt gehasht werden. (Auf Zero-Knoweledge basis und so...))
| Uuid | Username | PasswordHash |
| - |

## `PasswordEntries`
(Alles bis auf die `UserUuid` wird natürlich Clientseitig verschlüsselt.)
| UserUuid | Name | Link | Username | Password |
| - |
