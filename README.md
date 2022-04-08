!!! Przeczytaj tresc tego dokumentu zanim uruchomisz Nelderim.exe !!!

1. Utworz plik Scripts/Ustawienia.cs kopiując Scripts\Ustawienia.cs.example
1a. Wszelkie podstawowe konfiguracje shardu znajduja sie w Scripts\Ustawienia.cs.
2. Jesli w "public static bool Shard_Local = " mam ustawione "true;" wszelkie konfiguracje ktore korzystaja z tej zmiennej kozystaja z 1 parametru, np:
"public static int ShardPort = ( Shard_Local ? 2493 : 2593 );", w tym przypadku port shardu bedzie = 2493, w przypadku gdy zmienimy w Shard_Local na false, portem bedzie 2593.
-- public static bool Shard_Local = true; 
3. W "public static string CustomPath" nalezy wprowadzic poprawna sciezke katalogu do plikow z mapa.
-- public static string CustomPath = Shard_Local ? @"I:\ULTIMA_WORKSHOP\RunUO36\Map\" : @"I:\ULTIMA_WORKSHOP\RunUO36\Map\";
4. W "public static readonly string Address" nalezy wprowadzic adres ip komputera gdzie znajduje sie serwer RunUO, moze byc 127.0.0.1
5. Reszta parametrow w zaleznosci od potrzeb.
6. Nie nalezy wlaczac boota IRC.
7. Przy zmienianiu czegokolwiek w plikach w katalogu 'source' nalezy uruchomic w tymze katalogu 'compile - nelderim.bat' i korzystac z nowo w ten sposob utworzonego pliku uruchamiania serwera //Loki