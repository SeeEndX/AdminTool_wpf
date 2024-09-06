УСТАНОВКА И ИСПОЛЬЗОВАНИЕ:
1) Установить MySQL https://dev.mysql.com/downloads/file/?id=531675

2) Создать подключение в MySQL Server

3) Выполнить команды в CMD:
```
MySQL -u ПОЛЬЗОВАТЕЛЬ_ПОДКЛЮЧЕНИЯ -pПАРОЛЬ -e "CREATE DATABASE admintooldb"
```
```
mysql -u ПОЛЬЗОВАТЕЛЬ_ПОДКЛЮЧЕНИЯ -pПАРОЛЬ admintooldb < "Путь\до\файла\DumpDB.sql"
```

4) Запустить скрипт CMD:
```
.\путь\до\addAdmin.bat ЛОГИН_АДМИНА ПАРОЛЬ_АДМИНА
```

5) Запустить установщик AdminTool.msi и установить приложения.

6) Выполнить команду в CMD: 
```
sc create AdminService binPath="ПАПКА УСТАНОВКИ\AdminService.exe" DisplayName= "ИМЯ СЛУЖБЫ" start=auto
```

7) Запустить AdminTool_WPF.

8) Ввести данные админа.

9) Добавить пользователей системы (веб-разработчиков) и назначить необходимые права.
