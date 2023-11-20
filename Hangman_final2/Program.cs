using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Wähle eine Farbe:");
        Console.WriteLine("1. Rot");
        Console.WriteLine("2. Grün");
        Console.WriteLine("3. Blau");
        Console.WriteLine("4. Gelb");
        Console.WriteLine("5. Dunkelrot");
        Console.WriteLine("6. Dunkel Gelb");

        ConsoleKeyInfo colorChoice = Console.ReadKey();
        Console.WriteLine();

        switch (colorChoice.Key)
        {
            case ConsoleKey.D1:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case ConsoleKey.D2:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case ConsoleKey.D3:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case ConsoleKey.D4:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case ConsoleKey.D5:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                break;
            case ConsoleKey.D6:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                break;
            default:
                Console.WriteLine("Ungültige Auswahl. Die Standardfarbe wird beibehalten.");
                break;
        }

        Console.WriteLine("Wähle die Schwierigkeitsstufe:");
        Console.WriteLine("1. Superschwer");
        Console.WriteLine("2. Schwer");
        Console.WriteLine("3. Mittel");
        Console.WriteLine("4. Leicht");

        ConsoleKeyInfo difficultyChoice = Console.ReadKey();
        Console.WriteLine();

        int difficultyLevel;
        switch (difficultyChoice.Key)
        {
            case ConsoleKey.D1:
                difficultyLevel = 1;
                break;
            case ConsoleKey.D2:
                difficultyLevel = 2;
                break;
            case ConsoleKey.D3:
                difficultyLevel = 3;
                break;
            case ConsoleKey.D4:
                difficultyLevel = 4;
                break;
            default:
                Console.WriteLine("Ungültige Auswahl. Die Standardanzahl an offenen Buchstaben wird verwendet.");
                difficultyLevel = 3;
                break;
        }

        int hangman_v = 0;
        string benutzername = Environment.UserName;
        string pfad = $@"C:\Users\{benutzername}\Desktop\wörter.txt";
        string[] haeufigsteWoerter = File.ReadAllLines(pfad);
        Random zufallsGenerator = new Random();
        int zufallsIndex;
        string zufaelligesWort;

        do
        {
            zufallsIndex = zufallsGenerator.Next(0, haeufigsteWoerter.Length);
            zufaelligesWort = RemovePunctuation(haeufigsteWoerter[zufallsIndex].ToLower());
        } while (string.IsNullOrEmpty(zufaelligesWort));

        int veruche = 0;
        int laenge = zufaelligesWort.Length;
        char[] geratenesWort = new char[laenge];

        int revealedLettersCount = 0;

        switch (difficultyLevel)
        {
            case 1:
                revealedLettersCount = 0;
                break;
            case 2:
                revealedLettersCount = (int)Math.Ceiling(laenge / 4.0);
                break;
            case 3:
                revealedLettersCount = (int)Math.Ceiling(laenge / 3.0);
                break;
            case 4:
                revealedLettersCount = (int)Math.Ceiling(laenge / 2.0);
                break;
            default:
                Console.WriteLine("Ungültige Auswahl. Die Standardanzahl an offenen Buchstaben wird verwendet.");
                revealedLettersCount = (int)Math.Ceiling(laenge / 3.0);
                break;
        }

        for (int i = 0; i < laenge; i++)
        {
            geratenesWort[i] = i < revealedLettersCount ? zufaelligesWort[i] : '_';
        }

        Console.WriteLine("Dein Wort ist:");

        for (int i = 0; i < laenge; i++)
        {
            Console.Write(ToTitleCase(geratenesWort[i].ToString()));
        }
        Console.WriteLine();

        bool showWord = false;

        while (Array.IndexOf(geratenesWort, '_') != -1)
        {
            Console.WriteLine($"Bis jetzt hast du {veruche} Versuche gebraucht");
            Console.WriteLine("Welchen Buchstaben wählst du?");
            char buchstabe = Console.ReadKey().KeyChar;
            Console.WriteLine();

            veruche++;

            bool buchstabeGefunden = false;
            for (int i = 0; i < laenge; i++)
            {
                if (zufaelligesWort[i] == buchstabe)
                {
                    buchstabeGefunden = true;
                    geratenesWort[i] = buchstabe;
                }
            }

            Console.WriteLine();

            if (buchstabeGefunden)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                await PlayCoinSoundEffect($@"C:\Users\{benutzername}\Desktop\Super-Mario-coin-sound-___.wav");
                Console.WriteLine($"Der Buchstabe '{ToTitleCase(buchstabe.ToString())}' ist im Wort enthalten!");
            }
            else
            {
                hangman_v++;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                await PlayFailSoundEffect($@"C:\Users\Henry\Downloads\Spongebob-duscusting-fog-horn-Sound-effect.wav");
                Console.WriteLine($"Der Buchstabe '{ToTitleCase(buchstabe.ToString())}' ist nicht im Wort enthalten.");
                DisplayHangman($@"C:\Users\{benutzername}\Desktop\Hangman_pic\hangman{hangman_v}.txt");
            }

            Console.WriteLine();
            Console.WriteLine("Aktueller Status:");

            for (int i = 0; i < laenge; i++)
            {
                Console.Write(i == 0 ? ToTitleCase(geratenesWort[i].ToString()) : geratenesWort[i].ToString());
            }
            Console.WriteLine();
            Console.WriteLine();

            if (hangman_v >= 7)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                await PlayGameOverSoundEffect($@"C:\Users\Henry\Desktop\Super-Mario-Game-Over-Sound-Effect-_Meme-SFX_.wav");
                DisplayGameOverText($@"C:\Users\{benutzername}\Desktop\GAME OVER.txt");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine($"Du hast 7 Fehler gemacht. Leider verloren!");
                Console.WriteLine("Das Wort war:");
                Console.WriteLine(ToTitleCase(zufaelligesWort));
                Console.WriteLine("Möchtest du nochmal spielen? y/n?");
                char nochmal = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (nochmal == 'n')
                {
                    Environment.Exit(0);
                }
                else if (nochmal == 'y')
                {
                    await Main();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Ungültige Eingabe");
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                }
            }
        }

        if (showWord)
        {
            Console.WriteLine();
            Console.WriteLine("Möchtest du nochmal Spielen? y/n?");
            char nochmal = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (nochmal == 'n')
            {
                Environment.Exit(0);
            }
            else if (nochmal == 'y')
            {
                await Main();
            }
        }
        Console.Clear();
        Console.WriteLine("Das Wort wurde komplett erraten. Herzlichen Glückwunsch!");
        Console.WriteLine($"Du hast {veruche} Veruche gebarucht");
        Thread.Sleep(3000);
        Console.Beep();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
        await Main();
    }

    private static async Task PlayGameOverSoundEffect(string audioFilePath)
    {
        try
        {
            using (var waveOut = new WaveOut())
            {
                using (var reader = new WaveFileReader(audioFilePath))
                {
                    waveOut.Init(reader);

                    await Task.Run(() =>
                    {
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Abspielen der Datei: {ex.Message}");
        }
    }

    private static void DisplayHangman(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string[] hangmanArt = File.ReadAllLines(filePath);
                foreach (string line in hangmanArt)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.WriteLine($"Hangman-Datei '{filePath}' nicht gefunden!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
        }
    }

    private static void DisplayGameOverText(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string[] gameOverText = File.ReadAllLines(filePath);
                foreach (string line in gameOverText)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.WriteLine($"Die Datei '{filePath}' wurde nicht gefunden!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
        }
    }

    private static async Task PlayCoinSoundEffect(string audioFilePath)
    {
        try
        {
            if (File.Exists(audioFilePath))
            {
                using (var waveOut = new WaveOut())
                {
                    using (var reader = new WaveFileReader(audioFilePath))
                    {
                        waveOut.Init(reader);

                        await Task.Run(() =>
                        {
                            waveOut.Play();
                            while (waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(100);
                            }
                        });
                    }
                }
            }
            else
            {
                Console.WriteLine($"Die Sounddatei '{audioFilePath}' wurde nicht gefunden.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Abspielen der Sounddatei: {ex.Message}");
        }
    }
    private static async Task PlayFailSoundEffect(string audioFilePath)
    {
        try
        {
            if (File.Exists(audioFilePath))
            {
                using (var waveOut = new WaveOut())
                {
                    using (var reader = new WaveFileReader(audioFilePath))
                    {
                        waveOut.Init(reader);

                        await Task.Run(() =>
                        {
                            waveOut.Play();
                            while (waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(100);
                            }
                        });
                    }
                }
            }
            else
            {
                Console.WriteLine($"Die Sounddatei '{audioFilePath}' wurde nicht gefunden.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Abspielen der Sounddatei: {ex.Message}");
        }
    }

    private static string ToTitleCase(string str)
    {
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        return textInfo.ToTitleCase(str);
    }

    private static string RemovePunctuation(string word)
    {
        return new string(word.Where(c => !char.IsPunctuation(c)).ToArray());
    }
}
