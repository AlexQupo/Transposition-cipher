using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrZI5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        CipherAlgorithms cipherAlgor;
        char[,] _table = new char[0, 0];

        

        public MainWindow()
        {
            InitializeComponent();
            cb_encryptionMethods.SelectedIndex = (int)Algorithm.Route;
            cipherAlgor = new CipherAlgorithms();
            StartInitialization();

        }

        #region Стартовая подготовка
        void StartInitialization()
        {

            lb_columnKey.Visibility = Visibility.Hidden;
            tb_columnKey.Visibility = Visibility.Hidden;

            lb_rowKey.Visibility = Visibility.Hidden;
            tb_rowKey.Visibility = Visibility.Hidden;

            tb_columnKey.Text = String.Empty;
            tb_rowKey.Text = String.Empty;


            lb_fitRoute.Visibility = Visibility.Visible;
            cb_fitRoute.Visibility = Visibility.Visible;
            lb_dischargeRoute.Visibility = Visibility.Visible;
            cb_dischargeRoute.Visibility = Visibility.Visible;

            cb_fitRoute.SelectedIndex = (int)Fit.LeftRight;
            cb_dischargeRoute.SelectedIndex = (int)Discharge.UpDown;
            cb_fitRoute.Visibility = Visibility.Visible;
            cb_dischargeRoute.Visibility = Visibility.Visible;

            sp_tables.Children.Clear();
            sp_pattern.Children.Clear();
            sp_pattern.Visibility = Visibility.Collapsed;
            chkbox_autoSizeTable.IsEnabled = true;


            tb_message.Text = "Осталось найти выход из этой темной комнаты.";
            tb_numOfColumns.Text = "10";
            tb_numOfRows.Text = "10";
        }

        Fit _input = Fit.LeftRight;
        Discharge _output = Discharge.UpDown;
        public bool[,] _cellKardanoSet = new bool[5, 5];
        Button[,] _buttons = new Button[5, 5];
        #endregion

        #region Методы шифрования
        string DoRoute(int rows, int cols)
        {
            //составление начальной таблица
            _table = cipherAlgor.MakeTable(tb_message.Text, rows, cols, _input);
            //первая таблица
            ShowCurTable(rows, cols);
            return cipherAlgor.EncodeTable(_table, rows, cols, _output);
        }
        string DoVertical(int rows, int cols)
        {
            //составление начальной таблица
            _table = cipherAlgor.MakeTable(tb_message.Text, rows, cols, _input);
            //первая таблица
            ShowCurTable(rows, cols);

            //вторая матрица
            int[] keysCols = MakeKeys(tb_columnKey.Text);
            _table = cipherAlgor.Vertical(_table, rows, cols, keysCols);
            ShowCurTable(rows, cols);
            return cipherAlgor.EncodeTable(_table, rows, cols, _output);
        }
        string DoDouble(int rows, int cols)
        {
            //составление начальной таблица
            _table = cipherAlgor.MakeTable(tb_message.Text, rows, cols, _input);
            //первая таблица
            ShowCurTable(rows, cols);

            //вторая таблица
            int[] keysCols = MakeKeys(tb_columnKey.Text);
            int[] keysRows = MakeKeys(tb_rowKey.Text);
            _table = cipherAlgor.Vertical(_table, rows, cols, keysCols);
            ShowCurTable(rows, cols);

            //третья таблица
            _table = cipherAlgor.Horizontal(_table, rows, cols, keysRows);
            ShowCurTable(rows, cols);

            return cipherAlgor.EncodeTable(_table, rows, cols, _output);
        }
        private string DoKardano(int rows, int cols)
        {
            _table = cipherAlgor.MakeTable("", rows, cols, _input);
            _table = cipherAlgor.Kardano(_table, rows, cols, 0, _cellKardanoSet, tb_message.Text);
            ShowCurTable(rows, cols);
            _table = cipherAlgor.Kardano(_table, rows, cols, 90, _cellKardanoSet, tb_message.Text);
            ShowCurTable(rows, cols);
            _table = cipherAlgor.Kardano(_table, rows, cols, 180, _cellKardanoSet, tb_message.Text);
            ShowCurTable(rows, cols);
            _table = cipherAlgor.Kardano(_table, rows, cols, 270, _cellKardanoSet, tb_message.Text);
            ShowCurTable(rows, cols);

            return cipherAlgor.EncodeTable(_table, rows, cols, _output);
        }
        #endregion

        #region Вывод таблицы
        void ShowCurTable(int rows, int cols)
        {
            //не забыть в поле добавить ссылку на объект
            Grid newGrid = new Grid { Margin = new Thickness(30, 0, 0, 0) };

            for (int j = 0; j < Math.Max(rows, cols); j++)
            {
                newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    TextBox tb = new TextBox
                    {
                        Name = "c" + r + c, //c - cell(ячейка)
                        Text = _table[r, c].ToString(),
                        BorderBrush = Brushes.Black,
                        Padding = new Thickness(2),
                        IsReadOnly = true
                    };
                    newGrid.Children.Add(tb);
                    Grid.SetRow(tb, r);
                    Grid.SetColumn(tb, c);
                }
            }
            sp_tables.Children.Add(newGrid);
        }
        #endregion

        #region Отображение трафарета и таблицы Кардано
        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            var button = (Button)sender; //обращаемся к кнопке, которая была нажата.
            int row = int.Parse(button.Name.Split('_')[1]);
            int col = int.Parse(button.Name.Split('_')[2]);
            //если была уже выбрана то сброс
            if (_cellKardanoSet[row, col])
            {
                _cellKardanoSet[row, col] = false;
                button.Background = Brushes.Gray;
            }
            else//иначе - устанавливаем
            {
                _cellKardanoSet[row, col] = true;
                button.Background = Brushes.Yellow;
            }
        }
        void MakeKardanoPattern(int rows, int cols)
        {
            sp_pattern.Children.Clear();
            sp_tables.Children.Clear();
            _cellKardanoSet = new bool[rows, cols];
            _buttons = new Button[rows, cols];
            Grid newGrid = new Grid { Margin = new Thickness(30, 0, 0, 0) };

            for (int j = 0; j < Math.Max(rows, cols); j++)
            {
                newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Button button = new Button
                    {
                        Name = "b_" + r + "_" + c, //c - cell(ячейка)
                        Background = Brushes.Gray,
                        BorderBrush = Brushes.Black,
                    };
                    _buttons[r, c] = button;
                    button.Click += ButtonOnClick;
                    _cellKardanoSet[r, c] = false;//изначально ничего не выбрано
                    newGrid.Children.Add(button);
                    Grid.SetRow(button, r);
                    Grid.SetColumn(button, c);
                }
            }
            sp_pattern.Children.Add(newGrid);

        }
        void StartCellsPoints()
        {
            _cellKardanoSet[0, 0] = true;
            _cellKardanoSet[0, 1] = true;
            _cellKardanoSet[0, 2] = true;
            _cellKardanoSet[1, 0] = true;
            _cellKardanoSet[1, 1] = true;
            _cellKardanoSet[1, 2] = true;
            _cellKardanoSet[2, 0] = true;
            _cellKardanoSet[2, 1] = true;
            _cellKardanoSet[2, 2] = true;
        }
        void KardanoTableClick()
        {
            int n = Convert.ToInt32(Math.Sqrt(_cellKardanoSet.Length));
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (_cellKardanoSet[r, c])
                        _buttons[r, c].Background = Brushes.Yellow;
        }
        #endregion

        #region События
        private void tb_numOfColumns_Changed(object sender, TextChangedEventArgs e)
        {
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Kardano)
                if (int.TryParse(tb_numOfColumns.Text, out int cols))
                {
                    if (tb_numOfColumns.Text == tb_numOfRows.Text && Convert.ToInt32(tb_numOfColumns.Text) % 2 == 0)
                        MakeKardanoPattern(cols, cols);
                    else
                        tb_numOfRows.Text = tb_numOfColumns.Text;
                }
            if (tb_numOfColumns.Text == "" || Convert.ToInt32(tb_numOfColumns.Text) <= 0)
                tb_numOfColumns.Text = "1";
        }
        private void tb_numOfRows_Changed(object sender, TextChangedEventArgs e)
        {
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Kardano)
                if (int.TryParse(tb_numOfRows.Text, out int rows))
                {
                    if (tb_numOfColumns.Text == tb_numOfRows.Text && Convert.ToInt32(tb_numOfRows.Text) % 2 == 0)
                        MakeKardanoPattern(rows, rows);
                    else
                        tb_numOfColumns.Text = tb_numOfRows.Text;
                }
            if (tb_numOfRows.Text == "" || Convert.ToInt32(tb_numOfRows.Text) <= 0)
                tb_numOfRows.Text = "1";
            //Проверка чекбокса
            if (chkbox_autoSizeTable.IsChecked == true)
            {
                int n = 0;
                int m = Convert.ToInt32(tb_numOfRows.Text);
                int k = tb_message.Text.Length;
                n = ((k - 1) / m) + 1;
                tb_numOfColumns.Text = n.ToString();
            }

        }

        private void cb_encryptionMethods_SelectChanged(object sender, EventArgs e)
        {
            StartInitialization();

            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Vertical || cb_encryptionMethods.SelectedIndex == (int)Algorithm.Double)
            {
                lb_columnKey.Visibility = Visibility.Visible;
                tb_columnKey.Visibility = Visibility.Visible;
                chkbox_autoSizeTable.IsEnabled = true;

                tb_columnKey.Text = "3 6 2 7 1 8 9 4 5 2";
            }

            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Double)
            {
                lb_rowKey.Visibility = Visibility.Visible;
                tb_rowKey.Visibility = Visibility.Visible;
                chkbox_autoSizeTable.IsEnabled = true;

                tb_rowKey.Text = "6 2 8 4 6 9 6 7 1 4";
            }

            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Kardano)
            {
                chkbox_autoSizeTable.IsChecked = false;
                chkbox_autoSizeTable.IsEnabled = false;
                tb_numOfColumns.IsReadOnly = false;

                tb_message.Text = "Ночь.Улица.Фонарь.Пора спать, но нет";
                tb_numOfColumns.Text = "6";
                tb_numOfRows.Text = "6";
                sp_pattern.Visibility = Visibility.Visible;
                StartCellsPoints();
                KardanoTableClick();
                cb_fitRoute.Visibility = Visibility.Hidden;

            }
        }
        private void cb_fitRoute_SelectChanged(object sender, EventArgs e)
        {
            if (cb_fitRoute.SelectedIndex == (int)Fit.LeftRight)
                _input = Fit.LeftRight;
            if (cb_fitRoute.SelectedIndex == (int)Fit.RightLeft)
                _input = Fit.RightLeft;
            if (cb_fitRoute.SelectedIndex == (int)Fit.UpDown)
                _input = Fit.UpDown;
            if (cb_fitRoute.SelectedIndex == (int)Fit.DownUp)
                _input = Fit.DownUp;
        }
        private void cb_dischargeRoute_SelectChanged(object sender, EventArgs e)
        {
            if (cb_dischargeRoute.SelectedIndex == (int)Discharge.LeftRight)
                _output = Discharge.LeftRight;
            if (cb_dischargeRoute.SelectedIndex == (int)Discharge.RightLeft)
                _output = Discharge.RightLeft;
            if (cb_dischargeRoute.SelectedIndex == (int)Discharge.UpDown)
                _output = Discharge.UpDown;
            if (cb_dischargeRoute.SelectedIndex == (int)Discharge.DownUp)
                _output = Discharge.DownUp;
        }

        private void chkbox_autoSizeTable_Checked(object sender, RoutedEventArgs e)
        {
            if (chkbox_autoSizeTable.IsChecked == true)
                tb_numOfColumns.IsReadOnly = true;
            else
                tb_numOfColumns.IsReadOnly = false;
        }

        private void tb_digitCheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }
        private void tb_spaceCheck_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        #endregion

        #region Проверки на корректность ввода
        private bool isCorrectFit()
        {
            if (cb_encryptionMethods.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите метод шифрования.");
                return false;
            }
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Double && (cb_fitRoute.SelectedIndex == -1 || cb_dischargeRoute.SelectedIndex == -1))
            {
                MessageBox.Show("Выберите маршрут вписывания и выписывания.");
                return false;
            }
            if (!int.TryParse(tb_numOfColumns.Text, out int cols))
            {
                return false;
            }

            if (!int.TryParse(tb_numOfRows.Text, out int rows))
            {
                return false;
            }

            string key = "0123456789 ";
            foreach (char c in tb_columnKey.Text)
                if (!key.Contains(c))
                {
                    MessageBox.Show("Ключ может содержать только числовые значения через пробел");
                    return false;
                }

            foreach (char c in tb_rowKey.Text)
                if (!key.Contains(c))
                {
                    MessageBox.Show("Ключ может содержать только числовые значения через пробел");
                    return false;
                }
            return true;
        }

        private int[] MakeKeys(string keyText)
        {
            int[] keys;
            try
            {
                keys = keyText.Split().Select(i => int.Parse(i.ToString())).ToArray();

            }
            catch (Exception e)
            {
                var ErMsg = e.Message;
                return keys = new int[0];
            }
            return keys;
        }

        public bool areCorrectKeys(int rows, int cols)
        {
            //Vertical+Double
            int[] keysCols = MakeKeys(tb_columnKey.Text);
            if (keysCols.Length == 0)
            {
                MessageBox.Show("Ошибка ввода ключа.");
                return false;
            }
            if (keysCols.Length != cols)
            {
                MessageBox.Show("Ключ должн быть равен числу столбцов таблицы.");
                return false;
            }
            foreach (int k in keysCols)
                if (k < 1 || k > cols)
                {
                    MessageBox.Show("Значения ключа должны быть числами от 1 до n (n-количество столбцов)");
                    return false;
                }

            //Double
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Double)
            {
                int[] keysRows = MakeKeys(tb_rowKey.Text);
                if (keysRows.Length == 0)
                {
                    MessageBox.Show("Ошибка ввода ключа.");
                    return false;
                }
                if (keysRows.Length != rows)
                {
                    MessageBox.Show("Ключ должн быть равен числу строк таблицы.");
                    return false;
                }
                foreach (int k in keysRows)
                    if (k < 1 || k > cols)
                    {
                        MessageBox.Show("Значения ключа должны быть числами от 1 до n (n-количество строк)");
                        return false;
                    }
            }

            return true;
        }
        #endregion

        //Кнопка шифрования
        private void btn_encrypt_Click(object sender, RoutedEventArgs e)
        {
            if (!isCorrectFit())
                return;
            sp_tables.Children.Clear();
            int rows = int.Parse(tb_numOfRows.Text);
            int cols = int.Parse(tb_numOfColumns.Text);

            //проверка ключей
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Vertical || cb_encryptionMethods.SelectedIndex == (int)Algorithm.Double)
                if (!areCorrectKeys(rows, cols))
                    return;

            //Шифры перестановки
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Route)
                tb_result.Text = DoRoute(rows, cols);
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Vertical)
                tb_result.Text = DoVertical(rows, cols);
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Kardano)
                tb_result.Text = DoKardano(rows, cols);
            if (cb_encryptionMethods.SelectedIndex == (int)Algorithm.Double)
                tb_result.Text = DoDouble(rows, cols);
        }

    }

}

