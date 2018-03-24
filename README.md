# EncodingDetector
Encoding Detector est une application console pour Windows 7/8/10. Elle fonctionne avec le .Net Framework 4.0. Cette application permet la détection de l'encodage d'un fichier texte (ASCII, ANSI, UTF8, UTF16) ainsi que du caractère de fin de ligne (style UNIX ou DOS). Elle peut également effectuer un ré-encodage vers un fichier cible.

## Librairies tierces
Encoding Detector utilise deux références tierces pour fonctionner :

- Pour la détection de l'encodage, la librairie text-encoding-detect est utilisée.
	- Licence Apache 2.0
	- URL : [https://github.com/AutoItConsulting/text-encoding-detect](https://github.com/AutoItConsulting/text-encoding-detect)
	- Branche master utilisée pour la référence dans VisualStudio (Latest commit f8d9b7a on 28 Jul 2016)
- Pour la gestion des options d'entrée de l'application, et diverses méthodes utiles, la librairie AryxDevLibrary est utilisée.
	- URL :  [https://code.dacendi.net/Aryx/AryxDevLibraryCsharp](https://code.dacendi.net/Aryx/AryxDevLibraryCsharp)
	- Version 1.0.1.659 - [Page NuGet](https://www.nuget.org/packages/AryxDevLibrary/1.0.1.659 "Page NuGet")

## Fonctionnement
Encoding Detector fonctionne comme une application console, c'est-à-dire via un shell Windows (invite de commandes ou PowerShell).

### Syntaxe
```
DetectEncoding.exe -f fichier 
[-c UTF8_BOM|UTF8_NOBOM|ANSI|UTF16LE|UTF16BE|UTF16LE_NOBOM|UTF16BE_NOBOM|ASCII] [-e DOS|UNIX] [-o cibleConversion]
```
### Détails
#### Fichier à traiter

**Obligatoire**

**Option :** -f ou --file

**Description :** Indique le chemin du fichier pour lequel il faut déterminer l'encodage et le caractère de fin de ligne.

### Conversion de l'encodage

**Option :** -c ou --convert-to

**Description :** Spécifie l'encodage cible désiré.

**Choix :**
UTF8_BOM, UTF8_NOBOM, ANSI, UTF16LE, UTF16BE, UTF16LE_NOBOM, UTF16BE_NOBOM, ASCII

### Conversion du caractère de fin de ligne

**Option :** -e ou --end-of-line-to

**Description :** Spécifie le caractère de fin ligne cible désiré.

**Choix :**
DOS : pour un caractère de fin de ligne \r\n. Utilisé dans les produits Microsoft (DOS, Windows)

UNIX : pour un caractère de fin de ligne  \n.

### Nom du fichier converti

**Option :** -o ou --output-file

**Description :** Nom désiré du fichier converti. 
Si une des options -e ou -c est utilisée, et que -o est omis, le fichier en sortie aura le même nom que le fichier en entrée, suffixé avec "Out".
Si la valeur "SAME_AS_INPUT" est indiquée pour -o, alors le nom du fichier en sortie sera le même que celui en entrée : le fichier en entrée sera remplacé par sa conversion.

Attention : aucune sauvegarde du fichier en entrée n'est effectuée.

### Niveau de silence

**Option :** -s ou --silence-level

**Description :** Permet de régler le nombre d'éléments affichés dans la console. Selon 3 niveaux :

* 0 : Affiche le texte de présentation de l'application, ainsi que les lignes de traitement. Niveau par défaut.
* 1 : Affiche uniquement les lignes de traitement.
* 2 : N'affiche rien.

## Conversion

La conversion de l'encodage et du caractère de fin de ligne ne s'effectue que lorsque l'encodage du fichier à traiter à réussi. Si pour une raison ou une autre l'encodage du fichier source n'est pas détecté, la conversion ne sera pas effectuée.

Si l'option "o" n'est pas indiqué, le fichier converti sera sous la forme :

```
NomDuFichierSource-Out.ExtensionDuFichierSource
```

## Exemples

### Exemple 1
Commande : 
```
DetectEncoding.exe -f .\fileTest.sql
```

Sortie écran :

    Encoding Detector with reencoding !
    ===================================
    by Aryx - Wolfaryx informatique - 2018
    
    Encoding detection based on work from AutoIt Consulting :
    https://github.com/AutoItConsulting/text-encoding-detect
    
    InputFile: C:\Users\ARyx\fileTest.sql
    Encoding: Ansi; Unix

Ici, l'encodage du fichier fileTest.sql est détecté comme de l'Ansi.
Son caractère de fin de ligne est "\n", de type Unix.

### Exemple 2
Commande : 
```
DetectEncoding.exe -f .\fileTest.sql -c UTF8_BOM -e DOS
```

Sortie écran :

    Encoding Detector with reencoding !
    ===================================
    by Aryx - Wolfaryx informatique - 2018
    
    
    Encoding detection based on work from AutoIt Consulting :
    https://github.com/AutoItConsulting/text-encoding-detect
    
    InputFile: C:\Users\ARyx\fileTest.sql
    Encoding: Ansi; Unix
    Output : Encoding: UTF8_BOM; DOS

Ici, l'encodage du fichier fileTest.sql est détecté comme étant de l'ANSI.
Son caractère de fin de ligne est "\n", de type Unix.
Le fichier de sorti sera encodé en UTF8 avec BOM, et avec un caractère de fin de ligne "\r\n".

### Exemple 3
Commande : 
```
DetectEncoding.exe -f .\fileTest.sql -c UTF8_BOM -e DOS -s 1
```

Sortie écran :

    InputFile: C:\Users\ARyx\fileTest.sql
    Encoding: Ansi; Unix
    Output : Encoding: UTF8_BOM; DOS

Ici, le résultat de cet exemple est similaire à celui de l'exemple 2. Seul change, l'ajout de l'option s 1. Le texte de présentation de l'application n'est plus affiché.