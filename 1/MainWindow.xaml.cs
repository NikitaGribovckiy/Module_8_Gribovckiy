using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace _1
{
    public partial class MainWindow : Window
    {
        private string currentWord;
        private int attemptsLeft = 8;
        private string currentGuess = "";
        private string selectedTheme;
        private List<char> guessedLetters = new List<char>();

        public MainWindow(string theme)
        {
            InitializeComponent();
            selectedTheme = theme;
            InitializeGame();
            GuessTextBox.PreviewTextInput += GuessTextBox_PreviewTextInput;
            GuessTextBox.KeyUp += GuessTextBox_KeyUp; // Обработчик для клавиши Enter
        }

        private void InitializeGame()
        {
            Random random = new Random();
            string[] themeWordList;

            switch (selectedTheme)
            {
                case "Фрукты":
                    themeWordList = new string[] { "яблоко", "банан", "вишня", "апельсин", "груша", "слива", "абрикос", "киви", "грейпфрут", "манго" };
                    break;
                case "Животные":
                    themeWordList = new string[] { "собака", "кошка", "слон", "тигр", "лев", "жираф", "крокодил", "пингвин", "хорек", "белка" };
                    break;
                case "Страны":
                    themeWordList = new string[] { "россия", "сша", "франция", "китай", "германия", "бразилия", "япония", "австралия", "канада", "индия" };
                    break;
                case "Цвета":
                    themeWordList = new string[] { "красный", "синий", "зеленый", "желтый", "оранжевый", "фиолетовый", "черный", "белый", "розовый", "коричневый" };
                    break;
                case "Профессии":
                    themeWordList = new string[] { "доктор", "учитель", "инженер", "пожарник", "полицейский", "актер", "певец", "повар", "столяр", "медсестра" };
                    break;
                case "Транспорт":
                    themeWordList = new string[] { "автомобиль", "велосипед", "поезд", "самолет", "корабль", "метро", "трамвай", "мотоцикл", "такси", "автобус" };
                    break;
                case "Планеты":
                    themeWordList = new string[] { "меркурий", "венера", "земля", "марс", "юпитер", "сатурн", "уран", "нептун", "плутон", "марсиан" };
                    break;
                case "Фильмы":
                    themeWordList = new string[] { "звездные войны", "гарри поттер", "властелин колец", "титаник", "аватар", "интерстеллар", "пираты карибского моря", "терминатор", "мстители", "бойцовский клуб" };
                    break;
                case "Еда":
                    themeWordList = new string[] { "пицца", "бургер", "суши", "салат", "паста", "борщ", "суп", "ролл", "сэндвич", "кебаб" };
                    break;
                case "Растения":
                    themeWordList = new string[] { "роза", "тюльпан", "подсолнух", "орхидея", "герань", "лилия", "мак", "кактус", "папоротник", "дуб" };
                    break;
                default:
                    themeWordList = new string[] { "яблоко", "банан", "вишня", "апельсин", "груша", "слива", "абрикос", "киви", "грейпфрут", "манго" };
                    break;
            }

            currentWord = themeWordList[random.Next(themeWordList.Length)];

            // Замените пробелы в слове на символ подчеркивания
            currentGuess = string.Join(" ", currentWord.Select(c => c == ' ' ? ' ' : '_'));
            AttemptsTextBlock.Text = "Попытки: " + attemptsLeft;
            ThemeTextBlock.Text = "Тема: " + selectedTheme;
            WordDisplay.Text = currentGuess;
            WordLength.Text = "Количество букв: " + currentWord.Length;
            attemptsLeft = 8;
            guessedLetters.Clear();
        }

        private void CheckGuess(char guess)
        {
            bool correctGuess = false;
            char[] updatedGuess = currentGuess.ToCharArray();

            if (guessedLetters.Contains(guess))
            {
                MessageBox.Show("Вы уже угадали эту букву.");
            }
            else
            {
                guessedLetters.Add(guess);

                for (int i = 0; i < currentWord.Length; i++)
                {
                    if (currentWord[i] == guess)
                    {
                        updatedGuess[i * 2] = guess;
                        correctGuess = true;
                    }
                }

                currentGuess = new string(updatedGuess);
                WordDisplay.Text = currentGuess;

                if (!correctGuess)
                {
                    attemptsLeft--;
                    HangmanImage.Source = new BitmapImage(new Uri("/Images/hangman" + (8 - attemptsLeft) + ".png", UriKind.Relative));

                    // Обновляем значение количества попыток в AttemptsTextBlock
                    AttemptsTextBlock.Text = "Попытки: " + attemptsLeft;

                    if (attemptsLeft == 0)
                    {
                        MessageBox.Show("Вы проиграли! Загаданное слово: " + currentWord);
                        InitializeGame();
                    }
                }
                else if (!currentGuess.Contains('_'))
                {
                    MessageBox.Show("Поздравляем! Вы выиграли!");
                    InitializeGame();
                }
            }
        }


        private void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessGuess();
        }

        private void GuessTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsRussianLetter(e.Text);
        }

        private void GuessTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessGuess();
            }
        }

        private void ProcessGuess()
        {
            if (!string.IsNullOrWhiteSpace(GuessTextBox.Text) && GuessTextBox.Text.Length == 1)
            {
                char guess = char.ToLower(GuessTextBox.Text[0]);
                CheckGuess(guess);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите одну действительную букву.");
            }

            GuessTextBox.Text = string.Empty;
        }

        private bool IsRussianLetter(string text)
        {
            string pattern = @"^[А-Яа-я]$";
            return Regex.IsMatch(text, pattern);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            Close();
        }
    }
}
